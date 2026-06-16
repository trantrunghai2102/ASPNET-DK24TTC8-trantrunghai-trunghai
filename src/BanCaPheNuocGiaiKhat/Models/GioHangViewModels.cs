using System.ComponentModel.DataAnnotations;

namespace BanCaPheNuocGiaiKhat.Models;

public class GioHangItemViewModel
{
    public int CartItemId { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int StockQty { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}

public class GioHangViewModel
{
    public List<GioHangItemViewModel> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
}

public class ThemVaoGioHangInput
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;
}

public class CapNhatGioHangInput
{
    [Required]
    public int CartItemId { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; }
}