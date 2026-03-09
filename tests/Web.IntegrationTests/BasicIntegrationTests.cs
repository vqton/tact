using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using AccountingVAS.Web.Data;
using AccountingVAS.Web.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Web.IntegrationTests;

public sealed class BasicIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateAccount_ValidInput_PersistsAndRedirects()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var token = await GetRequestVerificationTokenAsync(client, "/ChartOfAccounts/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Code"] = "9001",
            ["Name"] = "Tai khoan test",
            ["Type"] = ((int)AccountType.Asset).ToString(),
            ["ParentId"] = string.Empty
        };

        using var response = await client.PostAsync("/ChartOfAccounts/Create", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var account = await context.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.Code == "9001");

        Assert.NotNull(account);
        Assert.Equal("Tai khoan test", account.Name);
        Assert.True(account.IsActive);
    }

    [Fact]
    public async Task CreateJournalEntry_UnbalancedLines_ReturnsValidationErrorAndDoesNotPersist()
    {
        await using var setupScope = factory.Services.CreateAsyncScope();
        var setupContext = setupScope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var debitAccount = await EnsureAccountAsync(setupContext, "9002", "Tai khoan No", AccountType.Asset);
        var creditAccount = await EnsureAccountAsync(setupContext, "9003", "Tai khoan Co", AccountType.Revenue);

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        var token = await GetRequestVerificationTokenAsync(client, "/JournalEntries/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["VoucherRef"] = "PT-0001",
            ["Date"] = "2026-03-09",
            ["Description"] = "Chung tu khong can",
            ["Lines[0].AccountId"] = debitAccount.Id.ToString(),
            ["Lines[0].Debit"] = "100000",
            ["Lines[0].Credit"] = "0",
            ["Lines[1].AccountId"] = creditAccount.Id.ToString(),
            ["Lines[1].Debit"] = "0",
            ["Lines[1].Credit"] = "90000"
        };

        using var response = await client.PostAsync("/JournalEntries/Create", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Tong No phai bang Tong Co", html);

        await using var assertScope = factory.Services.CreateAsyncScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var created = await assertContext.JournalEntries.AsNoTracking().SingleOrDefaultAsync(x => x.VoucherRef == "PT-0001");
        Assert.Null(created);
    }

    [Fact]
    public async Task LedgerIndex_FilterByAccount_ReturnsExpectedRows()
    {
        await using (var seedScope = factory.Services.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<AccountingDbContext>();
            var cashAccount = await EnsureAccountAsync(context, "9004", "Tien mat test", AccountType.Asset);
            var revenueAccount = await EnsureAccountAsync(context, "9005", "Doanh thu test", AccountType.Revenue);

            var journalEntry = JournalEntry.Create(new DateTime(2026, 3, 8), "Ban hang test", "PT-0002", "integration-user");
            await context.JournalEntries.AddAsync(journalEntry);
            await context.SaveChangesAsync();

            var debitEntry = TransactionEntry.Create(journalEntry.Id, cashAccount.Id, 200000m, 0m);
            var creditEntry = TransactionEntry.Create(journalEntry.Id, revenueAccount.Id, 0m, 200000m);
            journalEntry.AddEntry(debitEntry);
            journalEntry.AddEntry(creditEntry);
            journalEntry.Post("integration-user");

            await context.TransactionEntries.AddRangeAsync(debitEntry, creditEntry);
            await context.SaveChangesAsync();
        }

        int accountId;
        await using (var lookupScope = factory.Services.CreateAsyncScope())
        {
            var lookupContext = lookupScope.ServiceProvider.GetRequiredService<AccountingDbContext>();
            accountId = await lookupContext.Accounts
                .Where(a => a.Code == "9004")
                .Select(a => a.Id)
                .SingleAsync();
        }

        using var client = factory.CreateClient();
        var response = await client.GetAsync($"/Ledger?accountId={accountId}&fromDate=2026-03-01&toDate=2026-03-31");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Ban hang test", html);
        Assert.Contains("9004", html);
    }

    private static async Task<Account> EnsureAccountAsync(
        AccountingDbContext context,
        string code,
        string name,
        AccountType type)
    {
        var existing = await context.Accounts.FirstOrDefaultAsync(a => a.Code == code);
        if (existing is not null)
        {
            return existing;
        }

        var account = Account.Create(code, name, type, level: 1);
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();
        return account;
    }

    private static async Task<string> GetRequestVerificationTokenAsync(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(
            html,
            "name=\"__RequestVerificationToken\"\\s+type=\"hidden\"\\s+value=\"([^\"]+)\"");

        Assert.True(match.Success, "Anti-forgery token was not found in form HTML.");
        return match.Groups[1].Value;
    }
}
