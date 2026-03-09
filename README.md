# AccountingVAS - Vietnam Accounting Standards Compliant System

VAS-compliant accounting ledger application built with .NET 9, ASP.NET Core MVC, and Clean Architecture.

## Technology Stack

- **.NET 9.0** (STS - supported until Nov 2026)
- **ASP.NET Core MVC** with Razor Views
- **Entity Framework Core 9.0**
  - SQLite (development/small installations)
  - SQL Server Express (production)
- **MediatR** - CQRS pattern
- **FluentValidation** - VAS business rules validation
- **Mapster** - Object mapping
- **Serilog** - Structured logging
- **FastReport.OpenSource** - Report generation
- **ASP.NET Core Identity** - Authentication & Authorization

## Prerequisites

- .NET 9.0 SDK
- Visual Studio Code with C# Dev Kit
- SQL Server Express (for production)
- Optional: Seq (for log aggregation)

## Setup in VS Code

### 1. Restore Packages

```bash
dotnet restore
```

### 2. Database Setup

**For SQLite (Development):**
```bash
cd src/Web
dotnet ef migrations add InitialCreate --project ../Infrastructure
dotnet ef database update
```

**For SQL Server (Production):**
1. Install SQL Server Express
2. Update connection string in `appsettings.Production.json`
3. Set environment variable: `Database__Provider=SqlServer`
4. Run migrations:
```bash
dotnet ef database update
```

### 3. Run Application

**Using VS Code:**
- Press `F5` and select "Launch (SQLite)" or "Launch (SQL Server)"

**Using CLI:**
```bash
cd src/Web
dotnet run
```

**Using Watch (Hot Reload):**
```bash
dotnet watch run --project src/Web/Web.csproj
```

### 4. Switch Database Provider

**Development (SQLite):**
- Set in `appsettings.Development.json`: `"Database": { "Provider": "SQLite" }`

**Production (SQL Server):**
- Set in `appsettings.Production.json`: `"Database": { "Provider": "SqlServer" }`
- Or use environment variable: `Database__Provider=SqlServer`

## Project Structure

```
src/
├── Core/              # Domain entities, enums, value objects
├── Application/       # MediatR commands/queries, DTOs, validators
├── Infrastructure/    # EF Core, Identity, data access
└── Web/              # MVC controllers, views, wwwroot
tests/
└── Tests/            # xUnit tests
```

## VAS Compliance (Circular 99/2025/TT-BTC)

This system implements:
- Flexible Chart of Accounts with new VAS accounts (215, 2295, 1383, etc.)
- Double-entry bookkeeping validation (Debit = Credit)
- VND primary currency with proper formatting
- Audit trail for all transactions
- Financial reports: Balance Sheet, Income Statement, Cash Flow
- Role-based access: Admin, Accountant, Auditor, Viewer

## FastReport Setup

### Designing Reports

1. Download FastReport Community Designer
2. Create `.frx` files in `wwwroot/Reports/`
3. Register data sources in controller
4. Preview/export via WebReport component

### Example Report Controller Action

```csharp
public IActionResult BalanceSheet(DateTime? asOfDate)
{
    var data = GetBalanceSheetData(asOfDate ?? DateTime.Today);
    
    var webReport = new WebReport();
    webReport.Report.RegisterData(data, "BalanceSheet");
    webReport.Report.Load(Path.Combine(_env.WebRootPath, "Reports/BalanceSheet.frx"));
    
    return View(webReport);
}
```

## Testing

```bash
dotnet test
```

## Deployment

### IIS (Windows)

1. Publish: `dotnet publish -c Release`
2. Copy output to IIS directory
3. Configure application pool (.NET CLR Version: No Managed Code)
4. Set connection string in `appsettings.Production.json`

### Linux systemd

1. Publish: `dotnet publish -c Release -r linux-x64`
2. Copy to `/var/www/accountingvas/`
3. Create systemd service file
4. Enable and start service

## Logging

Logs are written to:
- Console (development)
- `logs/accounting-{Date}.log` files
- Seq (if configured at http://localhost:5341)

## Security

- Cookie-based authentication
- HTTPS enforced
- Password requirements: 8+ chars, uppercase, lowercase, digit, special char
- Account lockout after 5 failed attempts
- Role-based authorization policies

## Future Migration to .NET 10 LTS

This codebase is designed for easy migration to .NET 10 LTS (Nov 2025):
- Update `global.json` SDK version
- Update package versions
- Recompile and test

## License

Proprietary - For internal use only
