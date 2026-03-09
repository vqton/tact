namespace Web.Controllers;

using System.Text.RegularExpressions;
using AccountingVAS.Web.Data;
using AccountingVAS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChartOfAccountFormVm = global::Web.Models.ChartOfAccounts.ChartOfAccountFormVm;

[Authorize(Roles = "Admin,Accountant")]
public class ChartOfAccountsController(AccountingDbContext context) : Controller
{
    private static readonly Regex FourDigitCodeRegex = new(@"^\d{4}$", RegexOptions.Compiled);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accounts = await GetActiveAccountsAsync();
        return View(accounts);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateParentOptionsAsync();
        return View(new ChartOfAccountFormVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChartOfAccountFormVm model)
    {
        await ValidateFormAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateParentOptionsAsync(model.ParentId);
            return View(model);
        }

        var level = await ResolveLevelAsync(model.ParentId);
        var account = Account.Create(model.Code, model.Name, model.Type, model.ParentId, level);
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();

        if (IsHtmxRequest())
        {
            Response.Headers["HX-Trigger"] = "coa-form-clear";
            var accounts = await GetActiveAccountsAsync();
            return PartialView("_ChartOfAccountTable", accounts);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        if (account is null)
        {
            return NotFound();
        }

        await PopulateParentOptionsAsync(account.ParentId, id);
        var model = new ChartOfAccountFormVm
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            Type = account.Type,
            ParentId = account.ParentId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ChartOfAccountFormVm model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        if (account is null)
        {
            return NotFound();
        }

        await ValidateFormAsync(model, id);

        if (model.ParentId.HasValue && await CreatesCircularHierarchyAsync(id, model.ParentId.Value))
        {
            ModelState.AddModelError(nameof(model.ParentId), "Tai khoan cha khong hop le.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateParentOptionsAsync(model.ParentId, id);
            return View(model);
        }

        var level = await ResolveLevelAsync(model.ParentId);
        account.UpdateDetails(model.Code, model.Name, model.Type, model.ParentId, level);
        await context.SaveChangesAsync();

        if (IsHtmxRequest())
        {
            Response.Headers["HX-Trigger"] = "coa-form-clear";
            var accounts = await GetActiveAccountsAsync();
            return PartialView("_ChartOfAccountTable", accounts);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        if (account is null)
        {
            return NotFound();
        }

        var hasChildren = await context.Accounts.AnyAsync(a => a.ParentId == id && a.IsActive);
        if (hasChildren)
        {
            ModelState.AddModelError(string.Empty, "Khong the xoa tai khoan dang co tai khoan con.");
        }

        var hasTransactions = await context.TransactionEntries.AnyAsync(t => t.AccountId == id);
        if (hasTransactions)
        {
            ModelState.AddModelError(string.Empty, "Khong the xoa tai khoan da phat sinh nghiep vu.");
        }

        if (ModelState.IsValid)
        {
            account.Deactivate();
            await context.SaveChangesAsync();
        }

        var accounts = await GetActiveAccountsAsync();
        if (IsHtmxRequest())
        {
            return PartialView("_ChartOfAccountTable", accounts);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateFormAsync(ChartOfAccountFormVm model, int? currentId = null)
    {
        if (!FourDigitCodeRegex.IsMatch(model.Code))
        {
            ModelState.AddModelError(nameof(model.Code), "Ma TK phai gom dung 4 chu so.");
        }

        var code = model.Code.Trim();
        var hasDuplicateCode = await context.Accounts
            .AnyAsync(a => a.Code == code && (!currentId.HasValue || a.Id != currentId.Value));
        if (hasDuplicateCode)
        {
            ModelState.AddModelError(nameof(model.Code), "Ma TK da ton tai.");
        }

        if (model.ParentId.HasValue)
        {
            var parent = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == model.ParentId.Value && a.IsActive);
            if (parent is null)
            {
                ModelState.AddModelError(nameof(model.ParentId), "Tai khoan cha khong ton tai.");
            }
        }
    }

    private async Task<int> ResolveLevelAsync(int? parentId)
    {
        if (!parentId.HasValue)
        {
            return 1;
        }

        var parentLevel = await context.Accounts
            .AsNoTracking()
            .Where(a => a.Id == parentId.Value)
            .Select(a => a.Level)
            .FirstOrDefaultAsync();
        return Math.Min(parentLevel + 1, 5);
    }

    private async Task<bool> CreatesCircularHierarchyAsync(int accountId, int parentId)
    {
        var parentLookup = await context.Accounts
            .AsNoTracking()
            .Select(a => new { a.Id, a.ParentId })
            .ToDictionaryAsync(a => a.Id, a => a.ParentId);

        var currentParentId = parentId;
        while (true)
        {
            if (currentParentId == accountId)
            {
                return true;
            }

            if (!parentLookup.TryGetValue(currentParentId, out var nextParentId) || !nextParentId.HasValue)
            {
                return false;
            }

            currentParentId = nextParentId.Value;
        }
    }

    private async Task<List<Account>> GetActiveAccountsAsync()
    {
        return await context.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Code)
            .ToListAsync();
    }

    private async Task PopulateParentOptionsAsync(int? selectedId = null, int? excludeId = null)
    {
        var query = context.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        var parentOptions = await query
            .OrderBy(a => a.Code)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Code} - {a.Name}",
                Selected = selectedId.HasValue && a.Id == selectedId.Value
            })
            .ToListAsync();

        parentOptions.Insert(0, new SelectListItem { Value = string.Empty, Text = "Khong co" });
        ViewBag.ParentOptions = parentOptions;
    }

    private bool IsHtmxRequest()
    {
        return string.Equals(Request.Headers["HX-Request"], "true", StringComparison.OrdinalIgnoreCase);
    }
}
