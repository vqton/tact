namespace AccountingVAS.Web.Models.ValueObjects;

/// <summary>
/// Represents a Chart of Accounts code (CoA code per VAS Circular 99).
/// Value Object: Immutable, no identity, compared by value.
/// Examples: 110, 211, 3121, 411, 511, etc.
/// </summary>
public sealed class AccountCode : IEquatable<AccountCode>
{
    /// <summary>
    /// The account code string (e.g., "110", "3121").
    /// </summary>
    public string Code { get; private init; }

    /// <summary>
    /// Private constructor for EF Core deserialization.
    /// </summary>
    private AccountCode()
    {
        Code = string.Empty;
    }

    /// <summary>
    /// Creates an AccountCode value object.
    /// </summary>
    /// <param name="code">Account code (1-5 characters, numeric)</param>
    public AccountCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Account code cannot be null or whitespace", nameof(code));
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{1,5}$"))
        {
            throw new ArgumentException("Account code must be 1-5 digits", nameof(code));
        }

        Code = code;
    }

    /// <summary>
    /// Returns the account code string.
    /// </summary>
    public override string ToString() => Code;

    /// <summary>
    /// Implicit conversion from string to AccountCode.
    /// </summary>
    public static implicit operator AccountCode(string code) => new(code);

    /// <summary>
    /// Implicit conversion from AccountCode to string.
    /// </summary>
    public static implicit operator string(AccountCode accountCode) => accountCode.Code;

    /// <summary>
    /// Equality comparison by Code value.
    /// </summary>
    public bool Equals(AccountCode? other) => other is not null && Code == other.Code;

    /// <summary>
    /// Equality comparison by Code value.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as AccountCode);

    /// <summary>
    /// Hash code based on Code value.
    /// </summary>
    public override int GetHashCode() => Code.GetHashCode();

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(AccountCode? left, AccountCode? right) =>
        (left is null && right is null) || (left is not null && left.Equals(right));

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(AccountCode? left, AccountCode? right) => !(left == right);
}
