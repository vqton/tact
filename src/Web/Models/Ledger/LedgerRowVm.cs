namespace Web.Models.Ledger;

public class LedgerRowVm
{
    public DateTime Date { get; init; }

    public string VoucherRef { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal Debit { get; init; }

    public decimal Credit { get; init; }
}
