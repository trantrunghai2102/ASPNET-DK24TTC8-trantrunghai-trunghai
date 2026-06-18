using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Staff)]
public class BanHangController : Controller
{
    private readonly AppDbContext _db;

    public BanHangController(AppDbContext db)
    {
        _db = db;
    }

    // GET /BanHang
    public async Task<IActionResult> Index()
    {
        var products = await _db.Products
            .Where(p => p.Status == "active")
            .Include(p => p.Category)
            .Select(p => new SanPhamChonViewModel
            {
                ProductId    = p.ProductId,
                Name         = p.Name,
                Price        = p.PromotionPrice ?? p.BasePrice,
                ThumbnailUrl = p.ThumbnailUrl,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View("~/Views/Staff/BanHang/Index.cshtml", new BanHangViewModel { Products = products });
    }

    // POST /BanHang/TaoHoaDon
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaoHoaDon(TaoHoaDonInput input)
    {
        if (input.Items == null || !input.Items.Any(i => i.Quantity > 0))
        {
            TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        var productIds = input.Items
            .Where(i => i.Quantity > 0)
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        var products = await _db.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId);

        if (products.Count == 0)
        {
            TempData["Error"] = "Không tìm thấy sản phẩm hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        // Kiểm tra tồn kho
        var stockErrors = input.Items
            .Where(i => i.Quantity > 0 && products.TryGetValue(i.ProductId, out var p) && i.Quantity > p.StockQty)
            .Select(i => $"{products[i.ProductId].Name} (còn {products[i.ProductId].StockQty})")
            .ToList();

        if (stockErrors.Any())
        {
            TempData["Error"] = "Số lượng vượt tồn kho: " + string.Join(", ", stockErrors) + ".";
            return RedirectToAction(nameof(Index));
        }

        _ = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var staffId);
        var now = DateTime.UtcNow;

        var order = new Order
        {
            StaffId   = staffId > 0 ? staffId : null,
            Status    = "pending",
            CreatedAt = now,
            UpdatedAt = now
        };

        decimal total = 0;
        foreach (var item in input.Items.Where(i => i.Quantity > 0))
        {
            if (!products.TryGetValue(item.ProductId, out var product)) continue;

            var price    = product.PromotionPrice ?? product.BasePrice;
            var subtotal = price * item.Quantity;
            total += subtotal;

            order.OrderItems.Add(new OrderItem
            {
                ProductId   = product.ProductId,
                ProductName = product.Name,
                UnitPrice   = price,
                Quantity    = item.Quantity,
                Subtotal    = subtotal
            });
        }

        if (!order.OrderItems.Any())
        {
            TempData["Error"] = "Không có sản phẩm hợp lệ trong đơn hàng.";
            return RedirectToAction(nameof(Index));
        }

        order.TotalAmount = total;

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(ThanhToan), new { orderId = order.OrderId });
    }

    // GET /BanHang/ThanhToan/{orderId}
    public async Task<IActionResult> ThanhToan(int orderId)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Status == "pending");

        if (order == null)
            return NotFound();

        var vm = new ThanhToanViewModel
        {
            OrderId     = order.OrderId,
            TotalAmount = order.TotalAmount,
            Items       = order.OrderItems.Select(i => new ThanhToanItemViewModel
            {
                ProductName = i.ProductName,
                UnitPrice   = i.UnitPrice,
                Quantity    = i.Quantity,
                Subtotal    = i.Subtotal
            }).ToList()
        };

        return View("~/Views/Staff/BanHang/ThanhToan.cshtml", vm);
    }

    // POST /BanHang/XacNhanThanhToan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhanThanhToan(int orderId, decimal cashGiven)
    {
        // Chuyển đổi từ tiền VND nhập vào (ví dụ 100000) về đơn vị nghìn VND trong DB (ví dụ 100.00)
        cashGiven = cashGiven / 1000m;

        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Status == "pending");

        if (order == null)
            return NotFound();

        if (cashGiven < order.TotalAmount)
        {
            TempData["Error"] = "Số tiền khách đưa không đủ để thanh toán.";
            return RedirectToAction(nameof(ThanhToan), new { orderId });
        }

        var now          = DateTime.UtcNow;
        var changeAmount = cashGiven - order.TotalAmount;

        order.CashGiven    = cashGiven;
        order.ChangeAmount = changeAmount;
        order.Status       = "paid";
        order.UpdatedAt    = now;

        var invoice = new Invoice
        {
            OrderId      = order.OrderId,
            InvoiceCode  = $"HD-{order.OrderId:D6}",
            TotalAmount  = order.TotalAmount,
            CashGiven    = cashGiven,
            ChangeAmount = changeAmount,
            PaidAt       = now,
            CreatedAt    = now
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        return RedirectToAction("ChiTiet", "StaffHoaDon", new { id = invoice.InvoiceId });
    }
}