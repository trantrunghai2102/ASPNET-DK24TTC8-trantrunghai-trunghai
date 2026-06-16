using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Customer)]
public class DatHangController : Controller
{
    private readonly AppDbContext _db;

    public DatHangController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    // GET /DatHang/ThongTin
    public async Task<IActionResult> ThongTin()
    {
        var userId = GetUserId();

        var cartItems = await _db.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        if (!cartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.";
            return RedirectToAction("Index", "GioHang");
        }

        var user = await _db.Users.FindAsync(userId);

        var vm = new DatHangViewModel
        {
            Items = cartItems.Select(c => new GioHangItemViewModel
            {
                CartItemId   = c.CartItemId,
                ProductId    = c.ProductId,
                ProductName  = c.Product?.Name ?? "(Sản phẩm không còn)",
                ThumbnailUrl = c.Product?.ThumbnailUrl,
                UnitPrice    = c.Product != null ? (c.Product.PromotionPrice ?? c.Product.BasePrice) : 0,
                Quantity     = c.Quantity,
                StockQty     = c.Product?.StockQty ?? 0
            }).ToList(),
            RecipientName   = user?.FullName ?? string.Empty,
            RecipientPhone  = user?.Phone ?? string.Empty,
            DeliveryAddress = user?.Address ?? string.Empty
        };

        return View(vm);
    }

    // POST /DatHang/XacNhan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(DatHangViewModel input)
    {
        var userId = GetUserId();

        var cartItems = await _db.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        if (!cartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống.";
            return RedirectToAction("Index", "GioHang");
        }

        if (!ModelState.IsValid)
        {
            input.Items = cartItems.Select(c => new GioHangItemViewModel
            {
                CartItemId   = c.CartItemId,
                ProductId    = c.ProductId,
                ProductName  = c.Product?.Name ?? "(Sản phẩm không còn)",
                ThumbnailUrl = c.Product?.ThumbnailUrl,
                UnitPrice    = c.Product != null ? (c.Product.PromotionPrice ?? c.Product.BasePrice) : 0,
                Quantity     = c.Quantity,
                StockQty     = c.Product?.StockQty ?? 0
            }).ToList();
            return View("ThongTin", input);
        }

        // B9: Kiểm tra tồn kho
        var stockErrors = cartItems
            .Where(c => c.Product != null && c.Quantity > c.Product.StockQty)
            .Select(c => $"{c.Product!.Name} (còn {c.Product.StockQty})")
            .ToList();

        if (stockErrors.Any())
        {
            TempData["Error"] = "Một số sản phẩm không đủ tồn kho: " + string.Join(", ", stockErrors) + ". Vui lòng cập nhật giỏ hàng.";
            return RedirectToAction("Index", "GioHang");
        }

        var now = DateTime.UtcNow;

        // B11: Tạo đơn hàng với trạng thái Pending
        var order = new Order
        {
            CustomerId      = userId,
            OrderType       = "online",
            Status          = "pending",
            PaymentStatus   = "unpaid",
            RecipientName   = input.RecipientName,
            RecipientPhone  = input.RecipientPhone,
            DeliveryAddress = input.DeliveryAddress,
            Notes           = input.Notes,
            CreatedAt       = now,
            UpdatedAt       = now
        };

        decimal total = 0;
        foreach (var ci in cartItems.Where(c => c.Product != null))
        {
            var price    = ci.Product!.PromotionPrice ?? ci.Product.BasePrice;
            var subtotal = price * ci.Quantity;
            total += subtotal;

            order.OrderItems.Add(new OrderItem
            {
                ProductId   = ci.ProductId,
                ProductName = ci.Product.Name,
                UnitPrice   = price,
                Quantity    = ci.Quantity,
                Subtotal    = subtotal
            });
        }

        order.TotalAmount = total;

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cartItems);

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(ThanhCong), new { orderId = order.OrderId });
    }

    // GET /DatHang/DonHangCuaToi
    public async Task<IActionResult> DonHangCuaToi(string? status, int page = 1)
    {
        const int pageSize = 10;
        var userId = GetUserId();

        var query = _db.Orders
            .Where(o => o.CustomerId == userId && o.OrderType == "online")
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
            .Select(o => new DonHangCuaToiTomTatViewModel
            {
                OrderId       = o.OrderId,
                TotalAmount   = o.TotalAmount,
                Status        = o.Status,
                PaymentStatus = o.PaymentStatus,
                CreatedAt     = o.CreatedAt,
                ItemCount     = o.OrderItems.Count
            })
            .ToListAsync();

        return View(new DonHangCuaToiListViewModel
        {
            Orders       = orders,
            FilterStatus = status,
            CurrentPage  = page,
            TotalPages   = Math.Max(1, totalPages)
        });
    }

    // GET /DatHang/TheoDoi/{id}
    public async Task<IActionResult> TheoDoi(int id)
    {
        var userId = GetUserId();

        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == userId && o.OrderType == "online");

        if (order == null) return NotFound();

        var vm = new DonHangCuaToiChiTietViewModel
        {
            OrderId         = order.OrderId,
            RecipientName   = order.RecipientName,
            RecipientPhone  = order.RecipientPhone,
            DeliveryAddress = order.DeliveryAddress,
            Notes           = order.Notes,
            TotalAmount     = order.TotalAmount,
            Status          = order.Status,
            PaymentStatus   = order.PaymentStatus,
            CreatedAt       = order.CreatedAt,
            UpdatedAt       = order.UpdatedAt,
            Items           = order.OrderItems.Select(i => new DatHangItemViewModel
            {
                ProductName = i.ProductName,
                UnitPrice   = i.UnitPrice,
                Quantity    = i.Quantity,
                Subtotal    = i.Subtotal
            }).ToList()
        };

        return View(vm);
    }

    // GET /DatHang/ThanhCong/{orderId}
    public async Task<IActionResult> ThanhCong(int orderId)
    {
        var userId = GetUserId();

        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == userId);

        if (order == null) return NotFound();

        var vm = new DatHangThanhCongViewModel
        {
            OrderId         = order.OrderId,
            TotalAmount     = order.TotalAmount,
            RecipientName   = order.RecipientName ?? string.Empty,
            RecipientPhone  = order.RecipientPhone ?? string.Empty,
            DeliveryAddress = order.DeliveryAddress ?? string.Empty,
            CreatedAt       = order.CreatedAt,
            Items           = order.OrderItems.Select(i => new DatHangItemViewModel
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