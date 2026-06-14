using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanCaPheNuocGiaiKhat.Models.Entities;

[Table("product_images")]
public class ProductImage
{
    [Key]
    [Column("image_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ImageId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("url")]
    [Required]
    [StringLength(500)]
    public string Url { get; set; } = string.Empty;

    [Column("alt_text")]
    [StringLength(200)]
    public string? AltText { get; set; }

    [Column("is_primary")]
    public bool IsPrimary { get; set; } = false;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    // Navigation
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
}
