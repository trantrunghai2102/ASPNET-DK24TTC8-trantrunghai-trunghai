using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class AdminHoaDonController : Controller
{
    private readonly AppDbContext _db;

    public AdminHoaDonController(AppDbContext db)
    {
        _db = db;
    }

    // GET /AdminHoaDon
    public async Task<IActionResult> Index(int page = 1, string type = "")
    {
        const int pageSize = 10;

        var query = _db.Orders
            .Include(o => o.Staff)
            .Include(o => o.Invoice)
            .AsQueryable();

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(o => o.OrderType.ToLower() == type.ToLower());
        }

        query = query.OrderByDescending(o => o.CreatedAt);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var invoices = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new HoaDonTomTatViewModel
            {
                InvoiceId   = o.Invoice != null ? o.Invoice.InvoiceId : 0,
                OrderId     = o.OrderId,
                InvoiceCode = o.Invoice != null ? o.Invoice.InvoiceCode : $"ORD-{o.OrderId:D6}",
                Date        = o.Invoice != null ? o.Invoice.PaidAt : o.CreatedAt,
                TotalAmount = o.TotalAmount,
                StaffName   = o.Staff != null ? o.Staff.FullName : null,
                OrderType   = o.OrderType,
                Status      = o.Status
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

        return View("~/Views/Admin/HoaDon/Index.cshtml", viewModel);
    }

    // GET /AdminHoaDon/ChiTiet/{id}  (id = OrderId)
    public async Task<IActionResult> ChiTiet(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Staff)
            .Include(o => o.Invoice)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null) return NotFound();

        var vm = new AdminOrderChiTietViewModel
        {
            OrderId      = order.OrderId,
            InvoiceCode  = order.Invoice?.InvoiceCode ?? $"ORD-{order.OrderId:D6}",
            OrderType    = order.OrderType,
            Status       = order.Status,
            PaymentStatus = order.PaymentStatus,
            TotalAmount  = order.TotalAmount,
            CashGiven    = order.CashGiven,
            ChangeAmount = order.ChangeAmount,
            CreatedAt    = order.CreatedAt,
            UpdatedAt    = order.UpdatedAt,
            PaidAt       = order.Invoice?.PaidAt,
            CustomerName = order.Customer?.FullName,
            CustomerEmail = order.Customer?.Email,
            StaffName    = order.Staff?.FullName,
            RecipientName  = order.RecipientName,
            RecipientPhone = order.RecipientPhone,
            DeliveryAddress = order.DeliveryAddress,
            Notes        = order.Notes,
            Items = order.OrderItems.Select(i => new HoaDonItemViewModel
            {
                ProductName = i.ProductName,
                UnitPrice   = i.UnitPrice,
                Quantity    = i.Quantity,
                Subtotal    = i.Subtotal
            }).ToList()
        };

        return View("~/Views/Admin/HoaDon/ChiTiet.cshtml", vm);
    }

    // POST /AdminHoaDon/Xoa/{id}
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
