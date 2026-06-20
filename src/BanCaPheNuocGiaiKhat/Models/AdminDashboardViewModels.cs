namespace BanCaPheNuocGiaiKhat.Models;

public class AdminDashboardViewModel
{
    public int? Month { get; set; }
    public int Year { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalProductsSold { get; set; }
    
    public List<AdminRevenuePointViewModel> RevenueByDay { get; set; } = new();
    public bool IsMonthlyView { get; set; }
    public List<AdminTopProductViewModel> TopProducts { get; set; } = new();
    public List<AdminRecentInvoiceViewModel> RecentInvoices { get; set; } = new();
}

public class AdminRevenuePointViewModel
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}

public class AdminTopProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
}

public class AdminRecentInvoiceViewModel
{
    public string InvoiceCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime PaidAt { get; set; }
}
