using System.Data.Common;
using AccountingVAS.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Web.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AccountingDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddSingleton<DbConnection>(_connection);
            services.AddDbContext<AccountingDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            services.PostConfigure<MvcOptions>(options =>
            {
                options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS Accounts (
                Id INTEGER NOT NULL CONSTRAINT PK_Accounts PRIMARY KEY AUTOINCREMENT,
                Code TEXT NOT NULL,
                Name TEXT NOT NULL,
                Type INTEGER NOT NULL,
                ParentId INTEGER NULL,
                Level INTEGER NOT NULL DEFAULT 1,
                IsActive INTEGER NOT NULL DEFAULT 1,
                Balance TEXT NOT NULL DEFAULT 0,
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                ModifiedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT FK_Accounts_Accounts_ParentId FOREIGN KEY (ParentId) REFERENCES Accounts (Id) ON DELETE RESTRICT
            );
            CREATE UNIQUE INDEX IF NOT EXISTS ix_account_code ON Accounts (Code);
            CREATE INDEX IF NOT EXISTS ix_account_parentid ON Accounts (ParentId);
            CREATE INDEX IF NOT EXISTS ix_account_isactive ON Accounts (IsActive);

            CREATE TABLE IF NOT EXISTS JournalEntries (
                Id INTEGER NOT NULL CONSTRAINT PK_JournalEntries PRIMARY KEY AUTOINCREMENT,
                Date TEXT NOT NULL,
                Description TEXT NOT NULL,
                VoucherRef TEXT NOT NULL,
                Status INTEGER NOT NULL,
                CreatedByUserId TEXT NOT NULL,
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                ModifiedByUserId TEXT NULL,
                ModifiedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS ix_journalentry_date ON JournalEntries (Date);
            CREATE INDEX IF NOT EXISTS ix_journalentry_status ON JournalEntries (Status);

            CREATE TABLE IF NOT EXISTS TransactionEntries (
                Id INTEGER NOT NULL CONSTRAINT PK_TransactionEntries PRIMARY KEY AUTOINCREMENT,
                JournalEntryId INTEGER NOT NULL,
                AccountId INTEGER NOT NULL,
                Debit TEXT NOT NULL DEFAULT 0,
                Credit TEXT NOT NULL DEFAULT 0,
                Description TEXT NOT NULL DEFAULT '',
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT FK_TransactionEntries_JournalEntries_JournalEntryId FOREIGN KEY (JournalEntryId) REFERENCES JournalEntries (Id) ON DELETE CASCADE,
                CONSTRAINT FK_TransactionEntries_Accounts_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts (Id) ON DELETE RESTRICT
            );
            CREATE INDEX IF NOT EXISTS ix_transactionentry_journalentryid ON TransactionEntries (JournalEntryId);
            CREATE INDEX IF NOT EXISTS ix_transactionentry_accountid ON TransactionEntries (AccountId);
            """);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _connection.DisposeAsync();
        await base.DisposeAsync();
    }
}
