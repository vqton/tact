namespace Web.Models.Reports;

public class CashFlowRowVm
{
    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal CashIn { get; init; }

    public decimal CashOut { get; init; }
}
