using System.ComponentModel.DataAnnotations;

namespace BanCaPheNuocGiaiKhat.Models;

public class SanPhamChonViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class BanHangViewModel
{
    public List<SanPhamChonViewModel> Products { get; set; } = new();
}

// POST model — cart items sent from JS as indexed form fields
public class CartItemInput
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class TaoHoaDonInput
{
    public List<CartItemInput> Items { get; set; } = new();
}

public class ThanhToanItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}

public class ThanhToanViewModel
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ThanhToanItemViewModel> Items { get; set; } = new();

    [Required(ErrorMessage = "Vui lòng nhập số tiền khách đưa.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
    public decimal CashGiven { get; set; }
}