namespace AccountingVAS.Web.Data;

using Microsoft.EntityFrameworkCore;
using AccountingVAS.Web.Data.Configurations;
using AccountingVAS.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Web.Models.Identity;

/// <summary>
/// Entity Framework Core DbContext for the accounting application.
/// Supports dual database providers: SQLite (development), SQL Server (production).
/// All accounting data flows through this context: Accounts, JournalEntries, TransactionEntries, AuditLogs.
///
/// Usage:
///   - Inject as scoped service: services.AddDbContext{AccountingDbContext}(options => ...)
///   - Query: var accounts = await _context.Accounts.ToListAsync();
///   - Update: _context.Accounts.Update(account); await _context.SaveChangesAsync();
///   - All operations are async by design (no .Result, .Wait(), or .ToList())
/// </summary>
public class AccountingDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// DbSet for Chart of Accounts entities.
    /// </summary>
    public DbSet<Account> Accounts { get; set; } = null!;

    /// <summary>
    /// DbSet for Journal Entry headers (aggregates).
    /// </summary>
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;

    /// <summary>
    /// DbSet for individual transaction lines (owned by JournalEntry).
    /// </summary>
    public DbSet<TransactionEntry> TransactionEntries { get; set; } = null!;

    /// <summary>
    /// DbSet for audit log entries (VAS Circular 99 compliance).
    /// </summary>
    public DbSet<AuditLogEntry> AuditLogs { get; set; } = null!;

    /// <summary>
    /// Initializes AccountingDbContext with default options.
    /// </summary>
    public AccountingDbContext() { }

    /// <summary>
    /// Initializes AccountingDbContext with provided DbContextOptions.
    /// </summary>
    public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
        : base(options) { }

    /// <summary>
    /// Configures the DbContext model, entity mappings, and relationships.
    /// All Fluent API configurations applied via IEntityTypeConfiguration classes.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Fluent API configurations from IEntityTypeConfiguration classes
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new JournalEntryConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionEntryConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogEntryConfiguration());
    }

    /// <summary>
    /// Safely executes a query or command with async/await pattern.
    /// All database operations must be async to avoid blocking.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
