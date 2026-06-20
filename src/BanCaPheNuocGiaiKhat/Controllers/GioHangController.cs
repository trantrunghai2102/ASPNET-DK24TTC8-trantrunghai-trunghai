using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Helpers;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

public class GioHangController : Controller
{
    private readonly AppDbContext _db;

    public GioHangController(AppDbContext db)
    {
        _db = db;
    }

    private int? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim) : null;
    }

    // GET /GioHang/GetCartCount
    [HttpGet]
    public async Task<IActionResult> GetCartCount()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            var count = await _db.CartItems.Where(c => c.UserId == userId.Value).SumAsync(c => c.Quantity);
            return Json(new { count });
        }
        else
        {
            var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
            var count = sessionItems.Sum(s => s.Quantity);
            return Json(new { count });
        }
    }

    // GET /GioHang
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        List<GioHangItemViewModel> items = new();

        if (userId.HasValue)
        {
            items = await _db.CartItems
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
        else
        {
            var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
            if (sessionItems.Any())
            {
                var productIds = sessionItems.Select(s => s.ProductId).ToList();
                var products = await _db.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId);

                items = sessionItems
                    .Where(s => products.ContainsKey(s.ProductId))
                    .Select(s =>
                    {
                        var p = products[s.ProductId];
                        return new GioHangItemViewModel
                        {
                            CartItemId   = s.ProductId, // Use ProductId as dummy CartItemId
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
        }

        return View("~/Views/Customer/GioHang/Index.cshtml", new GioHangViewModel { Items = items });
    }

    // POST /GioHang/Them
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(ThemVaoGioHangInput input)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin không hợp lệ.";
            return RedirectToAction("Detail", "SanPham", new { id = input.ProductId });
        }

        var product = await _db.Products.FindAsync(input.ProductId);
        if (product == null || product.Status != "active")
        {
            TempData["Error"] = "Sản phẩm không tồn tại.";
            return RedirectToAction("Index", "SanPham");
        }

        if (input.Quantity > product.StockQty)
        {
            TempData["Error"] = $"Chỉ còn {product.StockQty} sản phẩm trong kho.";
            return RedirectToAction("Detail", "SanPham", new { slug = product.Slug });
        }

        var userId = GetUserId();
        if (userId.HasValue)
        {
            var existing = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId.Value && c.ProductId == input.ProductId);

            var now = DateTime.UtcNow;
            if (existing != null)
            {
                var newQty = existing.Quantity + input.Quantity;
                if (newQty > product.StockQty)
                {
                    TempData["Error"] = $"Tổng số lượng vượt tồn kho (còn {product.StockQty}).";
                    return RedirectToAction("Detail", "SanPham", new { slug = product.Slug });
                }
                existing.Quantity  = newQty;
                existing.UpdatedAt = now;
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId    = userId.Value,
                    ProductId = input.ProductId,
                    Quantity  = input.Quantity,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
            await _db.SaveChangesAsync();
        }
        else
        {
            var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
            var existing = sessionItems.FirstOrDefault(s => s.ProductId == input.ProductId);
            if (existing != null)
            {
                var newQty = existing.Quantity + input.Quantity;
                if (newQty > product.StockQty)
                {
                    TempData["Error"] = $"Tổng số lượng vượt tồn kho (còn {product.StockQty}).";
                    return RedirectToAction("Detail", "SanPham", new { slug = product.Slug });
                }
                existing.Quantity = newQty;
            }
            else
            {
                sessionItems.Add(new SessionCartItem { ProductId = input.ProductId, Quantity = input.Quantity });
            }
            SessionCartHelper.SaveCart(HttpContext.Session, sessionItems);
        }

        TempData["Success"] = $"Đã thêm «{product.Name}» vào giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    // POST /GioHang/ThemAjax
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemAjax(ThemVaoGioHangInput input)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Thông tin không hợp lệ." });

        var product = await _db.Products.FindAsync(input.ProductId);
        if (product == null || product.Status != "active")
            return Json(new { success = false, message = "Sản phẩm không tồn tại." });

        if (input.Quantity > product.StockQty)
            return Json(new { success = false, message = $"Chỉ còn {product.StockQty} sản phẩm trong kho." });

        var userId = GetUserId();
        int newCount = 0;

        if (userId.HasValue)
        {
            var existing = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId.Value && c.ProductId == input.ProductId);

            var now = DateTime.UtcNow;
            if (existing != null)
            {
                var newQty = existing.Quantity + input.Quantity;
                if (newQty > product.StockQty)
                    return Json(new { success = false, message = $"Tổng số lượng vượt tồn kho (còn {product.StockQty})." });
                
                existing.Quantity  = newQty;
                existing.UpdatedAt = now;
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId    = userId.Value,
                    ProductId = input.ProductId,
                    Quantity  = input.Quantity,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
            await _db.SaveChangesAsync();
            newCount = await _db.CartItems.Where(c => c.UserId == userId.Value).SumAsync(c => c.Quantity);
        }
        else
        {
            var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
            var existing = sessionItems.FirstOrDefault(s => s.ProductId == input.ProductId);
            if (existing != null)
            {
                var newQty = existing.Quantity + input.Quantity;
                if (newQty > product.StockQty)
                    return Json(new { success = false, message = $"Tổng số lượng vượt tồn kho (còn {product.StockQty})." });
                
                existing.Quantity = newQty;
            }
            else
            {
                sessionItems.Add(new SessionCartItem { ProductId = input.ProductId, Quantity = input.Quantity });
            }
            SessionCartHelper.SaveCart(HttpContext.Session, sessionItems);
            newCount = sessionItems.Sum(s => s.Quantity);
        }

        return Json(new { success = true, count = newCount, message = $"Đã thêm {product.Name} vào giỏ hàng." });
    }

    // POST /GioHang/CapNhat
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(CapNhatGioHangInput input)
    {
        var userId = GetUserId();

        if (userId.HasValue)
        {
            var item = await _db.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartItemId == input.CartItemId && c.UserId == userId.Value);

            if (item == null) return NotFound();

            if (item.Product != null && input.Quantity > item.Product.StockQty)
            {
                TempData["Error"] = $"Chỉ còn {item.Product.StockQty} sản phẩm trong kho.";
                return RedirectToAction(nameof(Index));
            }

            item.Quantity  = input.Quantity;
            item.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        else
        {
            var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
            var item = sessionItems.FirstOrDefault(s => s.ProductId == input.CartItemId); // For guest, CartItemId is ProductId
            if (item != null)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null && input.Quantity > product.StockQty)
                {
                    TempData["Error"] = $"Chỉ còn {product.StockQty} sản phẩm trong kho.";
                    return RedirectToAction(nameof(Index));
                }
                item.Quantity = input.Quantity;
                SessionCartHelper.SaveCart(HttpContext.Session, sessionItems);
            }
        }

        TempData["Success"] = "Đã cập nhật số lượng.";
        return RedirectToAction(nameof(Index));
    }

    // POST /GioHang/Xoa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int cartItemId)
    {
        var userId = GetUserId();

        if (userId.HasValue)
        {
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId.Value);

            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
        }
        else
        {
            var sessionItems = SessionCartHelper.GetCart(HttpContext.Session);
            var item = sessionItems.FirstOrDefault(s => s.ProductId == cartItemId);
            if (item != null)
            {
                sessionItems.Remove(item);
                SessionCartHelper.SaveCart(HttpContext.Session, sessionItems);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
        }

        return RedirectToAction(nameof(Index));
    }
}