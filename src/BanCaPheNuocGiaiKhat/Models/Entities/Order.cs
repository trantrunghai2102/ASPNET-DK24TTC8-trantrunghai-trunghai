using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanCaPheNuocGiaiKhat.Models.Entities;

[Table("orders")]
public class Order
{
    [Key]
    [Column("order_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }

    [Column("staff_id")]
    public int? StaffId { get; set; }

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column("cash_given", TypeName = "decimal(18,2)")]
    public decimal? CashGiven { get; set; }

    [Column("change_amount", TypeName = "decimal(18,2)")]
    public decimal? ChangeAmount { get; set; }

    [Column("status")]
    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "pending";

    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(StaffId))]
    public User? Staff { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public Invoice? Invoice { get; set; }
}