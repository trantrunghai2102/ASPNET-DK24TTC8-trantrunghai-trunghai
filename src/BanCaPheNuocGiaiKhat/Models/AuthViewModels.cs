using System.ComponentModel.DataAnnotations;

namespace BanCaPheNuocGiaiKhat.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui long nhap ho ten.")]
    [Display(Name = "Ho ten")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap email.")]
    [EmailAddress(ErrorMessage = "Email khong hop le.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "So dien thoai")]
    [StringLength(15)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui long nhap ten dang nhap.")]
    [Display(Name = "Ten dang nhap")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ten dang nhap tu 3 den 50 ky tu.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mat khau toi thieu 6 ky tu.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = UserRoles.Customer;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui long nhap email hoac ten dang nhap.")]
    [Display(Name = "Email hoac ten dang nhap")]
    public string LoginName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nho dang nhap")]
    public bool RememberMe { get; set; }
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Vui long nhap mat khau hien tai.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau hien tai")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau moi.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mat khau moi toi thieu 6 ky tu.")]
    [Display(Name = "Mat khau moi")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long xac nhan mat khau moi.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Mat khau xac nhan khong khop.")]
    [Display(Name = "Xac nhan mat khau moi")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public static class UserRoles
{
    public const string Admin = "admin";
    public const string Staff = "staff";
    public const string Customer = "customer";

    public static readonly string[] All = [Admin, Staff, Customer];
}
