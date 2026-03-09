# Architecture Overview – Accounting WebApp (VAS-Compliant, Traditional MVC)
**Version:** 2.0 | **Effective Date:** March 2026  
**Framework:** .NET 9 (supported until Nov 2026)  
**Primary Goal:** Deliver a maintainable, testable, on-premises accounting ledger system strictly compliant with Circular 99/2025/TT-BTC (effective Jan 1, 2026) using a **traditional MVC monolith** architecture.  
**Key Constraints:** 100% on-premises (no cloud), **pure ASP.NET Core MVC** (server-rendered Razor views only – no API, no SPA frameworks), lightweight interactivity via HTMX 2.x + Alpine.js, dual DB support (SQLite for dev/small installs, SQL Server Express for production).

## 1. High-Level Architectural Style
We adopt a **traditional MVC monolith** – a straightforward, time-tested pattern well-suited for on-premises business applications:

- **Three main tiers**:
  - **Presentation (Controllers + Views)**: Thin controllers dispatch requests to services, render Razor views with model data.
  - **Business Logic (Services)**: Domain logic, validation, orchestration of multiple operations. Services inject DbContext directly.
  - **Data Access (EF Core DbContext)**: Entity Framework Core manages relational data access; no repository layer needed.

- **Code Organization by Technical Responsibility**:
  - Controllers handle HTTP requests/responses only.
  - Services handle business logic, validation, transactions.
  - Models represent domain concepts + database entities.
  - Views are Razor templates (no ViewModels if not needed; pass entities directly or use DTOs for projection).

This approach provides **simplicity** (easy to understand), **speed** (fewer layers), and **direct control** (right from controller to service to data).

## 2. Project Structure (Traditional MVC Folder Layout)
AccountingVASApp (solution root)
├── src/
│   ├── Models/                     # Domain + DTO models
│   │   ├── Account.cs              # Rich entity with validation logic
│   │   ├── Journal.cs
│   │   ├── TransactionEntry.cs
│   │   ├── ValueObjects/           # Money (Amount + Currency = "VND"), AccountCode
│   │   └── Dtos/                   # DTOs for views (BalanceSheetRowDto, etc.)
│   │
│   ├── Controllers/                # HTTP handlers (thin)
│   │   ├── ChartOfAccountsController.cs
│   │   ├── TransactionsController.cs
│   │   └── ReportsController.cs
│   │
│   ├── Services/                   # Business logic layer
│   │   ├── AccountService.cs       # CRUD + hierarchy logic
│   │   ├── JournalService.cs       # Journal entry posting, balance validation
│   │   ├── ReportService.cs        # Aggregations for Balance Sheet, Income Statement
│   │   ├── AuditService.cs         # Logging changes to audit trail
│   │   └── Interfaces/             # IAccountService, IJournalService, etc. (optional, for testing)
│   │
│   ├── Data/                       # Data access
│   │   ├── AccountingDbContext.cs  # EF Core DbContext
│   │   ├── Configurations/         # Fluent API entity configs
│   │   └── Migrations/
│   │
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml      # Master layout (Bootstrap navbar, sidebar)
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   ├── ChartOfAccounts/
│   │   │   ├── Index.cshtml        # CoA tree view
│   │   │   ├── Create.cshtml
│   │   │   └── Edit.cshtml
│   │   ├── Transactions/
│   │   │   ├── Index.cshtml        # Journal list
│   │   │   ├── Post.cshtml         # Journal entry form (with HTMX for dynamic lines)
│   │   │   └── _EntryLinePartial.cshtml
│   │   └── Reports/
│   │       ├── BalanceSheet.cshtml # FastReport WebReport host
│   │       └── IncomeStatement.cshtml
│   │
│   ├── wwwroot/
│   │   ├── css/                    # site.css (Bootstrap overrides)
│   │   ├── js/                     # app.js (HTMX/Alpine init, custom logic)
│   │   └── Reports/                # *.frx files (FastReport templates)
│   │
│   ├── Program.cs                  # Host setup, DI (AddScoped<IAccountService>, AddDbContext switch, etc.)
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── tests/
│   └── Tests/
│       ├── Unit/                   # Service + model tests (xUnit)
│       └── Integration/            # EF + MVC controller tests (Respawn for DB reset)
│
├── .amazonq/
│   └── rules/                      # VASCompliance.md, CodeQualityAndSkills.md, etc.
│
├── .editorconfig
├── Directory.Build.props
├── global.json                     # Pin .NET 9
└── AccountingVASApp.sln


## 3. Key Design Decisions & Trade-offs (Traditional MVC)
- **No MediatR / CQRS**: Controllers call services directly via dependency injection. Simpler, fewer abstractions, faster development.
- **No layered project structure**: Single project or minimal separation (src/ + tests/). Classes organized by technical responsibility (Controllers, Services, Models, Views).
- **Services own business logic**: Services handle validation, transactions, domain rules enforcement. Can inject DbContext directly.
- **No Repositories**: DbContext is injected into services; use `_context.Entities.Where(...).ToListAsync()` directly.
- **Direct entity usage in views**: For simple CRUDs, pass entities; for complex reports, use DTOs.
- **FastReport integration**: WebReport component rendered directly in Razor views (server-side generation, PDF export priority).
- **Frontend stack**: Razor + Bootstrap 5 + HTMX 2.x + Alpine.js → minimal JS, excellent for form-heavy accounting UI (journal entries, report filters).
- **DbContext switch**: Use configuration (appsettings.Development.json vs appsettings.Production.json) or factory pattern for SQLite ↔ SQL Server Express.
- **Authentication**: ASP.NET Core Identity (cookie-based, roles/policies for Accountant/Auditor/Viewer).
- **Why traditional MVC?**: Simpler codebase for small-to-medium teams, easier debugging, familiar patterns for .NET developers, lower cognitive load, faster time-to-feature.

## 4. Dependency & Control Flow
- **HTTP Request Flow**: Controller action receives request → calls injected service method → service accesses DbContext → returns result → controller returns View with model data
- **Service Layer**: Services inject DbContext (scoped), optionally ILogger, other services. May throw domain exceptions for invariant violations.
- **Models**: Rich entities with validation logic in property setters or dedicated methods. Value objects (Money, AccountCode) encapsulate domain rules.
- **DbContext scope**: Per HTTP request (scoped). Services manage transactions; EF handles change tracking.

## 5. When to Deviate (Allowed Exceptions)
- Simple CRUD endpoints → action can use DbContext directly if trivial (no business logic).
- Shared cross-cutting logic → utilities folder or extension methods (e.g., Money.ToVndString()).
- Performance-critical reports → custom EF projections in service method with caching if needed.

**Enforcement:**  
- Amazon Q generations, PRs, and refactors **MUST** follow this traditional MVC structure + VASCompliance.md + CodeQualityAndSkills.md.  
- New features: Always create matching service + controller + views.

Last updated: March 09, 2026 – Software Architect