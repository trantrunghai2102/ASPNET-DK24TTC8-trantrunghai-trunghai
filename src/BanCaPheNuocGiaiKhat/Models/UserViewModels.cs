using System.ComponentModel.DataAnnotations;
using BanCaPheNuocGiaiKhat.Models.Entities;

namespace BanCaPheNuocGiaiKhat.Models;

public class UserItemViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public byte RoleId { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UserListViewModel
{
    public List<UserItemViewModel> Users { get; set; } = new();
    public List<Role> Roles { get; set; } = new();
    
    public string? SearchQuery { get; set; }
    public byte? SelectedRoleId { get; set; }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class UserCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
    public string Password { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phân quyền.")]
    public byte RoleId { get; set; }
}

public class UserEditViewModel
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phân quyền.")]
    public byte RoleId { get; set; }
}
