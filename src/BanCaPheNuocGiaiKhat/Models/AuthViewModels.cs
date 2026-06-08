using System.ComponentModel.DataAnnotations;

namespace BanCaPheNuocGiaiKhat.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ tên")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = UserRoles.Customer;
}


public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [Display(Name = "Email")]
    public string LoginName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
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
