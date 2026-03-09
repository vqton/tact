namespace Web.Models.Ledger;

using Microsoft.AspNetCore.Mvc.Rendering;

public class LedgerFilterVm
{
    public int? AccountId { get; init; }

    public DateTime? FromDate { get; init; }

    public DateTime? ToDate { get; init; }

    public IReadOnlyList<SelectListItem> AccountOptions { get; init; } = [];

    public IReadOnlyList<LedgerRowVm> Rows { get; init; } = [];
}
