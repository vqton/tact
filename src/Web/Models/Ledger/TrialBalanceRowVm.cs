namespace Web.Models.Ledger;

public class TrialBalanceRowVm
{
    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal TotalDebit { get; init; }

    public decimal TotalCredit { get; init; }
}
