
namespace BanCaPheNuocGiaiKhat.Models;

public class HoaDonTomTatViewModel
{
    public int InvoiceId { get; set; }
    public int OrderId { get; set; }
    public string? InvoiceCode { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string? StaffName { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class DanhSachHoaDonViewModel
{
    public List<HoaDonTomTatViewModel> Invoices { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;


    // Added stats
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int TotalOrders { get; set; }
    public string FilterType { get; set; } = string.Empty;
}

public class HoaDonItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}

public class HoaDonChiTietViewModel
{
    public int InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CashGiven { get; set; }
    public decimal ChangeAmount { get; set; }
    public List<HoaDonItemViewModel> Items { get; set; } = new();
}