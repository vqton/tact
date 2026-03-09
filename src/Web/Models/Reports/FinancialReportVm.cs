namespace Web.Models.Reports;

using FastReport.Web;

public class FinancialReportVm
{
    public string Title { get; init; } = string.Empty;

    public string ActionName { get; init; } = string.Empty;

    public DateTime? AsOfDate { get; init; }

    public DateTime? FromDate { get; init; }

    public DateTime? ToDate { get; init; }

    public WebReport? WebReport { get; init; }
}
