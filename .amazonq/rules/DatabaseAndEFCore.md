# Database and EF Core Rules – Accounting WebApp Project
**Version:** 1.0 | **Effective Date:** March 2026  
**ORM:** Entity Framework Core 9.0 (released Nov 2024, STS support until Nov 2026)  
**Databases:** 
- Development / small installs: SQLite (file-based, zero setup)
- Production: SQL Server Express (latest edition, e.g., 2022/2025)  
**Primary Goal:** Ensure reliable, performant, auditable persistence with VAS compliance (double-entry invariants, hierarchical CoA, VND precision, audit logging). Use EF Core directly in Application layer (Vertical Slice style) for simplicity and velocity.

## 1. Core EF Core Principles (2026 Best Practices)
- **Target .NET 9** via global.json (EF Core 9 targets .NET 8 but works perfectly on .NET 9).
- **Direct DbContext usage in Services** (no Repository pattern): Services inject DbContext (scoped) and query directly.
- **Async all the way**: Use `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()` everywhere.
- **No tracking for reads**: `AsNoTracking()` on queries unless change tracking required.
- **Projections & Select**: Use `.Select()` to DTOs for reports/ledger views – avoid loading full entities unnecessarily.
- **Compiled queries** (EF Core 9+ improvements): Use `EF.CompileAsyncQuery` for hot paths (e.g., trial balance aggregation).
- **Migrations**: Code-first, apply via `dotnet ef migrations add` and `dotnet ef database update` (or in Startup/Program for production seeding).

## 2. DbContext Configuration (Data/AccountingDbContext.cs)
- Location: `src/Data/AccountingDbContext.cs`
- **Dual-provider switch** (SQLite dev ↔ SQL Server prod):
  - Use `IConfiguration` or `IOptions` to read connection string.
  - In `OnConfiguring` (for design-time) or via factory in Program.cs.
  - Example:
    ```csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            if (IsDevelopment)
                optionsBuilder.UseSqlite(connStr);
            else
                optionsBuilder.UseSqlServer(connStr);
        }
    }
    ```
- **Global settings**:
  - `UseLazyLoadingProxies()` → avoid (performance hit; prefer eager/explicit loading).
  - `ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;` (global read-only).
  - Enable detailed errors in dev only.
- **Injection**: Register in Program.cs as scoped service → injected into service constructors.
- **Usage in services**: `var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);`

## 3. Entity Design & VAS Compliance
- **Entities** (src/Core/Entities/):
  - Rich models with invariants (e.g., TransactionEntry: ensure Debit/Credit balance in constructor).
  - Value Objects: Money (decimal Amount + string Currency = "VND"), AccountCode (string with validation).
  - Key entities:
    - Account: Id, Code (string, e.g. "111.01"), Name, Type (enum), ParentId (self-ref for hierarchy), Level
    - Journal: Id, Date, Description, VoucherRef, Entries (List<TransactionEntry>)
    - TransactionEntry: Id, JournalId, AccountId, Debit (Money), Credit (Money), Description
    - AuditLogEntry: Id, UserId, Timestamp, EntityType, EntityId, Action, Changes (JSON)
- **Configurations** (src/Infrastructure/Persistence/Configurations/):
  - Use IEntityTypeConfiguration<T> for fluent API.
  - Indexes: On Account.Code (unique), Journal.Date, TransactionEntry.AccountId.
  - Owned types: Money as owned entity (Amount decimal(18,2), Currency nvarchar(3)).
  - Precision: decimal(18,2) for amounts (VND no decimals, but safe).
  - Relationships: Cascade delete disabled on critical refs (e.g., Journal → Entries).

## 4. Service Layer Integration with DbContext
- In `src/Services/{ServiceName}.cs`:
  - Query methods: `var accounts = await _context.Accounts.Where(...).Select(dto => new ...).ToListAsync();`
  - Command methods: `_context.Journals.Add(journal); await _context.SaveChangesAsync();`
  - Inject `DbContext` via constructor (scoped lifetime auto-managed by DI container).
  - Example service:
    ```csharp
    public class JournalService
    {
        private readonly AccountingDbContext _context;

        public JournalService(AccountingDbContext context)
        {
            _context = context;
        }

        public async Task<JournalDto> GetJournalAsync(int id)
        {
            return await _context.Journals
                .AsNoTracking()
                .Where(j => j.Id == id)
                .Select(j => new JournalDto { ... })
                .FirstOrDefaultAsync();
        }

        public async Task PostJournalAsync(CreateJournalCommand cmd)
        {
            var journal = new Journal { ... };
            _context.Journals.Add(journal);
            await _context.SaveChangesAsync();
        }
    }
    ```
- No repositories – DbSet<T> is the repository.
- For complex queries (reports): Create extension methods on DbContext or custom methods in service.

## 5. Migrations & Seeding
- **Initial migration**: Seed VAS default CoA (Appendix II of Circular 99) in `OnModelCreating` or separate seeder class.
  - Example: Insert accounts like 111 (Cash), 215 (Biological Assets), 82112 (GloBE top-up).
- **Seeding strategy**: Use `HasData()` in configurations for static data; custom seeder for sample transactions.
- **Production**: Apply migrations at startup (carefully – use idempotent scripts or Flyway-like tools if needed).
- **SQLite → SQL Server switch**: Test both providers; ensure migrations compatible (avoid SQLite-specific features).

## 6. Performance Rules for Accounting Workloads
- Index heavily on Date, AccountId, JournalId for ledger/trial balance queries.
- Use split queries (`AsSplitQuery()`) for large includes (e.g., Journal with Entries).
- Paginate results (ledger views) – take/skip with ordering.
- Compiled queries for frequent aggregations (e.g., balance by account).
- Avoid N+1: Use `.Include()` judiciously or project to DTOs.
- Batch SaveChanges for bulk postings (EF Core 9+ bulk improvements help).

## 7. Testing Rules
- **Unit**: Mock DbContext (use Moq or InMemory provider).
- **Integration**: Use SQLite in-memory or real file + Respawn for reset between tests.
- Test VAS invariants (unbalanced entry throws), hierarchy queries, report aggregations.

## 8. Prohibited / High-Risk Practices
- Synchronous EF calls (`ToList()` instead of `ToListAsync()`).
- Loading full entities for reports (use projections).
- Raw SQL unless necessary (EF Core 9 query pipeline is fast enough).
- Tracking on read-heavy queries.
- Missing indexes on filter/sort columns.

## 9. Review Checklist for DB/EF Code
- [ ] Async EF methods used
- [ ] Projections to DTOs for reads
- [ ] Indexes defined for performance
- [ ] VAS invariants in entities/configs
- [ ] Dual-provider tested (SQLite + SQL Server)
- [ ] Migrations/seeding VAS CoA

**Enforcement:**  
- Amazon Q: Apply these rules + VASCompliance.md when generating DbContext, entities, handlers, or migrations.  
- Violations: Add // DB-EF-FIX comment or block PR.

Last updated: March 09, 2026 – Software Architect