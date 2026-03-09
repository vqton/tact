using AccountingVAS.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly AccountingDbContext _context;

    public HomeController(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Bảng điều khiển";
        ViewBag.TotalAccounts = await _context.Accounts.AsNoTracking().CountAsync();
        ViewBag.TotalJournalEntries = await _context.JournalEntries.AsNoTracking().CountAsync();
        ViewBag.TotalTransactionEntries = await _context.TransactionEntries.AsNoTracking().CountAsync();
        ViewBag.TotalAuditLogs = await _context.AuditLogs.AsNoTracking().CountAsync();

        return View();
    }
}
