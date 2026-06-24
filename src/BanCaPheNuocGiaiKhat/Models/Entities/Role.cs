using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanCaPheNuocGiaiKhat.Models.Entities;

[Table("roles")]
public class Role
{
    [Key]
    [Column("role_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public byte RoleId { get; set; }

    [Column("role_name")]
    [Required]
    [StringLength(30)]
    public string RoleName { get; set; } = string.Empty;

    [Column("description")]
    [StringLength(150)]
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
