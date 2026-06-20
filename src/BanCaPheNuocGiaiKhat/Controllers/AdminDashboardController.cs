using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class AdminDashboardController : Controller
{
    private readonly AppDbContext _db;

    public AdminDashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? month, int? year, DateTime? fromDate, DateTime? toDate)
    {
        var today = DateTime.Today;
        var selectedYear = year ?? today.Year;
        var selectedMonth = month;

        DateTime periodStart;
        DateTime periodEnd;
        if (fromDate.HasValue || toDate.HasValue)
        {
            periodStart = (fromDate ?? today.AddDays(-30)).Date;
            periodEnd = (toDate ?? today).Date.AddDays(1);
        }
        else if (selectedMonth.HasValue)
        {
            periodStart = new DateTime(selectedYear, selectedMonth.Value, 1);
            periodEnd = periodStart.AddMonths(1);
        }
        else
        {
            periodStart = new DateTime(selectedYear, 1, 1);
            periodEnd = periodStart.AddYears(1);
        }

        var invoiceQuery = _db.Invoices
            .AsNoTracking()
            .Where(i => i.PaidAt >= periodStart && i.PaidAt < periodEnd);

        var totalRevenue = await invoiceQuery.SumAsync(i => (decimal?)i.TotalAmount) ?? 0m;
        var totalOrders = await invoiceQuery.CountAsync();

        var isMonthlyView = !selectedMonth.HasValue && !fromDate.HasValue && !toDate.HasValue;

        List<AdminRevenuePointViewModel> revenueByDay;
        if (isMonthlyView)
        {
            // Fetch daily data first, then aggregate to monthly in-memory (EF can't translate new DateTime in GroupBy)
            var dailyData = await invoiceQuery
                .GroupBy(i => i.PaidAt.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(i => i.TotalAmount), Orders = g.Count() })
                .ToListAsync();

            revenueByDay = dailyData
                .GroupBy(d => new { d.Date.Year, d.Date.Month })
                .Select(g => new AdminRevenuePointViewModel
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Revenue = g.Sum(x => x.Revenue),
                    Orders = g.Sum(x => x.Orders)
                })
                .OrderBy(x => x.Date)
                .ToList();
        }
        else
        {
            revenueByDay = await invoiceQuery
                .GroupBy(i => i.PaidAt.Date)
                .Select(g => new AdminRevenuePointViewModel
                {
                    Date = g.Key,
                    Revenue = g.Sum(i => i.TotalAmount),
                    Orders = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        var orderIds = await invoiceQuery.Select(i => i.OrderId).ToListAsync();
        var topProducts = orderIds.Count == 0
            ? new List<AdminTopProductViewModel>()
            : await _db.OrderItems
                .AsNoTracking()
                .Where(i => orderIds.Contains(i.OrderId))
                .GroupBy(i => i.ProductName)
                .Select(g => new AdminTopProductViewModel
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(6)
                .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            Month = selectedMonth,
            Year = selectedYear,
            FromDate = fromDate,
            ToDate = toDate,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd.AddDays(-1),
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            AverageOrderValue = totalOrders == 0 ? 0 : totalRevenue / totalOrders,
            TotalProductsSold = topProducts.Sum(p => p.Quantity),
            RevenueByDay = revenueByDay,
            IsMonthlyView = isMonthlyView,
            TopProducts = topProducts,
            RecentInvoices = await invoiceQuery
                .OrderByDescending(i => i.PaidAt)
                .Take(5)
                .Select(i => new AdminRecentInvoiceViewModel
                {
                    InvoiceCode = i.InvoiceCode,
                    TotalAmount = i.TotalAmount,
                    PaidAt = i.PaidAt
                })
                .ToListAsync()
        };

        return View("~/Views/Admin/Home/Index.cshtml", model);
    }
}
