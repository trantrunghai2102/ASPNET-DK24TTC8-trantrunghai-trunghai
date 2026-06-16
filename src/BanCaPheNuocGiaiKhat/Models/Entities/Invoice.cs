using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanCaPheNuocGiaiKhat.Models.Entities;

[Table("invoices")]
public class Invoice
{
    [Key]
    [Column("invoice_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvoiceId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("invoice_code")]
    [Required]
    [StringLength(50)]
    public string InvoiceCode { get; set; } = string.Empty;

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column("cash_given", TypeName = "decimal(18,2)")]
    public decimal CashGiven { get; set; }

    [Column("change_amount", TypeName = "decimal(18,2)")]
    public decimal ChangeAmount { get; set; }

    [Column("paid_at")]
    public DateTime PaidAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;
}