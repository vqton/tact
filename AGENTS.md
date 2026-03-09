# AGENTS.md
**Effective:** March 2026 | **Project:** Accounting WebApp (VAS-Compliant, Pure MVC, .NET 9)

This file defines agent behavior for this repository. Follow these rules strictly in all code generation, refactoring, reviews, and implementation tasks. If conflicts arise, prioritize **VAS Compliance** (Circular 99/2025/TT-BTC) and **Code Quality** first.

---

## 1. Architecture
- Framework: .NET 9, pure ASP.NET Core MVC (no separate API, no SPA frameworks)
- Pattern: Traditional MVC monolith (Controllers -> Services -> DbContext)
- Project structure:
  - `src/Models/`: Domain entities, DTOs, value objects
  - `src/Controllers/`: Thin HTTP request handlers
  - `src/Services/`: Business logic layer (handles DB via DbContext)
  - `src/Data/`: EF Core DbContext + configurations + migrations
  - `src/Views/`: Razor templates organized by controller
  - `src/wwwroot/`: Static assets, FastReport templates (`*.frx`)
  - `tests/`: Unit + Integration tests (xUnit)
- Frontend: Razor views + Bootstrap 5.3+ + HTMX 2.x + Alpine.js 3.x (server-first, progressive enhancement)
- Database: Triple provider - SQLite (development), SQL Server Express (staging/small production), SQL Server full (large production)
- Key principle: Controllers dispatch to Services -> Services access DbContext -> return View with model

---

## 2. Code Quality & Skills
- Language: C# 13 / .NET 9; use modern features where helpful (primary constructors, collection expressions, null checks)
- Naming conventions:
  - Classes/Records/Interfaces: `PascalCase` (e.g., `JournalEntry`, `IJournalService`)
  - Methods/Properties: `PascalCase`
  - Local variables/parameters: `camelCase`
  - Private fields: `_camelCase`
  - Async methods: `MethodNameAsync`
  - DTOs: Suffix with `Dto`, `Vm`, or `Response`
  - Test methods: `[MethodUnderTest]_[Scenario]_[Expected]`
- Formatting:
  - Indentation: 4 spaces (no tabs)
  - Max line length: 120 chars soft, 140 hard
  - Braces: Always use (even for single-line)
  - Prefer string interpolation over `string.Format`
  - Use `var` when type obvious; explicit otherwise
  - Use modern null patterns (`is null`, `??`, `?.`); use null-forgiving `!` sparingly
- Principles: SOLID + DRY (accept duplication over wrong abstraction)
- Prefer self-documenting code over comments
- Async everywhere: all I/O-bound operations use async/await (no `.Result`, `.Wait()`)
- Testing: target 80%+ coverage (xUnit), Arrange-Act-Assert
- Review checklist:
  - [ ] Naming & formatting rules followed
  - [ ] Business logic in Services, not controllers
  - [ ] Async all the way
  - [ ] Validated against VAS compliance
  - [ ] Tests cover happy + edge paths
  - [ ] No magic strings/numbers
  - [ ] Readable (one responsibility per method/class)
  - [ ] Secure + performant

---

## 3. Database & EF Core
- ORM: Entity Framework Core 9.0 (async-first)
- Providers: Triple - SQLite (dev), SQL Server Express (staging/small prod), SQL Server full (large prod)
- Rules:
  - Use direct `DbContext` in Services (no Repository pattern)
  - Async all the way: `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`
  - Use `AsNoTracking()` for read-only queries
  - Use projections to DTOs for reports/ledger views
  - Use compiled queries for hot paths
  - Use Fluent API configurations in `Data/Configurations/`
  - No synchronous EF calls
  - Do not load full entities for report-only queries
  - Ensure indexes exist on filter/sort columns
- Entity design:
  - Rich domain models with invariants
  - Value objects: `Money` (decimal Amount + string Currency = `"VND"`), `AccountCode`
  - Key entities: `Account`, `JournalEntry`, `TransactionEntry`, `AuditLogEntry`
- Migrations:
  - Code-first; seed VAS default Chart of Accounts (Appendix II)
  - Maintain triple-provider compatibility

---

## 4. Frontend Guidelines
- Stack: Razor Views + Bootstrap 5.3+ + HTMX 2.x + Alpine.js 3.x
- Principles:
  - Server-first, progressive enhancement
  - Desktop-first, responsive
  - Vietnamese/VAS focus (Vietnamese labels, VND formatting: `1.234.567 ?`)
  - Accessibility: WCAG 2.1 AA minimum
  - Performance: <100 KB additional JS/CSS; TTFB <200 ms
- HTMX patterns:
  - `hx-post`/`hx-get` for form submits and filters
  - `hx-target` + `hx-swap="innerHTML"`
  - `hx-indicator` for spinner states
- Alpine.js: small local state only (modals, toggles)
- Prohibited: inline JS, heavy jQuery, full-page reloads when HTMX fits

---

## 5. Reporting & FastReport
- Engine: FastReport.OpenSource 2026.1.4 + PdfSimple plugin
- Approach: server-side via WebReport in Razor views
- Template location: `wwwroot/Reports/`
- Integration flow: Controller -> Service -> DTO list -> `RegisterData` -> View with WebReport
- Export: PDF prioritized via PdfSimple
- VAS compliance: match Circular 99 appendices, comparative periods, VND formatting

---

## 6. VAS Compliance
Legal reference: Circular 99/2025/TT-BTC (effective January 1, 2026)

Core rules:
- Double-entry: Debits = Credits per journal entry
- Currency: Default and mandatory = VND
- Fiscal year: January to December
- Chart of accounts: seed core Level 1 from Appendix II; allow customization for Level 2+

Key seeded accounts:
- 111: Tiền mặt
- 112: Tiền gửi không kỳ hạn
- 131: Phải thu khách hàng
- 152: Nguyên liệu, vật liệu
- 211: Tài sản cố định hữu hình
- 311: Vay ngắn hạn
- 331: Phải trả người bán
- 411: Vốn đầu tư của chủ sở hữu
- 511: Doanh thu bán hàng và cung cấp dịch vụ
- 632: Giá vốn hàng bán

Prohibited:
- Using eliminated accounts from Circular 200
- Unbalanced journal entries
- Ignoring VND default

---

## 7. Security (Always Enforce)
- Enforce HTTPS
- Secure cookies (`HttpOnly`, `Secure`, `SameSite=Strict`)
- Input validation + AntiForgeryToken
- Role-based authorization
- Never log sensitive data

---

## 8. Review Checklist
- [ ] Naming & formatting
- [ ] Async all the way
- [ ] VAS compliance (balanced entries, core CoA protected)
- [ ] No magic strings
- [ ] Secure & performant

---

## 9. Quick Start
Controllers:
- Keep thin: inject service, call method, return `View(model)`

Services:
- Inject DbContext
- Place all business logic here
- Example: `await _context.JournalEntries.AddAsync(entry); await _context.SaveChangesAsync();`

Views:
- Bootstrap + HTMX + Alpine
- Vietnamese labels
- VND formatting

Reports:
- Service aggregates DTOs
- Controller creates WebReport
- View renders preview + PDF button

---

## 10. Git & Version Control
- Commit frequently: after each completed task or major change (feature, bug fix, refactor)
- Commit message format:
  - `Task X: [short description]`
  - `Fix: [bug + fix summary]`
  - `Refactor: [change summary]`
  - `Style: [formatting/naming updates]`
- Push after commit:
  - `git add .`
  - `git commit -m "Task 3: Triple DB config + VAS CoA seeding"`
  - `git push origin main`
- If repo is not initialized:
  - `git init`
  - `git remote add origin https://github.com/yourusername/AccountingVASApp.git`
  - `git branch -M main`
  - `git push -u origin main`
- Branching: `main` for stable code; use `feature/task-X` for longer development
- `.gitignore`: do not commit `appsettings.Production.json`, `*.db`, or user secrets

---

## 11. Versioning & Release
- Semantic versioning: `MAJOR.MINOR.PATCH`
  - MAJOR: breaking changes
  - MINOR: backward-compatible features
  - PATCH: fixes and minor polish
- Tags after stable release:
  - `git tag -a v1.0.0 -m "Release v1.0.0: Initial VAS-compliant version"`
  - `git push origin v1.0.0`
- Publish release notes/changelog on GitHub
- Deployment:
  - Dev: `dotnet run` (SQLite)
  - Staging: Publish -> IIS/Kestrel + SQL Server Express
  - Prod: Publish -> IIS/Kestrel + SQL Server full + HTTPS

---

## 12. Environment Variables & Secrets
- Never hard-code connection strings, passwords, or keys
- Development: use user-secrets
  - `dotnet user-secrets init`
  - `dotnet user-secrets set "Database:ConnectionStrings:SqlServer" "Server=..."`
- Production: use environment variables or non-committed secret files
- Enable secret scanning in GitHub

---

## 13. Common Commands & Troubleshooting
- Build: `dotnet build`
- Run dev: `cd src/Web && dotnet run`
- Migration:
  - Add: `dotnet ef migrations add Name --project src/Web`
  - Update: `dotnet ef database update --project src/Web`
- Test: `dotnet test`
- Publish: `dotnet publish -c Release -o ./publish`

Troubleshooting:
- EF provider errors -> check appsettings and connection string
- Migration conflict -> `dotnet ef migrations remove`, then re-add
- SQLite lock (dev) -> remove `.db` file if safe
- HTTPS dev cert: `dotnet dev-certs https --trust`

---

## 14. Dependencies & Versions (Pinned March 2026)
- .NET SDK: 9.0.100+
- EF Core: 9.0.x
- Identity: 9.0.x
- Serilog.AspNetCore: 8.x
- FastReport.OpenSource: 2026.1.x
- Bootstrap: 5.3+
- HTMX: 2.x
- Alpine.js: 3.x
- xUnit: 2.9+

Upgrade rule:
- Update packages incrementally
- Run full tests
- Commit with message: `Upgrade: EF Core 9.0.x -> 9.0.y`

---

**Last Updated:** March 9, 2026

