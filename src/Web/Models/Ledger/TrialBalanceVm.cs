namespace Web.Models.Ledger;

public class TrialBalanceVm
{
    public IReadOnlyList<TrialBalanceGroupVm> Groups { get; init; } = [];

    public decimal TotalDebit { get; init; }

    public decimal TotalCredit { get; init; }
}
