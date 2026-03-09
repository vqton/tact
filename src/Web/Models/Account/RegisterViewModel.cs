namespace Web.Models.Account;

using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui long nhap ho va ten.")]
    [StringLength(150)]
    [Display(Name = "Ho va ten")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap phong ban.")]
    [StringLength(100)]
    [Display(Name = "Phong ban")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap email.")]
    [EmailAddress(ErrorMessage = "Email khong hop le.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mat khau phai tu 8 ky tu tro len.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap lai mat khau.")]
    [Compare(nameof(Password), ErrorMessage = "Mat khau nhap lai khong khop.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nhap lai mat khau")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
