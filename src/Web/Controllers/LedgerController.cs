namespace Web.Controllers;

using AccountingVAS.Web.Data;
using AccountingVAS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LedgerFilterVm = global::Web.Models.Ledger.LedgerFilterVm;
using LedgerRowVm = global::Web.Models.Ledger.LedgerRowVm;
using TrialBalanceGroupVm = global::Web.Models.Ledger.TrialBalanceGroupVm;
using TrialBalanceRowVm = global::Web.Models.Ledger.TrialBalanceRowVm;
using TrialBalanceVm = global::Web.Models.Ledger.TrialBalanceVm;

[Authorize(Roles = "Admin,Accountant")]
public class LedgerController(AccountingDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? accountId, DateTime? fromDate, DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            (fromDate, toDate) = (toDate, fromDate);
        }

        var query = context.TransactionEntries
            .AsNoTracking()
            .Where(t => t.JournalEntry != null && t.Account != null && t.JournalEntry.Status == JournalEntryStatus.Posted);

        if (accountId.HasValue)
        {
            query = query.Where(t => t.AccountId == accountId.Value);
        }

        if (fromDate.HasValue)
        {
            var start = fromDate.Value.Date;
            query = query.Where(t => t.JournalEntry!.Date >= start);
        }

        if (toDate.HasValue)
        {
            var endExclusive = toDate.Value.Date.AddDays(1);
            query = query.Where(t => t.JournalEntry!.Date < endExclusive);
        }

        var rows = await query
            .OrderBy(t => t.JournalEntry!.Date)
            .ThenBy(t => t.JournalEntryId)
            .ThenBy(t => t.Id)
            .Select(t => new LedgerRowVm
            {
                Date = t.JournalEntry!.Date,
                VoucherRef = t.JournalEntry!.VoucherRef,
                Description = t.JournalEntry!.Description,
                AccountCode = t.Account!.Code,
                AccountName = t.Account!.Name,
                Debit = t.Debit,
                Credit = t.Credit
            })
            .ToListAsync();

        var model = new LedgerFilterVm
        {
            AccountId = accountId,
            FromDate = fromDate?.Date,
            ToDate = toDate?.Date,
            AccountOptions = await GetAccountOptionsAsync(accountId),
            Rows = rows
        };

        if (IsHtmxRequest())
        {
            return PartialView("_LedgerTable", model);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> TrialBalance()
    {
        var rows = await context.TransactionEntries
            .AsNoTracking()
            .Where(t => t.JournalEntry != null && t.Account != null && t.JournalEntry.Status == JournalEntryStatus.Posted)
            .GroupBy(t => new { t.AccountId, t.Account!.Code, t.Account.Name, t.Account.Type })
            .Select(g => new
            {
                g.Key.Code,
                g.Key.Name,
                g.Key.Type,
                TotalDebit = g.Sum(x => x.Debit),
                TotalCredit = g.Sum(x => x.Credit)
            })
            .OrderBy(x => x.Code)
            .ToListAsync();

        var groups = rows
            .GroupBy(x => x.Type)
            .OrderBy(g => (int)g.Key)
            .Select(g => new TrialBalanceGroupVm
            {
                GroupName = GetAccountTypeName(g.Key),
                Rows = g.Select(r => new TrialBalanceRowVm
                    {
                        AccountCode = r.Code,
                        AccountName = r.Name,
                        TotalDebit = r.TotalDebit,
                        TotalCredit = r.TotalCredit
                    })
                    .OrderBy(r => r.AccountCode)
                    .ToList(),
                SubtotalDebit = g.Sum(x => x.TotalDebit),
                SubtotalCredit = g.Sum(x => x.TotalCredit)
            })
            .ToList();

        var model = new TrialBalanceVm
        {
            Groups = groups,
            TotalDebit = groups.Sum(g => g.SubtotalDebit),
            TotalCredit = groups.Sum(g => g.SubtotalCredit)
        };

        return View(model);
    }

    private async Task<IReadOnlyList<SelectListItem>> GetAccountOptionsAsync(int? selectedAccountId)
    {
        var options = await context.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Code)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Code} - {a.Name}",
                Selected = selectedAccountId.HasValue && a.Id == selectedAccountId.Value
            })
            .ToListAsync();

        options.Insert(0, new SelectListItem
        {
            Value = string.Empty,
            Text = "Tất cả tài khoản",
            Selected = !selectedAccountId.HasValue
        });

        return options;
    }

    private bool IsHtmxRequest()
    {
        return string.Equals(Request.Headers["HX-Request"], "true", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetAccountTypeName(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Asset => "Tài sản",
            AccountType.Liability => "Nợ phải trả",
            AccountType.Equity => "Vốn chủ sở hữu",
            AccountType.Revenue => "Doanh thu",
            AccountType.Expense => "Chi phí",
            _ => "Khác"
        };
    }
}
