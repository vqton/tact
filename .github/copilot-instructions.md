# GitHub Copilot Instructions
**Effective:** March 2026 | **Project:** Accounting WebApp (VAS-Compliant, Pure MVC, .NET 9)

You are assisting with a Vietnamese accounting application built on ASP.NET Core MVC. Follow these rules strictly in all code generation, refactoring, and suggestions. When in doubt, prioritize **VAS Compliance** (Circular 99/2025/TT-BTC) and **Code Quality** rules first.

---

## 1. ARCHITECTURE
- **Framework**: .NET 9, pure ASP.NET Core MVC (no separate API, no SPA frameworks)
- **Pattern**: Traditional MVC monolith (Controllers → Services → DbContext)
- **Project Structure**:
  - `src/Models/` – Domain entities, DTOs, value objects
  - `src/Controllers/` – Thin HTTP request handlers
  - `src/Services/` – Business logic layer (handles DB access via DbContext)
  - `src/Data/` – EF Core DbContext + configurations + migrations
  - `src/Views/` – Razor templates organized by controller (Controllers/, Services/, etc.)
  - `src/wwwroot/` – Static assets, FastReport templates
  - `tests/` – Unit + Integration tests (xUnit)
- **Frontend**: Razor views + Bootstrap 5.3 + HTMX 2.x + Alpine.js (server-first, progressive enhancement)
- **Database**: Dual provider – SQLite (development), SQL Server Express (production)
- **Key principle**: Controllers dispatch to Services → Services access DbContext → return View with model

---

## 2. CODE QUALITY & SKILLS (CodeQualityAndSkills.md)
- **Language**: C# 13 / .NET 9 – use modern features aggressively (primary constructors, collection expressions, null checks)
- **Naming Conventions**:
  - Classes/Records/Interfaces: `PascalCase` (e.g., `JournalEntry`, `PostJournalEntryCommand`, `IJournalService`)
  - Methods/Properties: `PascalCase`
  - Local variables/parameters: `camelCase`
  - Private fields: `_camelCase`
  - Async methods: `MethodNameAsync` (e.g., `GetLedgerAsync`)
  - Commands/Queries: `[Noun][Verb]Command/Query` (e.g., `CreateAccountCommand`, `GetBalanceSheetQuery`)
  - DTOs: Suffix with `Dto`, `Vm`, or `Response`
  - Test methods: `[MethodUnderTest]_[Scenario]_[Expected]` (e.g., `PostJournalEntry_WithUnbalancedAmounts_ThrowsDomainException`)
- **Formatting**:
  - Indentation: 4 spaces (no tabs)
  - Max line length: 120 chars soft, 140 hard
  - Braces: Always use (even for single-line if/else)
  - String interpolation over `string.Format`
  - Use `var` when type is obvious; explicit type otherwise
  - Modern patterns: `is null`, `??`, `?.` nullish coalescing, null-forgiving `!` sparingly
- **Principles**: SOLID + DRY (but accept duplication over wrong abstraction in Vertical Slices)
- **Code must be self-documenting** – prefer meaningful names over comments
- **Async everywhere**: All I/O-bound operations use async/await (no `.Result`, `.Wait()`)
- **Testing**: Target 80%+ coverage (xUnit), Arrange-Act-Assert pattern, test behavior not implementation
- **Review Checklist**:
  - [ ] Naming + formatting rules followed
  - [ ] Business logic in Application/Domain, not controllers
  - [ ] Async all the way
  - [ ] No magic strings/numbers (use constants)
  - [ ] Tests cover happy + edge paths
  - [ ] Readable (one responsibility per method/class)
  - [ ] Secure + performant

---

## 3. DATABASE & EF CORE (DatabaseAndEFCore.md)
- **ORM**: Entity Framework Core 9.0 (async-first)
- **Providers**: SQLite (dev), SQL Server Express (prod)
- **Key Rules**:
  - ✅ Direct `DbContext` in Services – no Repository pattern
  - ✅ Async all the way: `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`
  - ✅ `AsNoTracking()` for read-only queries
  - ✅ Projections to DTOs (`.Select(...)`) for reports/ledger views
  - ✅ Use compiled queries (`EF.CompileAsyncQuery`) for hot paths
  - ✅ Fluent API configurations in `Data/Configurations/`
  - ❌ NO synchronous EF calls (`,ToList()`, `.Result`, `.Wait()`)
  - ❌ NO loading full entities for reports (project to DTOs)
  - ❌ NO missing indexes on filter/sort columns
  - ❌ NO lazy loading proxies
  - ❌ NO repositories
- **Entity Design**:
  - Rich domain models with invariants (not anemic)
  - Value Objects: `Money` (decimal Amount + string Currency = "VND"), `AccountCode`
  - Key entities: `Account`, `Journal`, `TransactionEntry`, `AuditLogEntry`
  - Use owned types for Money (18,2 precision)
- **Service Integration**:
  - Inject DbContext in service constructor
  - Query: `var acct = await _context.Accounts.FirstOrDefaultAsync(...)`
  - Update: `_context.Accounts.Update(account); await _context.SaveChangesAsync();`
- **Migrations**:
  - Code-first, seed VAS default Chart of Accounts (Appendix II)
  - Dual-provider compatibility (test both SQLite + SQL Server)
- **Testing**: Integration tests use SQLite in-memory + Respawn for reset

---

## 4. FRONTEND GUIDELINES (FrontendGuidelines.md)
- **Stack**: Razor Views + Bootstrap 5.3+ + HTMX 2.x + Alpine.js 3.x
- **Principles**:
  - Server-first, progressive enhancement
  - Desktop-first, responsive (1366×768+)
  - VAS/Vietnamese focus (Vietnamese labels, VND formatting: 1.234.567 ₫)
  - Accessibility: WCAG 2.1 AA minimum
  - Performance: <100 KB additional JS/CSS beyond frameworks; TTFB <200 ms
- **Colors**: Bootstrap palette (blue actions, green success, red danger, yellow warning)
- **Forms**:
  - Bootstrap form-floating or form-group + label
  - Required fields: asterisk + `required` class
  - Server-side validation (FluentValidation) + client hints (Bootstrap invalid-feedback)
  - Dynamic lines (journal entries): HTMX for partial submits
- **HTMX Patterns**:
  - `hx-post`/`hx-get` for form submits, filters, pagination
  - `hx-target="#target-id"` and `hx-swap="innerHTML|outerHTML"`
  - `hx-indicator=".htmx-indicator"` for spinner
- **Alpine.js**:
  - Small state only: `x-data="{ open: false, selected: null }"`
  - Modals: `x-show`, `x-transition`
  - Avoid complex logic (leave to server/HTMX)
- **Reports**: Card + table-responsive, sticky headers, zebra striping, totals row bolded
- **Prohibited**: Inline JavaScript, heavy jQuery, custom CSS overrides, full-page reloads when HTMX fits

---

## 5. REPORTING & FASTREPORT (ReportingAndFastReport.md)
- **Engine**: FastReport.OpenSource 2026.1.4 + PdfSimple plugin (free PDF export)
- **Approach**: Server-side rendering via WebReport in Razor views (no client-side report logic)
- **Template Location**: `wwwroot/Reports/` (e.g., `BalanceSheet.frx`, `IncomeStatement.frx`)
- **Integration**:
  ```csharp
  // Controller: thin, dispatch to MediatR
  var query = new GetBalanceSheetQuery { From, To };
  var data = await _mediator.Send(query);  // Returns DTO list
  
  var webReport = new WebReport { /* load .frx */ };
  webReport.Report.Load(reportPath);
  webReport.Report.RegisterData(data, "BalanceData");
  webReport.Report.SetParameterValue("ReportDate", DateTime.Today.ToString("dd/MM/yyyy"));
  
  return View("ReportPreview", webReport);
  ```
- **Key Features**:
  - VDS-compliant formats (per Circular 99 appendices)
  - Comparative periods support
  - VND formatting (1.234.567 ₫)
  - PDF export as primary output
  - Audit logging (user, timestamp, params)
- **Limitations**: No encryption, digital signatures, advanced font embedding in free version

---

## 6. VAS COMPLIANCE (VASCompliance.md)
**Legal Reference**: Circular 99/2025/TT-BTC (effective Jan 1, 2026)

### Core Rules (Always Enforce)
- **Double-entry invariant**: Debits = Credits per journal entry (domain guard + FluentValidation)
- **Functional currency**: Default & mandatory = **VND** only (no currency switching mid-year)
- **Fiscal year**: Jan–Dec (Vietnamese standard)
- **No prohibited negative balances** (e.g., assets, equity)

### Chart of Accounts (Appendix II – HIGHEST PRIORITY)
- **Mandatory new/key accounts** (seed in migrations):
  - `215` – Biological Assets (separate from `211` Tangible Fixed Assets)
  - `2295` – Provision for Impairment of Biological Assets
  - `332` – Dividends Payable / Profit Payable
  - `82112` – Additional Corporate Income Tax (GloBE 15% top-up)
  - `419` – Treasury shares (renamed)
- **Eliminated accounts** (BLOCK usage, validate in handlers):
  - `161`, `441`, `611`, `631`, `1562`
- **Level 1 preservation**: Allow customization below Level 2, preserve Level 1 substance for statutory reporting
- **Implementation**: Hierarchical entity (Code, Name, Type enum, Level, ParentId)

### Financial Statements
- **Formats**: Must follow Circular 99 structure
  - Balance Sheet: current/non-current sections
  - Income Statement: multi-step (revenue → COGS → gross profit → operating → net)
  - Cash Flow: indirect method default
- **Enhanced Notes**: Cash restrictions, biological assets changes, top-up tax, going concern, items >10% of totals

### Validators & Business Rules (FluentValidation)
- Balanced journal entries
- No forbidden accounts in transactions
- Revenue recognition at performance obligation fulfillment
- Impairment/provisions with reliable evidence

### Audit Trail
- Log all changes: user, timestamp, entity, action, before/after
- Use Serilog structured logging (mask sensitive data)

### Prohibited Practices (Reject or Flag)
- ❌ Using eliminated accounts
- ❌ Hard-coded outdated CoA
- ❌ Ignoring VND as default currency
- ❌ Unbalanced entries (throw `DomainException`)
- ❌ Reports missing comparative figures or Notes disclosures
- ❌ Missing audit logging

---

## 7. SECURITY (Always Enforce)
- HTTPS enforcement: `UseHsts()`, `UseHttpsRedirection()`
- Secure cookies: `HttpOnly`, `Secure`, `SameSite=Strict`
- Input validation: `FluentValidation` + `[ValidateAntiForgeryToken]`
- Authorization: Role-based + policy-based (Accountant, Auditor, Viewer)
- Never log sensitive data (mask account #, amounts in Serilog enrichers)
- Parameterized queries: EF Core does this by default

---

## 8. REVIEW CHECKLIST (For PRs / Code Generation)
- [ ] Naming & formatting rules followed
- [ ] Business logic in Services, not controllers
- [ ] Controllers call services, services access DbContext (no MediatR)
- [ ] Async all the way (no `.Result`, `.Wait()`, `.ToList()`)
- [ ] Validated against VAS Compliance (no forbidden accounts, balanced entries)
- [ ] Tests cover happy + edge paths
- [ ] No magic strings/numbers
- [ ] Readable (one responsibility per method/class)
- [ ] Secure + performant
- [ ] Reports VAS-compliant (correct structure, comparative figures, Notes)
- [ ] Database/EF changes dual-provider tested (SQLite + SQL Server)

---

## 9. QUICK START (What to do when generating code)

**For Controllers:**
- Keep thin: Inject service, call method, return View with model data
- Example: `var result = await _journalService.PostJournalAsync(dto); return View(result);`
- No business logic; HTTP handling only

**For Services:**
- Inject DbContext (scoped lifetime)
- All business logic: validation, transactions, domain rules
- Access database via `_context.Entities.Where(...).ToListAsync()`
- Example methods: `GetAccountAsync()`, `PostJournalAsync()`, `CalculateTrialBalanceAsync()`
- Throw domain exceptions for invariant violations

**For Models:**
- Rich entities with validation methods
- Use value objects (Money, AccountCode)
- Enforce VAS rules (no forbidden accounts, balanced entries)

**For Razor Views:**
- Bootstrap classes + HTMX attributes + Alpine directives
- Form validation feedback (Bootstrap invalid-feedback)
- VND formatting for amounts
- No inline JavaScript
- Pass simple entities or DTOs directly from controller

**For Reports:**
- Service method aggregates data to DTOs
- Controller calls service, creates WebReport, registers data
- View renders WebReport preview + export buttons
- Support PDF export via PdfSimple plugin
- VAS-compliant layout

---

## 10. WHEN IN DOUBT
1. **Check VASCompliance.md first** – accounting rules override everything
2. **Then CodeQualityAndSkills.md** – code standards
3. **Then architecture file** – structure & patterns
4. **Then domain-specific** (DB, Frontend, Reporting)

---

**Last Updated:** March 9, 2026

