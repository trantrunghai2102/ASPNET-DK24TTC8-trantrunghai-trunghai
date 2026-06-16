using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
public class HoaDonController : Controller
{
    private readonly AppDbContext _db;

    public HoaDonController(AppDbContext db)
    {
        _db = db;
    }

    // GET /HoaDon
    public async Task<IActionResult> Index()
    {
        var invoices = await _db.Invoices
            .Include(i => i.Order).ThenInclude(o => o.Staff)
            .OrderByDescending(i => i.PaidAt)
            .Select(i => new HoaDonTomTatViewModel
            {
                InvoiceId   = i.InvoiceId,
                InvoiceCode = i.InvoiceCode,
                PaidAt      = i.PaidAt,
                TotalAmount = i.TotalAmount,
                StaffName   = i.Order.Staff != null ? i.Order.Staff.FullName : null
            })
            .ToListAsync();

        var pending = await _db.Orders.CountAsync(o => o.Status == "pending");
        var processing = await _db.Orders.CountAsync(o => o.Status == "processing");
        var completed = await _db.Orders.CountAsync(o => o.Status == "completed");

        return View(new DanhSachHoaDonViewModel 
        { 
            Invoices = invoices,
            PendingOrders = pending,
            ProcessingOrders = processing,
            CompletedOrders = completed,
            TotalOrders = pending + processing + completed
        });
    }

    // POST /HoaDon/Xoa/{id}
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

    // GET /HoaDon/ChiTiet/{id}
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

        return View(vm);
    }
}