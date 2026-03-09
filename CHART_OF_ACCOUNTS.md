# VAS Chart of Accounts Implementation

## What Was Created

### Core Layer (Domain)
✅ **AccountType enum** - VAS account classification (Asset, Liability, Equity, Revenue, Expense, OffBalance)
✅ **Account entity** - Chart of Accounts with hierarchical structure

### Infrastructure Layer
✅ **AccountConfiguration** - EF Core entity configuration with unique index on Code
✅ **VasChartOfAccountsSeeder** - Seed data with 31 VAS-compliant accounts per Circular 99/2025/TT-BTC
✅ **ApplicationDbContext** - Updated with Account DbSet and seed data

## Key VAS Accounts Included (Circular 99/2025/TT-BTC)

### New Accounts in Circular 99:
- **215** - Tài sản sinh học (Biological assets)
- **2295** - Dự phòng giảm giá tài sản sinh học (Provision for impairment of biological assets)
- **1383** - Thuế TTĐB hàng nhập khẩu (Special consumption tax on imports)

### Standard VAS Accounts:
- **Class 1**: Assets (111 Cash, 112 Bank, 131 AR, 152 Materials, 156 Inventory, 211 Fixed Assets, etc.)
- **Class 2**: Liabilities (331 AP, 333 Taxes, 334 Salaries, 335 Accrued expenses, 341 Loans)
- **Class 3**: Equity (411 Owner's capital, 421 Retained earnings)
- **Class 5**: Expenses (511 Purchases, 621-627 Manufacturing costs, 641 Selling, 642 Admin)
- **Class 7**: Revenue (511 Sales, 515 Financial income, 711 Other income)
- **Class 8**: Other (811 Other expenses, 821 CIT expense)
- **Class 9**: Off-balance (002 Consignment, 003 Leased assets)

## Next Steps

### 1. Create Initial Migration

```bash
cd src/Web
dotnet ef migrations add InitialCreate --project ../Infrastructure --output-dir Data/Migrations
```

### 2. Apply Migration

```bash
dotnet ef database update
```

Or just run the application - migrations apply automatically on startup.

### 3. Verify Seed Data

After running, check that 31 VAS accounts are seeded:

```sql
-- SQLite
SELECT Code, NameVi, Type FROM Accounts ORDER BY Code;

-- SQL Server
SELECT Code, NameVi, Type FROM Accounts ORDER BY Code;
```

## Features

✅ **Hierarchical structure** - Parent-child relationships supported
✅ **Unique account codes** - Database constraint enforced
✅ **VAS compliance** - Circular 99/2025/TT-BTC accounts
✅ **System accounts** - Marked as IsSystem = true (cannot be deleted)
✅ **Audit trail** - CreatedAt, CreatedBy fields
✅ **Bilingual** - Name (English) and NameVi (Vietnamese)

## Usage Example

```csharp
// Query accounts
var cashAccount = await context.Accounts
    .FirstOrDefaultAsync(a => a.Code == "111");

// Get all asset accounts
var assets = await context.Accounts
    .Where(a => a.Type == AccountType.Asset && a.IsActive)
    .OrderBy(a => a.Code)
    .ToListAsync();
```
