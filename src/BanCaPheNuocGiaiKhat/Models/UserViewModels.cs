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
    
    // Filters
    public string? SearchQuery { get; set; }
    public byte? SelectedRoleId { get; set; }
}
