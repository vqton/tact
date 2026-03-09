namespace Web.Models.JournalEntries;

using System.ComponentModel.DataAnnotations;

public class JournalEntryCreateVm
{
    [Required(ErrorMessage = "Vui long nhap chung tu.")]
    [StringLength(100, ErrorMessage = "Chung tu khong duoc vuot qua 100 ky tu.")]
    [Display(Name = "Chung tu")]
    public string VoucherRef { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap ngay.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngay")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Vui long nhap dien giai.")]
    [StringLength(500, ErrorMessage = "Dien giai khong duoc vuot qua 500 ky tu.")]
    [Display(Name = "Dien giai")]
    public string Description { get; set; } = string.Empty;

    [MinLength(2, ErrorMessage = "Can it nhat 2 dong dinh khoan.")]
    public List<JournalEntryLineVm> Lines { get; set; } =
    [
        new(),
        new()
    ];
}

public class JournalEntryLineVm
{
    [Required(ErrorMessage = "Vui long chon tai khoan.")]
    public int? AccountId { get; set; }

    [Range(0, 9999999999999999.99, ErrorMessage = "No khong hop le.")]
    [Display(Name = "No")]
    public decimal Debit { get; set; }

    [Range(0, 9999999999999999.99, ErrorMessage = "Co khong hop le.")]
    [Display(Name = "Co")]
    public decimal Credit { get; set; }
}
