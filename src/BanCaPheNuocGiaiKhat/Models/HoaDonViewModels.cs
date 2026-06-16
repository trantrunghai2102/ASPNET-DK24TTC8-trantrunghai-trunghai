
namespace BanCaPheNuocGiaiKhat.Models;

public class HoaDonTomTatViewModel
{
    public int InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string? StaffName { get; set; }
}

public class DanhSachHoaDonViewModel
{
    public List<HoaDonTomTatViewModel> Invoices { get; set; } = new();
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