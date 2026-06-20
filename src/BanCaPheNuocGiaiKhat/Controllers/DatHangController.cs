using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Helpers;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

public class DatHangController : Controller
{
    private readonly AppDbContext _db;

    public DatHangController(AppDbContext db)
    {
        _db = db;
    }

    private int? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim) : null;
    }

    private async Task<List<GioHangItemViewModel>> LoadCartItemsAsync(int? userId)
    {
        if (userId.HasValue)
        {
            return await _db.CartItems
                .Where(c => c.UserId == userId.Value)
                .Include(c => c.Product)
                .Select(c => new GioHangItemViewModel
                {
                    CartItemId   = c.CartItemId,
                    ProductId    = c.ProductId,
                    ProductName  = c.Product != null ? c.Product.Name : "(Sản phẩm không còn)",
                    ThumbnailUrl = c.Product != null ? c.Product.ThumbnailUrl : null,
                    UnitPrice    = c.Product != null ? (c.Product.PromotionPrice ?? c.Product.BasePrice) : 0,
                    Quantity     = c.Quantity,
                    StockQty     = c.Product != null ? c.Product.StockQty : 0
                })
                .ToListAsync();
        }

        var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
        if (!sessionItems.Any()) return new();

        var productIds = sessionItems.Select(s => s.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId);

        return sessionItems
            .Where(s => products.ContainsKey(s.ProductId))
            .Select(s =>
            {
                var p = products[s.ProductId];
                return new GioHangItemViewModel
                {
                    CartItemId   = s.ProductId,
                    ProductId    = s.ProductId,
                    ProductName  = p.Name,
                    ThumbnailUrl = p.ThumbnailUrl,
                    UnitPrice    = p.PromotionPrice ?? p.BasePrice,
                    Quantity     = s.Quantity,
                    StockQty     = p.StockQty
                };
            })
            .ToList();
    }

    // GET /DatHang/ThongTin
    public async Task<IActionResult> ThongTin()
    {
        var userId = GetUserId();
        var cartItems = await LoadCartItemsAsync(userId);

        if (!cartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.";
            return RedirectToAction("Index", "GioHang");
        }

        var vm = new DatHangViewModel { Items = cartItems };

        if (userId.HasValue)
        {
            var user = await _db.Users.FindAsync(userId.Value);
            vm.RecipientName   = user?.FullName ?? string.Empty;
            vm.RecipientPhone  = user?.Phone ?? string.Empty;
            vm.DeliveryAddress = user?.Address ?? string.Empty;
        }

        return View("~/Views/Customer/DatHang/ThongTin.cshtml", vm);
    }

    // POST /DatHang/XacNhan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(DatHangViewModel input)
    {
        var userId = GetUserId();
        var cartItems = await LoadCartItemsAsync(userId);

        if (!cartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống.";
            return RedirectToAction("Index", "GioHang");
        }

        if (!ModelState.IsValid)
        {
            input.Items = cartItems;
            return View("~/Views/Customer/DatHang/ThongTin.cshtml", input);
        }

        // Kiểm tra tồn kho
        var stockErrors = cartItems
            .Where(c => c.Quantity > c.StockQty)
            .Select(c => $"{c.ProductName} (còn {c.StockQty})")
            .ToList();

        if (stockErrors.Any())
        {
            TempData["Error"] = "Một số sản phẩm không đủ tồn kho: " + string.Join(", ", stockErrors) + ". Vui lòng cập nhật giỏ hàng.";
            return RedirectToAction("Index", "GioHang");
        }

        var now = DateTime.UtcNow;
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
        foreach (var item in cartItems.Where(c => c.ProductId.HasValue))
        {
            var subtotal = item.UnitPrice * item.Quantity;
            total += subtotal;
            order.OrderItems.Add(new OrderItem
            {
                ProductId   = item.ProductId!.Value,
                ProductName = item.ProductName,
                UnitPrice   = item.UnitPrice,
                Quantity    = item.Quantity,
                Subtotal    = subtotal
            });
        }
        order.TotalAmount = total;

        _db.Orders.Add(order);

        if (userId.HasValue)
        {
            var dbCart = await _db.CartItems.Where(c => c.UserId == userId.Value).ToListAsync();
            _db.CartItems.RemoveRange(dbCart);
        }
        else
        {
            SessionCartHelper.ClearCart(HttpContext.Session);
        }

        await _db.SaveChangesAsync();

        if (!userId.HasValue)
            HttpContext.Session.SetInt32("GuestOrderId", order.OrderId);

        return RedirectToAction(nameof(ThanhCong), new { orderId = order.OrderId });
    }

    // GET /DatHang/DonHangCuaToi
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<IActionResult> DonHangCuaToi(string? status, int page = 1)
    {
        const int pageSize = 10;
        var userId = GetUserId()!.Value;

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

        return View("~/Views/Customer/DatHang/DonHangCuaToi.cshtml", new DonHangCuaToiListViewModel
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
        Order? order;

        if (userId.HasValue)
        {
            order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == userId.Value && o.OrderType == "online");
        }
        else
        {
            var sessionOrderId = HttpContext.Session.GetInt32("GuestOrderId");
            if (sessionOrderId != id) return NotFound();

            order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == null);
        }

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

        return View("~/Views/Customer/DatHang/TheoDoi.cshtml", vm);
    }

    // GET /DatHang/ThanhCong/{orderId}
    public async Task<IActionResult> ThanhCong(int orderId)
    {
        var userId = GetUserId();
        Order? order;

        if (userId.HasValue)
        {
            order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == userId.Value);
        }
        else
        {
            // Guest: xác minh qua session để tránh order enumeration
            var sessionOrderId = HttpContext.Session.GetInt32("GuestOrderId");
            if (sessionOrderId != orderId) return NotFound();

            order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == null);
        }

        if (order == null) return NotFound();

        var vm = new DatHangThanhCongViewModel
        {
            OrderId         = order.OrderId,
            TotalAmount     = order.TotalAmount,
            RecipientName   = order.RecipientName ?? string.Empty,
            RecipientPhone  = order.RecipientPhone ?? string.Empty,
            DeliveryAddress = order.DeliveryAddress ?? string.Empty,
            CreatedAt       = order.CreatedAt,
            IsGuest         = !userId.HasValue,
            Items           = order.OrderItems.Select(i => new DatHangItemViewModel
            {
                ProductName = i.ProductName,
                UnitPrice   = i.UnitPrice,
                Quantity    = i.Quantity,
                Subtotal    = i.Subtotal
            }).ToList()
        };

        return View("~/Views/Customer/DatHang/ThanhCong.cshtml", vm);
    }
}