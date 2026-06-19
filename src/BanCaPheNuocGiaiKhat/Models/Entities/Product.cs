using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanCaPheNuocGiaiKhat.Models.Entities;

[Table("products")]
public class Product
{
    [Key]
    [Column("product_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("name")]
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column("slug")]
    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Column("base_price", TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }

    [Column("promotion_price", TypeName = "decimal(18,2)")]
    public decimal? PromotionPrice { get; set; }

    [Column("short_desc")]
    [StringLength(500)]
    public string? ShortDesc { get; set; }

    [Column("detail_desc")]
    public string? DetailDesc { get; set; }

    [Column("thumbnail_url")]
    [StringLength(500)]
    public string? ThumbnailUrl { get; set; }

    [Column("stock_qty")]
    public int StockQty { get; set; } = 0;

    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    [Column("status")]
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "active";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
