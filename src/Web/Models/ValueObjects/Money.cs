namespace AccountingVAS.Web.Models.ValueObjects;

/// <summary>
/// Represents a monetary amount in Vietnamese Dong (VND).
/// Value Object: Immutable, no identity, compared by value.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// Monetary amount with 2 decimal places precision.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Currency code (default and mandatory: VND per VAS Circular 99).
    /// </summary>
    public string Currency { get; init; } = "VND";

    /// <summary>
    /// Private constructor for EF Core deserialization.
    /// </summary>
    private Money() { }

    /// <summary>
    /// Creates a Money value object.
    /// </summary>
    /// <param name="amount">Numeric amount (decimals are currency minor units)</param>
    /// <param name="currency">Currency code (default VND)</param>
    public Money(decimal amount, string currency = "VND")
    {
        if (currency != "VND")
        {
            throw new ArgumentException("Only VND currency is permitted per VAS Circular 99", nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Formats money as Vietnamese VND string (e.g., "1.234.567 ₫").
    /// </summary>
    public override string ToString() =>
        $"{Amount:N0} ₫";

    /// <summary>
    /// Implicit conversion from decimal to Money (VND).
    /// </summary>
    public static implicit operator Money(decimal amount) => new(amount);

    /// <summary>
    /// Implicit conversion from Money to decimal.
    /// </summary>
    public static implicit operator decimal(Money money) => money.Amount;
}
