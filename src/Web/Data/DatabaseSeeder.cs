namespace AccountingVAS.Web.Data;

using AccountingVAS.Web.Models;
using Microsoft.EntityFrameworkCore;

public static class DatabaseSeeder
{
    public static async Task SeedVasCoaIfNeededAsync(AccountingDbContext db)
    {
        if (await db.Accounts.AnyAsync())
        {
            return;
        }

        var coreAccounts = new (string Code, string Name, AccountType Type)[]
        {
            ("111", "Tien mat", AccountType.Asset),
            ("112", "Tien gui ngan hang", AccountType.Asset),
            ("113", "Tien dang chuyen", AccountType.Asset),
            ("121", "Chung khoan kinh doanh", AccountType.Asset),
            ("131", "Phai thu cua khach hang", AccountType.Asset),
            ("133", "Thue GTGT duoc khau tru", AccountType.Asset),
            ("152", "Nguyen lieu, vat lieu", AccountType.Asset),
            ("156", "Hang hoa", AccountType.Asset),
            ("211", "Tai san co dinh huu hinh", AccountType.Asset),
            ("311", "Vay va no thue tai chinh ngan han", AccountType.Liability),
            ("331", "Phai tra cho nguoi ban", AccountType.Liability),
            ("333", "Thue va cac khoan phai nop Nha nuoc", AccountType.Liability),
            ("411", "Von dau tu cua chu so huu", AccountType.Equity),
            ("511", "Doanh thu ban hang va cung cap dich vu", AccountType.Revenue),
            ("632", "Gia von hang ban", AccountType.Expense)
        };

        var existingCodes = await db.Accounts
            .Select(x => x.Code)
            .ToHashSetAsync();

        foreach (var (code, name, type) in coreAccounts)
        {
            if (existingCodes.Contains(code))
            {
                continue;
            }

            db.Add(Account.Create(code, name, type, level: 1));
            existingCodes.Add(code);
        }

        await db.SaveChangesAsync();
    }
}
