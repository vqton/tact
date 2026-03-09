namespace Web.Models.ChartOfAccounts;

using System.ComponentModel.DataAnnotations;
using AccountingVAS.Web.Models;

public class ChartOfAccountFormVm
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui long nhap ma tai khoan.")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Ma TK phai gom dung 4 chu so.")]
    [Display(Name = "Ma TK")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap ten tai khoan.")]
    [StringLength(255)]
    [Display(Name = "Ten TK")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Loai TK")]
    public AccountType Type { get; set; } = AccountType.Asset;

    [Display(Name = "Tai khoan cha")]
    public int? ParentId { get; set; }
}
