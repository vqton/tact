namespace Web.Models.Reports;

public class BalanceSheetRowVm
{
    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public string AccountType { get; init; } = string.Empty;

    public decimal Balance { get; init; }
}
