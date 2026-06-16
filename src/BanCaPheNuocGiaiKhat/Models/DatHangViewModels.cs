using System.ComponentModel.DataAnnotations;

namespace BanCaPheNuocGiaiKhat.Models;

public class DatHangViewModel
{
    public List<GioHangItemViewModel> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);

    [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
    [StringLength(100)]
    [Display(Name = "Họ tên người nhận")]
    public string RecipientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string RecipientPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    [StringLength(300)]
    [Display(Name = "Địa chỉ giao hàng")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }
}

public class DatHangThanhCongViewModel
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<DatHangItemViewModel> Items { get; set; } = new();
}

public class DatHangItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}

public class DonHangCuaToiTomTatViewModel
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
}

public class DonHangCuaToiListViewModel
{
    public List<DonHangCuaToiTomTatViewModel> Orders { get; set; } = new();
    public string? FilterStatus { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class DonHangCuaToiChiTietViewModel
{
    public int OrderId { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<DatHangItemViewModel> Items { get; set; } = new();
}