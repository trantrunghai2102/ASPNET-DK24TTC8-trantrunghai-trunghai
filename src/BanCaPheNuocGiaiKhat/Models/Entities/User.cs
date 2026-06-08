using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanCaPheNuocGiaiKhat.Models.Entities;

public enum UserStatus
{
    active,
    locked,
    pending
}

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint UserId { get; set; }

    [Column("role_id")]
    public byte RoleId { get; set; } = 3;

    [Column("full_name")]
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Column("email")]
    [Required]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Column("phone")]
    [StringLength(15)]
    public string? Phone { get; set; }

    [Column("address")]
    [StringLength(300)]
    public string? Address { get; set; }

    [Column("avatar_url")]
    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    [Column("password_hash")]
    [StringLength(255)]
    public string? PasswordHash { get; set; }

    [Column("google_id")]
    [StringLength(100)]
    public string? GoogleId { get; set; }

    [Column("facebook_id")]
    [StringLength(100)]
    public string? FacebookId { get; set; }

    [Column("status")]
    public UserStatus Status { get; set; } = UserStatus.active;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }
}
