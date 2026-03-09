namespace AccountingVAS.Web.Data.Seeders;

using Microsoft.EntityFrameworkCore;
using AccountingVAS.Web.Models;

/// <summary>
/// Seeder for default Chart of Accounts based on Circular 99/2025/TT-BTC (VAS).
/// Seeds 15 core accounts across Classes 1-5 (Asset, Liability, Equity, Revenue, Expense).
/// Appendix II: VAS Default Chart of Accounts structure.
/// </summary>
public class VasChartOfAccountsSeeder(AccountingDbContext context, ILogger<VasChartOfAccountsSeeder> logger)
{
    /// <summary>
    /// Seeds the default Chart of Accounts to an empty database.
    /// All accounts are created as Active and are not depth-limited (full CoA).
    /// </summary>
    public async Task SeedAsync()
    {
        var accounts = new List<Account>
        {
            // Class 1: ASSETS
            Account.Create("111", "Cash and Cash Equivalents", AccountType.Asset, level: 1),
            Account.Create("112", "Short-term Deposits with Banks", AccountType.Asset, level: 1),
            Account.Create("131", "Trade Receivables", AccountType.Asset, level: 1),
            Account.Create("152", "Inventories", AccountType.Asset, level: 1),

            // Class 2: FIXED ASSETS
            Account.Create("211", "Tangible Fixed Assets", AccountType.Asset, level: 1),
            Account.Create("215", "Biological Assets", AccountType.Asset, level: 1),

            // Class 3: LIABILITIES & EQUITY
            Account.Create("311", "Short-term Borrowings", AccountType.Liability, level: 1),
            Account.Create("312", "Trade Payables", AccountType.Liability, level: 1),
            Account.Create("331", "Equity / Capital", AccountType.Equity, level: 1),
            Account.Create("332", "Dividends Payable / Profit Distribution", AccountType.Equity, level: 1),

            // Class 4: REVENUE
            Account.Create("411", "Sales Revenue", AccountType.Revenue, level: 1),

            // Class 5: EXPENSES
            Account.Create("511", "Cost of Goods Sold", AccountType.Expense, level: 1),
            Account.Create("632", "Operating Expenses", AccountType.Expense, level: 1),

            // Class 8: TAXES & OTHER
            Account.Create("8211", "Corporate Income Tax", AccountType.Expense, level: 1),
            Account.Create("82112", "Additional Corporate Income Tax (Global Base Erosion – GloBE)", AccountType.Expense, level: 1),
        };

        await context.Accounts.AddRangeAsync(accounts);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} default Chart of Accounts entries.", accounts.Count);
    }
}

