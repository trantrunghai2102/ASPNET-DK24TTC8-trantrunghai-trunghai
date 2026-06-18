using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Staff)]
public class StaffHoaDonController : Controller
{
    private readonly AppDbContext _db;

    public StaffHoaDonController(AppDbContext db)
    {
        _db = db;
    }

    // GET /StaffHoaDon
    public async Task<IActionResult> Index(int page = 1, string type = "")
    {
        const int pageSize = 10;

        var query = _db.Invoices
            .Include(i => i.Order).ThenInclude(o => o.Staff)
            .AsQueryable();

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(i => i.Order.OrderType == type);
        }

        query = query.OrderByDescending(i => i.PaidAt);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var invoices = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new HoaDonTomTatViewModel
            {
                InvoiceId   = i.InvoiceId,
                InvoiceCode = i.InvoiceCode,
                PaidAt      = i.PaidAt,
                TotalAmount = i.TotalAmount,
                StaffName   = i.Order.Staff != null ? i.Order.Staff.FullName : null,
                OrderType   = i.Order.OrderType
            })
            .ToListAsync();

        var pending = await _db.Orders.CountAsync(o => o.Status == "pending");
        var processing = await _db.Orders.CountAsync(o => o.Status == "processing");
        var completed = await _db.Orders.CountAsync(o => o.Status == "completed");

        var viewModel = new DanhSachHoaDonViewModel 
        { 
            Invoices = invoices,
            CurrentPage = page,
            TotalPages  = Math.Max(1, totalPages),
            PendingOrders = pending,
            ProcessingOrders = processing,
            CompletedOrders = completed,
            TotalOrders = pending + processing + completed,
            FilterType = type
        };

        return View("~/Views/Staff/HoaDon/Index.cshtml", viewModel);
    }

    // GET /StaffHoaDon/ChiTiet/{id}
    public async Task<IActionResult> ChiTiet(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);

        if (invoice == null)
            return NotFound();

        var vm = new HoaDonChiTietViewModel
        {
            InvoiceId    = invoice.InvoiceId,
            InvoiceCode  = invoice.InvoiceCode,
            PaidAt       = invoice.PaidAt,
            TotalAmount  = invoice.TotalAmount,
            CashGiven    = invoice.CashGiven,
            ChangeAmount = invoice.ChangeAmount,
            Items        = invoice.Order.OrderItems.Select(i => new HoaDonItemViewModel
            {
                ProductName = i.ProductName,
                UnitPrice   = i.UnitPrice,
                Quantity    = i.Quantity,
                Subtotal    = i.Subtotal
            }).ToList()
        };

        return View("~/Views/Staff/HoaDon/ChiTiet.cshtml", vm);
    }

    // POST /StaffHoaDon/Xoa/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Order)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);

        if (invoice == null) return NotFound();

        var code = invoice.InvoiceCode;
        _db.Orders.Remove(invoice.Order);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa hóa đơn {code}.";
        return RedirectToAction(nameof(Index));
    }
}
