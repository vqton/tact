# AccountingVAS - Development Status

**Project:** Circular 99/2025/TT-BTC Compliant Accounting Ledger  
**Target:** .NET 9 (STS until Nov 2026)  
**Status:** In Development (6 of 14 tasks complete)  
**Last Updated:** March 9, 2026

---

## Task Completion Checklist

### ✅ Completed Tasks

- [x] **Task 1: Project Setup** *(3/9/2026)*
  - ✓ global.json locked to .NET SDK 9.0.308
  - ✓ Directory.Build.props with shared metadata
  - ✓ .editorconfig with SOLID/clean code rules
  - ✓ All .csproj files configured (Core, Application, Infrastructure, Web, Tests)
  - ✓ NuGet packages aligned (MediatR, FluentValidation, Mapster, Serilog, EF Core, FastReport)
  - ✓ Build succeeds with 0 errors, 2 benign warnings

- [x] **Task 2: Domain Entities + Value Objects** *(3/9/2026)*
  - ✓ Core layer: Money, AccountCode value objects
  - ✓ Core layer: Account, JournalEntry, TransactionEntry, AuditLogEntry entities
  - ✓ ApplicationDbContext with 4 DbSet (Accounts, JournalEntries, TransactionEntries, AuditLogs)
  - ✓ Entity configurations (Fluent API, conversions, relationships)
  - ✓ ApplicationUser (enhanced with Department, IsActive, LastLoginAt)
  - ✓ VasConstants (AccountTypes, NewAccountCodes, JournalEntryStatuses)
  - ✓ All entities use factory methods + immutability patterns
  - ✓ Build succeeds, nullable warnings fixed

- [x] **Task 3: Dual DB Configuration + Migration + Seeding** *(3/9/2026)*
  - ✓ appsettings.json: SQL Server as default (production)
  - ✓ appsettings.Development.json: SQLite (dev, accounting_vas.db)
  - ✓ Program.cs: Dynamic provider selection based on config
  - ✓ Program.cs: Async DB initialization (MigrateAsync + seeding)
  - ✓ EF Core migration "InitialCreate" created (20260309030856)
  - ✓ Migration includes all tables: Accounts, JournalEntries, TransactionEntries, AuditLogs, Identity
  - ✓ VasChartOfAccountsSeeder with 13 default accounts (Classes 1-5)
  - ✓ Build succeeds, ready for first application run

- [x] **Task 4: ASP.NET Core Identity Setup** *(3/9/2026)*
  - ✓ ApplicationIdentitySeeder: Creates 4 roles (Admin, Accountant, Auditor, Viewer)
  - ✓ ApplicationIdentitySeeder: Seeds admin user (admin@accounting-vas.local / AdminPassword@1234)
  - ✓ Integrated seeding in Program.cs async initialization
  - ✓ AccountController: Login action (GET/POST) with remember-me, lockout protection
  - ✓ AccountController: Logout action (signed-out redirect)
  - ✓ AccountController: Register action (new users default to Viewer role)
  - ✓ AccountController: AccessDenied action
  - ✓ Login.cshtml: Centered card layout with error handling, test credentials shown
  - ✓ Register.cshtml: Full registration form (email, full name, department, password confirmation)
  - ✓ AccessDenied.cshtml: User-friendly error page
  - ✓ Cookie configuration: 8-hour sliding expiration, HttpOnly, Secure, Strict SameSite
  - ✓ Password policy enforced: 8+ chars, digit, special, upper+lower
  - ✓ Build succeeds with 0 errors

- [x] **Task 5: MediatR + FluentValidation Setup** *(3/9/2026)*
  - ✓ **Queries:** GetAccountsQuery, GetAccountByIdQuery, GetAccountByCodeQuery, GetAccountHierarchyQuery
  - ✓ **Handlers:** Full async handlers for all queries with logging + Mapster mapping
  - ✓ **Commands:** CreateAccountCommand, UpdateAccountCommand, DeleteAccountCommand, ActivateAccountCommand
  - ✓ **Handlers:** Full async handlers for all commands with business rule validation
  - ✓ **Validators:** FluentValidation rules for Create/Update/Delete/Activate (VAS code format 4-digit, class 1-9, name max 200 chars)
  - ✓ **ValidationBehavior:** MediatR pipeline behavior for automatic validation before handlers execute
  - ✓ **DTOs:** AccountDto, CreateAccountRequest, UpdateAccountRequest for API contracts
  - ✓ **Mapster Config:** AccountMappingConfig with IRegister for entity ↔ DTO mapping
  - ✓ **Architecture:** Application layer references Infrastructure (DbContext injection), removed circular dependency (Infrastructure no longer references Application)
  - ✓ **Domain Enhancement:** Added Account.UpdateInfo() method to enforce immutability pattern
  - ✓ **Handlers Support:** Full CRUD operations respecting domain constraints (no property assignments, use factory/command methods)
  - ✓ **Logging:** Structured logging in all handlers (ILogger<T> injected, loglevel Info/Warning/Error per domain event)
  - ✓ **Error Handling:** Domain exceptions for business rule violations (duplicate codes, missing parents, circular hierarchies, orphan accounts)
  - ✓ **Build:** 0 errors, only non-critical NuGet conflict warning

- [x] **Task 6: Serilog Structured Logging Setup** *(3/9/2026)*
  - ✓ **LoggingBehavior:** MediatR pipeline behavior measuring request execution time (logs warnings if >500ms)
  - ✓ **ILogContextEnricher:** Service to enrich logs with UserId, UserEmail, Department from ClaimsPrincipal
  - ✓ **LogContextEnrichmentMiddleware:** HTTP pipeline middleware setting RequestPath, RequestMethod, RemoteIP context
  - ✓ **appsettings.json:** Production config with Console, File (daily rolling), and Seq sinks with structured output templates
  - ✓ **appsettings.Development.json:** Debug-level logging with console colors + daily rolling file with detailed templates
  - ✓ **Enrichment Properties:** Automatic context properties (UserId, Department, RequestPath, RequestMethod, RemoteIP, ThreadId, MachineName)
  - ✓ **Account Controller:** Structured logging for Login/Register/Logout with user email, department, error details
  - ✓ **Manual Instrumentation:** Stopwatch + LogWarning for long-running requests (>500ms)
  - ✓ **Configuration:** log levels per namespace (Microsoft=Warning, EntityFrameworkCore=Warning/Debug per env, AccountingVAS=Info/Debug)
  - ✓ **Output Templates:** Include timestamp, level, source context, user ID, department, request path, exception details
  - ✓ **Build:** 0 errors, ready for logging production traces

### ⏳ Pending Tasks

- [ ] **Task 7: First Vertical Slice - Chart of Accounts**
  - [ ] List accounts (hierarchical view)
  - [ ] Create account form
  - [ ] Edit account
  
- [ ] **Task 8: Second Vertical Slice - Transactions/Journal Entry**
  - [ ] Double-entry form with HTMX dynamic lines
  - [ ] Alpine.js for client-side interactions
  - [ ] VAS balance validation
  
- [ ] **Task 9: Ledger View + Trial Balance Report**

- [ ] **Task 10: Financial Reports (FastReport Integration)**
  - [ ] Balance Sheet
  - [ ] Income Statement
  - [ ] Cash Flow
  - [ ] PDF export via PdfSimple

- [ ] **Task 11: Audit Trail View**

- [ ] **Task 12: Dashboard + Navigation**

- [ ] **Task 13: Integration Tests Setup (Respawn + SQLite)**

- [ ] **Task 14: Polish + i18n (Vietnamese)**

---

## Architecture Decisions Made

### Clean Architecture Hybrid + Vertical Slices
- **Core:** Pure domain entities, value objects, exceptions
- **Application:** MediatR handlers/validators, DTOs, service interfaces (no DbContext dependency)
- **Infrastructure:** EF Core DbContext, configurations, migrations, Identity, Serilog setup
- **Web:** MVC Controllers (NOT Minimal APIs/Blazor), Razor views, static assets
- **Tests:** xUnit + Respawn, integration tests only (no unit test framework overkill)

### Value Objects
- **Money:** Immutable, currency-aware, arithmetic operations, decimal precision
- **AccountCode:** 4-digit VAS format (e.g., "1010"), hierarchy methods, validation

### Entity Relationships
- **Account → Account:** Self-referencing hierarchy (ParentAccount)
- **JournalEntry ← TransactionEntry:** 1:Many, cascade delete
- **TransactionEntry → Account:** Many:1, restrict delete (prevent orphaning accounts)
- **JournalEntry:** Immutable once posted (Status field prevents modification)

### Database Strategy
- **Development:** SQLite (file-based, no setup, .db in root)
- **Production:** SQL Server Express (docker or local instance)
- **Provider Switch:** Config-driven (`Database:Provider` in appsettings)
- **Migrations:** Infrastructure assembly, tracks all schema changes

### VAS Compliance (Circular 99/2025/TT-BTC)
- Default currency: VND (no multi-currency initially)
- 4-digit account codes per Appendix II
- 13 seed accounts cover Classes 1-5 (Assets, Liabilities, Equity, Revenue, Expenses)
- Enhanced notes/descriptions in all accounts
- Debit=Credit invariant enforced in JournalEntry.Post()
- Audit trail immutable (AuditLogEntry never deleted)

---

## Key Code Patterns Established

### Factory Methods in Entities
```csharp
public static Account Create(AccountCode code, string name, string @class, ...)
public static TransactionEntry CreateDebit(Guid accountId, Money amount, ...)
```
**Benefit:** Prevents invalid state construction, centralizes rules

### Value Object Conversions in EF
```csharp
builder.Property(a => a.Code)
    .HasConversion(
        v => v.Code,
        v => new AccountCode(v))
```
**Benefit:** Database stores primitives, code works with domain types

### Async Database Initialization
```csharp
await dbContext.Database.MigrateAsync();
await VasChartOfAccountsSeeder.SeedAsync(dbContext);
```
**Benefit:** Non-blocking startup, proper async/await flow

---

## Known Issues / Tech Debt

### Minor
1. **EF Tools Version:** "EF tools 8.0.10 older than runtime 9.0.0" — warning only, doesn't affect build
2. **FastReport Package Warning:** NU1608 (CodeAnalysis.VisualBasic mismatch) — benign, no runtime impact
3. **Serilog.Formatting.Compact:** Used version 3.0.0 (3.0.1 doesn't exist in NuGet)

### None Critical
- No blocker issues
- Build succeeds with 0 errors
- All entities compile and configure correctly

---

## Authentication & Authorization (Planned - Task 4)

### Identity Roles (4 tiers per VAS usage)
| Role | Permissions | Notes |
|------|-------------|-------|
| **Admin** | All + user management | Create/edit/delete accounts, post/reverse entries, manage users |
| **Accountant** | Post entries + CoA mgmt | Create/edit journal entries, view reports, cannot delete |
| **Auditor** | Read-only + audit trail | View all, cannot modify, audit trail access |
| **Viewer** | Read-only reports | Dashboard + reports only, no details |

### Authentication Flow
- Cookie-based (ASP.NET Core Identity)
- 8-hour sliding expiration
- Strong password: 8+ chars, digit, special, upper+lower
- 5 failed attempts → 15 min lockout
- HTTPS + secure cookies enforced

---

## Database Schema Snapshot

### Tables Created (InitialCreate migration)
1. **Accounts** — General ledger accounts with hierarchy, balance
2. **JournalEntries** — Transaction headers (reference, date, status)
3. **TransactionEntries** — Debit/credit lines (amount, account, is_debit)
4. **AuditLogs** — Change tracking (entity type, action, old/new values, user, timestamp)
5. **AspNetUsers** — Identity users (enhanced with app-specific fields)
6. **AspNetRoles** — Identity roles (Admin, Accountant, Auditor, Viewer)
7. **AspNetUserRoles** — User-role mappings
8. *(Other Identity tables: Claims, Logins, Tokens)*

### Key Indexes
- Accounts.Code (unique)
- JournalEntries.ReferenceNumber (unique)
- AuditLogs (EntityType, EntityId, ChangedAt)

---

## Build Status

| Project | Status | Notes |
|---------|--------|-------|
| Core | ✅ Success | 0 errors, clean domain logic |
| Application | ✅ Success | Handlers/validators ready, empty pending Task 5 |
| Infrastructure | ✅ Success | DbContext + migrations established |
| Web | ✅ Success | Program.cs wired, ready for identity Task 4 |
| Tests | ✅ Success | xUnit scaffold, Respawn imported, no tests yet |

**Overall:** Build succeeds in 8-10 seconds, 0 errors, 2 warnings (non-blocking)

---

## Next Steps (Task 7)

1. **Create ChartOfAccounts vertical slice** (first feature)
2. **Implement Account controller** actions (List, Create, Edit)
3. **Create views** (Index, Create, Edit with Bootstrap)
4. **Add HTMX for dynamic interactions** (optional: inline edit)
5. **Test CRUD flow** (create, update, view hierarchy)

**Estimated Time:** 3-4 hours

---

## References

- **Circular 99/2025/TT-BTC:** VAS Accounting Standards (Vietnam)
- **.NET 9:** https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9
- **EF Core 9:** https://learn.microsoft.com/en-us/ef/core/
- **Clean Architecture:** Uncle Bob's principles
- **MediatR:** CQRS pattern for command/query separation
- **Serilog:** Structured logging best practices
