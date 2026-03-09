namespace AccountingVAS.Web.Models;

/// <summary>
/// Represents an audit log entry for tracking changes to accounting data.
/// Required per VAS Circular 99 for accountability and audit trail compliance.
/// </summary>
public class AuditLogEntry
{
    /// <summary>
    /// Unique identifier (primary key).
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// User ID who performed the action (from ASP.NET Core Identity).
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    /// <summary>
    /// Timestamp when the action occurred (UTC).
    /// </summary>
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Type of entity modified (e.g., "Account", "JournalEntry", "TransactionEntry").
    /// </summary>
    public string EntityType { get; private set; } = string.Empty;

    /// <summary>
    /// Primary key ID of the modified entity (links to Account, JournalEntry, etc.).
    /// </summary>
    public int EntityId { get; private set; }

    /// <summary>
    /// Action performed: Create, Update, Delete, Post, Reverse, etc.
    /// </summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>
    /// JSON-serialized before/after changes (for detailed audit trail).
    /// Format: { "FieldName": { "Before": "old value", "After": "new value" }, ... }
    /// </summary>
    public string Changes { get; private set; } = string.Empty;

    /// <summary>
    /// Optional: IP address of the client making the request.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private AuditLogEntry() { }

    /// <summary>
    /// Factory method to create a new AuditLogEntry with validation.
    /// </summary>
    /// <param name="userId">User ID performing the action</param>
    /// <param name="entityType">Type of entity being modified</param>
    /// <param name="entityId">ID of the modified entity</param>
    /// <param name="action">Action being performed</param>
    /// <param name="changes">JSON-serialized changes (optional)</param>
    /// <param name="ipAddress">Client IP address (optional)</param>
    /// <returns>New AuditLogEntry instance</returns>
    public static AuditLogEntry Create(
        string userId,
        string entityType,
        int entityId,
        string action,
        string changes = "",
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID is required", nameof(userId));

        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required", nameof(entityType));

        if (entityId <= 0)
            throw new ArgumentException("Entity ID must be positive", nameof(entityId));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required", nameof(action));

        return new AuditLogEntry
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            EntityType = entityType.Trim(),
            EntityId = entityId,
            Action = action.Trim(),
            Changes = changes?.Trim() ?? string.Empty,
            IpAddress = ipAddress?.Trim()
        };
    }
}
