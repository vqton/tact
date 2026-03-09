namespace Web.Controllers;

using AccountingVAS.Web.Data;
using AccountingVAS.Web.Models;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BalanceSheetRowVm = global::Web.Models.Reports.BalanceSheetRowVm;
using CashFlowRowVm = global::Web.Models.Reports.CashFlowRowVm;
using FinancialReportVm = global::Web.Models.Reports.FinancialReportVm;
using IncomeStatementRowVm = global::Web.Models.Reports.IncomeStatementRowVm;

[Authorize(Roles = "Admin,Accountant")]
public class ReportsController(AccountingDbContext context, IWebHostEnvironment environment) : Controller
{
    [HttpGet]
    public async Task<IActionResult> BalanceSheet(DateTime? asOfDate, string? format)
    {
        var effectiveDate = asOfDate?.Date ?? DateTime.Today;
        var data = await GetBalanceSheetDataAsync(effectiveDate);

        if (IsPdfRequest(format))
        {
            var bytes = ExportPdf("BalanceSheet.frx", "BalanceSheetData", data);
            return File(bytes, "application/pdf", $"BangCanDoiKeToan_{effectiveDate:yyyyMMdd}.pdf");
        }

        var model = new FinancialReportVm
        {
            Title = "Bảng cân đối kế toán",
            ActionName = nameof(BalanceSheet),
            AsOfDate = effectiveDate,
            WebReport = BuildWebReport("BalanceSheet.frx", "BalanceSheetData", data)
        };

        if (IsHtmxRequest())
        {
            return PartialView("_ReportPreview", model);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> IncomeStatement(DateTime? fromDate, DateTime? toDate, string? format)
    {
        var startDate = (fromDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1));
        var endDate = (toDate?.Date ?? DateTime.Today);
        if (startDate > endDate)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        var data = await GetIncomeStatementDataAsync(startDate, endDate);

        if (IsPdfRequest(format))
        {
            var bytes = ExportPdf("IncomeStatement.frx", "IncomeStatementData", data);
            return File(bytes, "application/pdf", $"BaoCaoKetQuaKinhDoanh_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        var model = new FinancialReportVm
        {
            Title = "Báo cáo kết quả kinh doanh",
            ActionName = nameof(IncomeStatement),
            FromDate = startDate,
            ToDate = endDate,
            WebReport = BuildWebReport("IncomeStatement.frx", "IncomeStatementData", data)
        };

        if (IsHtmxRequest())
        {
            return PartialView("_ReportPreview", model);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> CashFlow(DateTime? fromDate, DateTime? toDate, string? format)
    {
        var startDate = (fromDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1));
        var endDate = (toDate?.Date ?? DateTime.Today);
        if (startDate > endDate)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        var data = await GetCashFlowDataAsync(startDate, endDate);

        if (IsPdfRequest(format))
        {
            var bytes = ExportPdf("CashFlow.frx", "CashFlowData", data);
            return File(bytes, "application/pdf", $"BaoCaoLuuChuyenTienTe_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        var model = new FinancialReportVm
        {
            Title = "Báo cáo lưu chuyển tiền tệ",
            ActionName = nameof(CashFlow),
            FromDate = startDate,
            ToDate = endDate,
            WebReport = BuildWebReport("CashFlow.frx", "CashFlowData", data)
        };

        if (IsHtmxRequest())
        {
            return PartialView("_ReportPreview", model);
        }

        return View(model);
    }

    private WebReport BuildWebReport<T>(string templateName, string dataSourceName, IReadOnlyCollection<T> data)
    {
        var webReport = new WebReport();
        var templatePath = GetTemplatePath(templateName);

        webReport.Report.Load(templatePath);
        webReport.Report.RegisterData(data, dataSourceName);
        var webDataSource = webReport.Report.GetDataSource(dataSourceName);
        if (webDataSource is not null)
        {
            webDataSource.Enabled = true;
        }
        webReport.Report.Prepare();

        return webReport;
    }

    private byte[] ExportPdf<T>(string templateName, string dataSourceName, IReadOnlyCollection<T> data)
    {
        using var report = new Report();
        using var export = new PDFSimpleExport();
        using var stream = new MemoryStream();
        var templatePath = GetTemplatePath(templateName);

        report.Load(templatePath);
        report.RegisterData(data, dataSourceName);
        var dataSource = report.GetDataSource(dataSourceName);
        if (dataSource is not null)
        {
            dataSource.Enabled = true;
        }
        report.Prepare();
        report.Export(export, stream);

        return stream.ToArray();
    }

    private async Task<List<BalanceSheetRowVm>> GetBalanceSheetDataAsync(DateTime asOfDate)
    {
        var cutoff = asOfDate.Date.AddDays(1);

        return await context.TransactionEntries
            .AsNoTracking()
            .Where(t =>
                t.JournalEntry != null &&
                t.Account != null &&
                t.JournalEntry.Status == JournalEntryStatus.Posted &&
                t.JournalEntry.Date < cutoff &&
                (t.Account.Type == AccountType.Asset ||
                 t.Account.Type == AccountType.Liability ||
                 t.Account.Type == AccountType.Equity))
            .GroupBy(t => new { t.Account!.Code, t.Account.Name, t.Account.Type })
            .Select(g => new BalanceSheetRowVm
            {
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                AccountType = GetAccountTypeName(g.Key.Type),
                Balance = g.Key.Type == AccountType.Asset
                    ? g.Sum(x => x.Debit - x.Credit)
                    : g.Sum(x => x.Credit - x.Debit)
            })
            .OrderBy(x => x.AccountCode)
            .ToListAsync();
    }

    private async Task<List<IncomeStatementRowVm>> GetIncomeStatementDataAsync(DateTime fromDate, DateTime toDate)
    {
        var start = fromDate.Date;
        var endExclusive = toDate.Date.AddDays(1);

        return await context.TransactionEntries
            .AsNoTracking()
            .Where(t =>
                t.JournalEntry != null &&
                t.Account != null &&
                t.JournalEntry.Status == JournalEntryStatus.Posted &&
                t.JournalEntry.Date >= start &&
                t.JournalEntry.Date < endExclusive &&
                (t.Account.Type == AccountType.Revenue || t.Account.Type == AccountType.Expense))
            .GroupBy(t => new { t.Account!.Code, t.Account.Name, t.Account.Type })
            .Select(g => new IncomeStatementRowVm
            {
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                AccountType = GetAccountTypeName(g.Key.Type),
                NetAmount = g.Key.Type == AccountType.Revenue
                    ? g.Sum(x => x.Credit - x.Debit)
                    : g.Sum(x => x.Debit - x.Credit)
            })
            .OrderBy(x => x.AccountCode)
            .ToListAsync();
    }

    private async Task<List<CashFlowRowVm>> GetCashFlowDataAsync(DateTime fromDate, DateTime toDate)
    {
        var start = fromDate.Date;
        var endExclusive = toDate.Date.AddDays(1);

        return await context.TransactionEntries
            .AsNoTracking()
            .Where(t =>
                t.JournalEntry != null &&
                t.Account != null &&
                t.JournalEntry.Status == JournalEntryStatus.Posted &&
                t.JournalEntry.Date >= start &&
                t.JournalEntry.Date < endExclusive &&
                (t.Account.Code.StartsWith("111") || t.Account.Code.StartsWith("112")))
            .GroupBy(t => new { t.Account!.Code, t.Account.Name })
            .Select(g => new CashFlowRowVm
            {
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                CashIn = g.Sum(x => x.Debit),
                CashOut = g.Sum(x => x.Credit)
            })
            .OrderBy(x => x.AccountCode)
            .ToListAsync();
    }

    private string GetTemplatePath(string templateName)
    {
        var templatePath = Path.Combine(environment.WebRootPath, "Reports", templateName);
        if (!System.IO.File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Khong tim thay mau bao cao: {templateName}", templatePath);
        }

        return templatePath;
    }

    private bool IsHtmxRequest()
    {
        return string.Equals(Request.Headers["HX-Request"], "true", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPdfRequest(string? format)
    {
        return string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase);
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
