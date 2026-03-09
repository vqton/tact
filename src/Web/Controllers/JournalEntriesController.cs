namespace Web.Controllers;

using System.Security.Claims;
using AccountingVAS.Web.Data;
using AccountingVAS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JournalEntryCreateVm = global::Web.Models.JournalEntries.JournalEntryCreateVm;
using JournalEntryLineVm = global::Web.Models.JournalEntries.JournalEntryLineVm;

[Authorize(Roles = "Admin,Accountant")]
public class JournalEntriesController(AccountingDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateAccountOptionsAsync();
        return View(new JournalEntryCreateVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JournalEntryCreateVm model)
    {
        await ValidateLinesAsync(model.Lines);
        ValidateTotals(model.Lines);

        if (!ModelState.IsValid)
        {
            await PopulateAccountOptionsAsync();
            return View(model);
        }

        var createdByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var journalEntry = JournalEntry.Create(
                model.Date,
                model.Description,
                model.VoucherRef,
                createdByUserId);

            await context.JournalEntries.AddAsync(journalEntry);
            await context.SaveChangesAsync();

            foreach (var line in model.Lines.Where(l => l.AccountId.HasValue && (l.Debit > 0 || l.Credit > 0)))
            {
                var entry = TransactionEntry.Create(
                    journalEntry.Id,
                    line.AccountId!.Value,
                    line.Debit,
                    line.Credit);

                journalEntry.AddEntry(entry);
                await context.TransactionEntries.AddAsync(entry);
            }

            journalEntry.Post(createdByUserId);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, $"Khong the luu chung tu: {ex.Message}");
            await PopulateAccountOptionsAsync();
            return View(model);
        }

        return RedirectToAction(nameof(Create));
    }

    private async Task ValidateLinesAsync(IReadOnlyList<JournalEntryLineVm> lines)
    {
        if (lines.Count < 2)
        {
            ModelState.AddModelError(nameof(JournalEntryCreateVm.Lines), "Can it nhat 2 dong dinh khoan.");
            return;
        }

        var activeAccountIds = await context.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive)
            .Select(a => a.Id)
            .ToListAsync();

        var activeLookup = activeAccountIds.ToHashSet();
        var validLineCount = 0;
        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var hasAmount = line.Debit > 0 || line.Credit > 0;
            if (!hasAmount)
            {
                continue;
            }

            validLineCount++;
            if (!line.AccountId.HasValue || !activeLookup.Contains(line.AccountId.Value))
            {
                ModelState.AddModelError($"Lines[{index}].AccountId", "Tai khoan khong ton tai hoac ngung su dung.");
            }

            if (line.Debit > 0 && line.Credit > 0)
            {
                ModelState.AddModelError($"Lines[{index}].Debit", "Moi dong chi duoc nhap No hoac Co.");
            }
        }

        if (validLineCount < 2)
        {
            ModelState.AddModelError(nameof(JournalEntryCreateVm.Lines), "Can it nhat 2 dong co so tien.");
        }
    }

    private void ValidateTotals(IEnumerable<JournalEntryLineVm> lines)
    {
        var totalDebit = lines.Sum(l => l.Debit);
        var totalCredit = lines.Sum(l => l.Credit);

        if (totalDebit != totalCredit)
        {
            ModelState.AddModelError(nameof(JournalEntryCreateVm.Lines), "Tong No phai bang Tong Co.");
        }
    }

    private async Task PopulateAccountOptionsAsync()
    {
        var accountOptions = await context.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Code)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Code} - {a.Name}"
            })
            .ToListAsync();

        ViewBag.AccountOptions = accountOptions;
    }
}
