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
  - `src/Views/` – Razor templates organized by controller
  - `src/wwwroot/` – Static assets, FastReport templates (*.frx)
  - `tests/` – Unit + Integration tests (xUnit)
- **Frontend**: Razor views + Bootstrap 5.3+ + HTMX 2.x + Alpine.js 3.x (server-first, progressive enhancement)
- **Database**: Triple provider – SQLite (development), SQL Server Express (staging/small production), SQL Server full (large production)
- **Key principle**: Controllers dispatch to Services → Services access DbContext → return View with model

---

## 2. CODE QUALITY & SKILLS (CodeQualityAndSkills.md)
- **Language**: C# 13 / .NET 9 – use modern features aggressively (primary constructors, collection expressions, null checks)
- **Naming Conventions**:
  - Classes/Records/Interfaces: `PascalCase` (e.g., `JournalEntry`, `IJournalService`)
  - Methods/Properties: `PascalCase`
  - Local variables/parameters: `camelCase`
  - Private fields: `_camelCase`
  - Async methods: `MethodNameAsync`
  - DTOs: Suffix with `Dto`, `Vm`, or `Response`
  - Test methods: `[MethodUnderTest]_[Scenario]_[Expected]`
- **Formatting**:
  - Indentation: 4 spaces (no tabs)
  - Max line length: 120 chars soft, 140 hard
  - Braces: Always use (even for single-line)
  - String interpolation over `string.Format`
  - Use `var` when type obvious; explicit otherwise
  - Modern patterns: `is null`, `??`, `?.`, null-forgiving `!` sparingly
- **Principles**: SOLID + DRY (accept duplication over wrong abstraction)
- **Code must be self-documenting** – prefer meaningful names over comments
- **Async everywhere**: All I/O-bound operations use async/await (no `.Result`, `.Wait()`)
- **Testing**: Target 80%+ coverage (xUnit), Arrange-Act-Assert pattern
- **Review Checklist**:
  - [ ] Naming & formatting rules followed
  - [ ] Business logic in Services, not controllers
  - [ ] Async all the way
  - [ ] Validated against VAS Compliance
  - [ ] Tests cover happy + edge paths
  - [ ] No magic strings/numbers
  - [ ] Readable (one responsibility per method/class)
  - [ ] Secure + performant

---

## 3. DATABASE & EF CORE (DatabaseAndEFCore.md)
- **ORM**: Entity Framework Core 9.0 (async-first)
- **Providers**: Triple – SQLite (dev), SQL Server Express (staging/small prod), SQL Server full (large prod)
- **Key Rules**:
  - Direct `DbContext` in Services – no Repository pattern
  - Async all the way: `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`
  - `AsNoTracking()` for read-only queries
  - Projections to DTOs for reports/ledger views
  - Compiled queries for hot paths
  - Fluent API configurations in `Data/Configurations/`
  - NO synchronous EF calls
  - NO loading full entities for reports
  - NO missing indexes on filter/sort columns
- **Entity Design**:
  - Rich domain models with invariants
  - Value Objects: `Money` (decimal Amount + string Currency = "VND"), `AccountCode`
  - Key entities: `Account`, `JournalEntry`, `TransactionEntry`, `AuditLogEntry`
- **Migrations**:
  - Code-first, seed VAS default Chart of Accounts (Appendix II)
  - Triple-provider compatibility

---

## 4. FRONTEND GUIDELINES (FrontendGuidelines.md)
- **Stack**: Razor Views + Bootstrap 5.3+ + HTMX 2.x + Alpine.js 3.x
- **Principles**:
  - Server-first, progressive enhancement
  - Desktop-first, responsive
  - VAS/Vietnamese focus (Vietnamese labels, VND formatting: 1.234.567 ₫)
  - Accessibility: WCAG 2.1 AA minimum
  - Performance: <100 KB additional JS/CSS; TTFB <200 ms
- **HTMX Patterns**:
  - `hx-post`/`hx-get` for form submits, filters
  - `hx-target` + `hx-swap="innerHTML"`
  - `hx-indicator` for spinner
- **Alpine.js**: Small state only (modals, toggles)
- **Prohibited**: Inline JS, heavy jQuery, full-page reloads when HTMX fits

---

## 5. REPORTING & FASTREPORT (ReportingAndFastReport.md)
- **Engine**: FastReport.OpenSource 2026.1.4 + PdfSimple plugin
- **Approach**: Server-side via WebReport in Razor views
- **Template Location**: `wwwroot/Reports/`
- **Integration**: Controller → Service → DTO list → RegisterData → View with WebReport
- **Export**: PDF priority via PdfSimple
- **VAS-compliant**: Match Circular 99 appendices, comparative periods, VND formatting

---

## 6. VAS COMPLIANCE (VASCompliance.md)
**Legal Reference**: Circular 99/2025/TT-BTC (effective Jan 1, 2026)

### Core Rules
- Double-entry: Debits = Credits per entry
- Currency: Default & mandatory = VND
- Fiscal year: Jan–Dec
- Chart of Accounts: Seed core Level 1 from Appendix II, allow customization Level 2+

### Key Accounts (seed in migrations)
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
- (etc. – ~15 core accounts)

### Prohibited
- Using eliminated accounts from Circular 200
- Unbalanced entries
- Ignoring VND default

---

## 7. SECURITY (Always Enforce)
- HTTPS enforcement
- Secure cookies (HttpOnly, Secure, SameSite=Strict)
- Input validation + AntiForgeryToken
- Role-based authorization
- Never log sensitive data

---

## 8. REVIEW CHECKLIST
- [ ] Naming & formatting
- [ ] Async all the way
- [ ] VAS compliance (balanced entries, core CoA protected)
- [ ] No magic strings
- [ ] Secure & performant

---

## 9. QUICK START (What to do when generating code)

**Controllers**:
- Thin: Inject service, call method, return View(model)

**Services**:
- Inject DbContext
- All business logic here
- Example: `await _context.JournalEntries.AddAsync(entry); await _context.SaveChangesAsync();`

**Views**:
- Bootstrap + HTMX + Alpine
- Vietnamese labels
- VND formatting

**Reports**:
- Service aggregates DTOs
- Controller creates WebReport
- View renders preview + PDF button

---

## 10. GIT & VERSION CONTROL ("Làm tới đâu push tới đó")
- **Commit thường xuyên**: Sau mỗi task hoàn thành hoặc thay đổi lớn (feature, fix bug, refactor)
- **Commit message format**:
  - `Task X: [Mô tả ngắn gọn]`
  - `Fix: [Mô tả lỗi + cách fix]`
  - `Refactor: [Mô tả thay đổi]`
  - `Style: [Cập nhật formatting, naming]`
- **Push ngay sau commit**:
  - `git add .`
  - `git commit -m "Task 3: Triple DB config + VAS CoA seeding"`
  - `git push origin main`
- **Nếu chưa có repo**:
  - `git init`
  - `git remote add origin https://github.com/yourusername/AccountingVASApp.git`
  - `git branch -M main`
  - `git push -u origin main`
- **Branching**: `main` cho code ổn định; tạo branch `feature/task-X` nếu cần phát triển dài
- **.gitignore**: Đảm bảo không commit appsettings.Production.json, *.db, user secrets

---

## 11. VERSIONING & RELEASE
- **Semantic Versioning**: MAJOR.MINOR.PATCH
  - MAJOR: Breaking changes (VAS rule lớn, breaking API)
  - MINOR: Tính năng mới không phá vỡ
  - PATCH: Bug fix, polish
- **Git tags**: Sau release stable → tag `v1.0.0`
  - `git tag -a v1.0.0 -m "Release v1.0.0: Initial VAS-compliant version"`
  - `git push origin v1.0.0`
- **Release Notes**: Tạo release trên GitHub với changelog (tóm tắt tasks, fixes)
- **Deployment**:
  - Dev: `dotnet run` (SQLite)
  - Staging: Publish → IIS/Kestrel + SQL Server Express
  - Prod: Publish → IIS/Kestrel + SQL Server full, HTTPS enforced

---

## 12. ENVIRONMENT VARIABLES & SECRETS
- **Không hard-code** connection strings, passwords, keys
- **Development**: Use `dotnet user-secrets`
  - `dotnet user-secrets init`
  - `dotnet user-secrets set "Database:ConnectionStrings:SqlServer" "Server=..."`
- **Production**: Environment variables hoặc file secrets.json (không commit)
- **Secret scanning**: Bật GitHub secret scanning để phát hiện leak

---

## 13. COMMON COMMANDS & TROUBLESHOOTING
- Build: `dotnet build`
- Run dev: `cd src/Web && dotnet run`
- Migration:
  - Add: `dotnet ef migrations add Name --project src/Web`
  - Update: `dotnet ef database update --project src/Web`
- Test: `dotnet test`
- Publish: `dotnet publish -c Release -o ./publish`
- Troubleshooting:
  - EF provider error → kiểm tra appsettings + connection string
  - Migration conflict → `dotnet ef migrations remove` rồi add lại
  - SQLite lock → xóa file .db khi dev
  - HTTPS dev cert: `dotnet dev-certs https --trust`

---

## 14. DEPENDENCIES & VERSIONS (Pinned as of March 2026)
- .NET SDK: 9.0.100+
- EF Core: 9.0.x
- Identity: 9.0.x
- Serilog.AspNetCore: 8.x
- FastReport.OpenSource: 2026.1.x
- Bootstrap: 5.3+
- HTMX: 2.x
- Alpine.js: 3.x
- xUnit: 2.9+

Khi nâng cấp: Update từng package, chạy test đầy đủ, commit với message "Upgrade: EF Core 9.0.x → 9.0.y"

---

**Last Updated:** March 9, 2026