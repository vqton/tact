namespace Web.Models.Ledger;

public class TrialBalanceGroupVm
{
    public string GroupName { get; init; } = string.Empty;

    public IReadOnlyList<TrialBalanceRowVm> Rows { get; init; } = [];

    public decimal SubtotalDebit { get; init; }

    public decimal SubtotalCredit { get; init; }
}
