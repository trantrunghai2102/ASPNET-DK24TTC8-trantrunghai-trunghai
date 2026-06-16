namespace BanCaPheNuocGiaiKhat.Models;

public class DonHangOnlineTomTatViewModel
{
    public int OrderId { get; set; }
    public string? CustomerName { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DonHangOnlineListViewModel
{
    public List<DonHangOnlineTomTatViewModel> Orders { get; set; } = new();
    public string? FilterStatus { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class DonHangOnlineChiTietViewModel
{
    public int OrderId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? StaffName { get; set; }
    public List<DonHangOnlineItemViewModel> Items { get; set; } = new();
}

public class DonHangOnlineItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}

public class CapNhatTrangThaiInput
{
    public int OrderId { get; set; }
    public string NewStatus { get; set; } = string.Empty;
}