using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Staff + "," + UserRoles.Admin)]
public class DonHangOnlineController : Controller
{
    private readonly AppDbContext _db;

    public DonHangOnlineController(AppDbContext db)
    {
        _db = db;
    }

    private static readonly string[] ValidStatuses =
        ["pending", "processing", "shipping", "delivered", "cancelled"];

    // GET /DonHangOnline
    public async Task<IActionResult> Index(string? status, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.Orders
            .Where(o => o.OrderType == "online")
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new DonHangOnlineTomTatViewModel
            {
                OrderId        = o.OrderId,
                CustomerName   = o.Customer != null ? o.Customer.FullName : null,
                RecipientName  = o.RecipientName,
                RecipientPhone = o.RecipientPhone,
                TotalAmount    = o.TotalAmount,
                Status         = o.Status,
                PaymentStatus  = o.PaymentStatus,
                CreatedAt      = o.CreatedAt
            })
            .ToListAsync();

        return View("~/Views/Staff/DonHangOnline/Index.cshtml", new DonHangOnlineListViewModel
        {
            Orders       = orders,
            FilterStatus = status,
            CurrentPage  = page,
            TotalPages   = Math.Max(1, totalPages)
        });
    }

    // GET /DonHangOnline/ChiTiet/{id}
    public async Task<IActionResult> ChiTiet(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Staff)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.OrderType == "online");

        if (order == null) return NotFound();

        var vm = new DonHangOnlineChiTietViewModel
        {
            OrderId         = order.OrderId,
            CustomerName    = order.Customer?.FullName,
            CustomerEmail   = order.Customer?.Email,
            RecipientName   = order.RecipientName,
            RecipientPhone  = order.RecipientPhone,
            DeliveryAddress = order.DeliveryAddress,
            Notes           = order.Notes,
            TotalAmount     = order.TotalAmount,
            Status          = order.Status,
            PaymentStatus   = order.PaymentStatus,
            CreatedAt       = order.CreatedAt,
            UpdatedAt       = order.UpdatedAt,
            StaffName       = order.Staff?.FullName,
            Items           = order.OrderItems.Select(i => new DonHangOnlineItemViewModel
            {
                ProductName = i.ProductName,
                UnitPrice   = i.UnitPrice,
                Quantity    = i.Quantity,
                Subtotal    = i.Subtotal
            }).ToList()
        };

        return View("~/Views/Staff/DonHangOnline/ChiTiet.cshtml", vm);
    }

    // POST /DonHangOnline/CapNhatTrangThai
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(CapNhatTrangThaiInput input)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.OrderId == input.OrderId && o.OrderType == "online");

        if (order == null) return NotFound();

        // Xác nhận thanh toán: delivered + paid
        if (input.NewStatus == "paid")
        {
            order.Status        = "delivered";
            order.PaymentStatus = "paid";
            order.UpdatedAt     =  DateTime.UtcNow;

            // Generate Invoice for online order
            var invoice = new Invoice
            {
                OrderId      = order.OrderId,
                InvoiceCode  = $"HD-{order.OrderId:D6}",
                TotalAmount  = order.TotalAmount,
                CashGiven    = order.TotalAmount,
                ChangeAmount = 0,
                PaidAt       =  DateTime.UtcNow,
                CreatedAt    =  DateTime.UtcNow
            };
            _db.Invoices.Add(invoice);

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xác nhận thanh toán — đơn hàng hoàn tất.";
            return RedirectToAction(nameof(ChiTiet), new { id = input.OrderId });
        }

        if (!ValidStatuses.Contains(input.NewStatus))
        {
            TempData["Error"] = "Trạng thái không hợp lệ.";
            return RedirectToAction(nameof(ChiTiet), new { id = input.OrderId });
        }

        var staffIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var staffId    = int.TryParse(staffIdStr, out var sid) ? sid : 0;
        var now        = DateTime.UtcNow;

        order.Status    = input.NewStatus;
        order.UpdatedAt = now;

        if (input.NewStatus == "processing" && order.StaffId == null && staffId > 0)
            order.StaffId = staffId;

        await _db.SaveChangesAsync();

        var label = input.NewStatus switch
        {
            "processing" => "Đang xử lý",
            "shipping"   => "Đang vận chuyển",
            "delivered"  => "Đã giao",
            "cancelled"  => "Đã hủy",
            _            => input.NewStatus
        };

        TempData["Success"] = $"Cập nhật trạng thái thành «{label}» thành công.";
        return RedirectToAction(nameof(ChiTiet), new { id = input.OrderId });
    }
}