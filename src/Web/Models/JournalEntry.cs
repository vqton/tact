namespace AccountingVAS.Web.Models;

/// <summary>
/// Represents a Journal Entry (Chứng từ kế toán) - an accounting transaction.
/// Aggregate Root: Contains a collection of TransactionEntry lines.
/// Invariant: Total debits must equal total credits (double-entry bookkeeping).
/// </summary>
public class JournalEntry
{
    /// <summary>
    /// Unique identifier (primary key).
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Transaction date (when the economic event occurred).
    /// Format: yyyy-MM-dd per Vietnamese accounting standards.
    /// </summary>
    public DateTime Date { get; private set; }

    /// <summary>
    /// Transaction description (e.g., "Sale to customer X", "Monthly rent payment").
    /// Required for audit trail clarity.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Voucher/document reference (e.g., invoice number, cheque number).
    /// Used for traceability to supporting documents per VAS audit requirements.
    /// </summary>
    public string VoucherRef { get; private set; } = string.Empty;

    /// <summary>
    /// Entry status: Draft, Posted, Reversed, or Cancelled.
    /// Only Posted entries affect account balances.
    /// </summary>
    public JournalEntryStatus Status { get; private set; } = JournalEntryStatus.Draft;

    /// <summary>
    /// User ID of the entry creator (for audit trail).
    /// </summary>
    public string CreatedByUserId { get; private set; } = string.Empty;

    /// <summary>
    /// Timestamp when entry was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID of the entry modifier (for audit trail, nullable).
    /// </summary>
    public string? ModifiedByUserId { get; private set; }

    /// <summary>
    /// Timestamp when entry was last modified.
    /// </summary>
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection of transaction lines (debits and credits).
    /// Aggregate: JournalEntry owns these entries; deletion of parent cascades to children.
    /// </summary>
    public List<TransactionEntry> Entries { get; private set; } = [];

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private JournalEntry() { }

    /// <summary>
    /// Factory method to create a new JournalEntry with validation.
    /// </summary>
    /// <param name="date">Transaction date</param>
    /// <param name="description">Transaction description</param>
    /// <param name="voucherRef">Voucher/document reference</param>
    /// <param name="createdByUserId">User ID of creator</param>
    /// <returns>New JournalEntry instance in Draft status</returns>
    public static JournalEntry Create(
        DateTime date,
        string description,
        string voucherRef,
        string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException("Creator user ID is required", nameof(createdByUserId));

        return new JournalEntry
        {
            Date = date,
            Description = description.Trim(),
            VoucherRef = voucherRef?.Trim() ?? string.Empty,
            Status = JournalEntryStatus.Draft,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            Entries = []
        };
    }

    /// <summary>
    /// Adds a TransactionEntry line to this journal entry.
    /// Entry remains in Draft until posted (invariant validation happens at posting).
    /// </summary>
    public void AddEntry(TransactionEntry entry)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Cannot add entries to a non-draft journal entry");

        Entries.Add(entry);
    }

    /// <summary>
    /// Posts this journal entry (changes status to Posted).
    /// Validates double-entry invariant: Total debits == Total credits.
    /// </summary>
    /// <param name="postedByUserId">User ID of the poster (for audit trail)</param>
    public void Post(string postedByUserId)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException($"Cannot post a {Status} journal entry");

        if (Entries.Count == 0)
            throw new InvalidOperationException("Cannot post a journal entry with no transaction lines");

        // Validate double-entry invariant: Debits = Credits
        var totalDebit = Entries.Sum(e => e.Debit);
        var totalCredit = Entries.Sum(e => e.Credit);

        if (totalDebit != totalCredit)
        {
            throw new InvalidOperationException(
                $"Journal entry is unbalanced: Total debits ({totalDebit:N2}) != Total credits ({totalCredit:N2})");
        }

        Status = JournalEntryStatus.Posted;
        ModifiedByUserId = postedByUserId;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reverses a Posted journal entry (creates a mirror entry with opposite signs).
    /// Note: Actual reversal logic handled by service layer.
    /// </summary>
    /// <param name="reversedByUserId">User ID of the reversal requester</param>
    public void Reverse(string reversedByUserId)
    {
        if (Status != JournalEntryStatus.Posted)
            throw new InvalidOperationException($"Cannot reverse a {Status} journal entry");

        Status = JournalEntryStatus.Reversed;
        ModifiedByUserId = reversedByUserId;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets total debit amount across all entries.
    /// </summary>
    public decimal TotalDebit => Entries.Sum(e => e.Debit);

    /// <summary>
    /// Gets total credit amount across all entries.
    /// </summary>
    public decimal TotalCredit => Entries.Sum(e => e.Credit);

    /// <summary>
    /// Checks if entry is balanced (debits == credits).
    /// </summary>
    public bool IsBalanced => TotalDebit == TotalCredit;
}

/// <summary>
/// Journal Entry status enum per VAS accounting standards.
/// </summary>
public enum JournalEntryStatus
{
    /// <summary>In preparation, not yet posted</summary>
    Draft = 1,

    /// <summary>Posted and affects account balances</summary>
    Posted = 2,

    /// <summary>Reversed via reversal entry (original entry no longer affects balances)</summary>
    Reversed = 3,

    /// <summary>Cancelled (e.g., error correction)</summary>
    Cancelled = 4
}
