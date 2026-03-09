namespace AccountingVAS.Web.Models;

using AccountingVAS.Web.Models.ValueObjects;

/// <summary>
/// Represents an Account in the Chart of Accounts (CoA per VAS Circular 99).
/// Domain entity with invariants: Code uniqueness, Type validity, Level structure.
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier (primary key).
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Account code (1-5 digit string, e.g., "110", "3121"). Must be unique.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Account name/description in Vietnamese.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Account type enum: Asset, Liability, Equity, Revenue, Expense.
    /// Enforces VAS Chart of Accounts structure.
    /// </summary>
    public AccountType Type { get; private set; }

    /// <summary>
    /// Parent account ID for hierarchical CoA (null if Level 1).
    /// </summary>
    public int? ParentId { get; private set; }

    /// <summary>
    /// Navigation property to parent account.
    /// </summary>
    public Account? Parent { get; private set; }

    /// <summary>
    /// Hierarchical level: 1-5 per VAS structure.
    /// Level 1 = Class (1-5), Level 2 = Subclass (11-59), etc.
    /// </summary>
    public int Level { get; private set; } = 1;

    /// <summary>
    /// Indicates if account is actively used in transactions.
    /// Inactive accounts cannot be used for new entries per VAS rules.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Current debit balance for this account (updated via JournalEntry postings).
    /// Expressed as decimal in VND.
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Timestamp when account was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when account was last modified.
    /// </summary>
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation: TransactionEntries posted to this account.
    /// </summary>
    public List<TransactionEntry> TransactionEntries { get; private set; } = [];

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Account() { }

    /// <summary>
    /// Factory method to create a new Account with validation.
    /// </summary>
    /// <param name="code">Account code (1-5 digits)</param>
    /// <param name="name">Account name in Vietnamese</param>
    /// <param name="type">Account type enum</param>
    /// <param name="parentId">Parent account ID (null for Level 1)</param>
    /// <param name="level">Hierarchical level (1-5)</param>
    /// <returns>New Account instance</returns>
    public static Account Create(
        string code,
        string name,
        AccountType type,
        int? parentId = null,
        int level = 1)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Account code is required", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name is required", nameof(name));

        if (level < 1 || level > 5)
            throw new ArgumentException("Account level must be between 1 and 5", nameof(level));

        return new Account
        {
            Code = code.Trim(),
            Name = name.Trim(),
            Type = type,
            ParentId = parentId,
            Level = level,
            IsActive = true,
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Deactivates an account (prevents future transactions).
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates a deactivated account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates account name (audit trail via AuditLogEntry).
    /// </summary>
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Account name cannot be empty", nameof(newName));

        Name = newName.Trim();
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates editable account fields for Chart of Accounts maintenance.
    /// </summary>
    public void UpdateDetails(string code, string name, AccountType type, int? parentId, int level)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Account code is required", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name is required", nameof(name));

        if (level < 1 || level > 5)
            throw new ArgumentException("Account level must be between 1 and 5", nameof(level));

        Code = code.Trim();
        Name = name.Trim();
        Type = type;
        ParentId = parentId;
        Level = level;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adjusts balance when a TransactionEntry is posted/reversed.
    /// Internal method called by JournalEntry posting logic.
    /// </summary>
    internal void AdjustBalance(decimal debit, decimal credit)
    {
        Balance += debit - credit;
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Account types per VAS Circular 99 Chart of Accounts (Level 1 classes).
/// </summary>
public enum AccountType
{
    /// <summary>Class 1: Assets</summary>
    Asset = 1,

    /// <summary>Class 2: Liabilities</summary>
    Liability = 2,

    /// <summary>Class 3: Equity</summary>
    Equity = 3,

    /// <summary>Class 4: Revenue</summary>
    Revenue = 4,

    /// <summary>Class 5: Expense</summary>
    Expense = 5
}
