namespace AccountingVAS.Web.Models;

/// <summary>
/// Represents a transaction entry line within a JournalEntry.
/// Each TransactionEntry posts a debit or credit to an account.
/// Part of Aggregate Root: JournalEntry owns multiple TransactionEntries.
/// </summary>
public class TransactionEntry
{
    /// <summary>
    /// Unique identifier (primary key).
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Foreign key to parent JournalEntry.
    /// </summary>
    public int JournalEntryId { get; private set; }

    /// <summary>
    /// Navigation property to parent JournalEntry.
    /// </summary>
    public JournalEntry? JournalEntry { get; private set; }

    /// <summary>
    /// Foreign key to Account being posted to.
    /// </summary>
    public int AccountId { get; private set; }

    /// <summary>
    /// Navigation property to Account.
    /// </summary>
    public Account? Account { get; private set; }

    /// <summary>
    /// Debit amount in VND (decimal precision 18,2).
    /// At most one of Debit or Credit should be non-zero (per double-entry rule).
    /// </summary>
    public decimal Debit { get; private set; }

    /// <summary>
    /// Credit amount in VND (decimal precision 18,2).
    /// At most one of Debit or Credit should be non-zero (per double-entry rule).
    /// </summary>
    public decimal Credit { get; private set; }

    /// <summary>
    /// Free-text description of this line (optional, used for clarity).
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Timestamp when entry was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private TransactionEntry() { }

    /// <summary>
    /// Factory method to create a new TransactionEntry with validation.
    /// </summary>
    /// <param name="journalEntryId">Parent JournalEntry ID</param>
    /// <param name="accountId">Account ID to post to</param>
    /// <param name="debit">Debit amount (0 if credit entry)</param>
    /// <param name="credit">Credit amount (0 if debit entry)</param>
    /// <param name="description">Optional description</param>
    /// <returns>New TransactionEntry instance</returns>
    public static TransactionEntry Create(
        int journalEntryId,
        int accountId,
        decimal debit,
        decimal credit,
        string description = "")
    {
        if (journalEntryId <= 0)
            throw new ArgumentException("Journal entry ID must be positive", nameof(journalEntryId));

        if (accountId <= 0)
            throw new ArgumentException("Account ID must be positive", nameof(accountId));

        if (debit < 0 || credit < 0)
            throw new ArgumentException("Debit and credit amounts must be non-negative");

        if (debit == 0 && credit == 0)
            throw new ArgumentException("At least one of debit or credit must be non-zero");

        return new TransactionEntry
        {
            JournalEntryId = journalEntryId,
            AccountId = accountId,
            Debit = debit,
            Credit = credit,
            Description = description?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Gets the amount (debit or credit, whichever is non-zero).
    /// </summary>
    public decimal Amount => Debit != 0 ? Debit : Credit;

    /// <summary>
    /// Indicates if this is a debit entry.
    /// </summary>
    public bool IsDebit => Debit > 0;
}
