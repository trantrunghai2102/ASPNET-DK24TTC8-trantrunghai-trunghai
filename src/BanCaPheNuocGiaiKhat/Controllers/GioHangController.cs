using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Customer)]
public class GioHangController : Controller
{
    private readonly AppDbContext _db;

    public GioHangController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    // GET /GioHang
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();

        var items = await _db.CartItems
            .Where(c => c.UserId == userId)
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
        var existing = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == input.ProductId);

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
                UserId    = userId,
                ProductId = input.ProductId,
                Quantity  = input.Quantity,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = $"Đã thêm «{product.Name}» vào giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    // POST /GioHang/CapNhat
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(CapNhatGioHangInput input)
    {
        var userId = GetUserId();
        var item = await _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.CartItemId == input.CartItemId && c.UserId == userId);

        if (item == null) return NotFound();

        if (item.Product != null && input.Quantity > item.Product.StockQty)
        {
            TempData["Error"] = $"Chỉ còn {item.Product.StockQty} sản phẩm trong kho.";
            return RedirectToAction(nameof(Index));
        }

        item.Quantity  = input.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật số lượng.";
        return RedirectToAction(nameof(Index));
    }

    // POST /GioHang/Xoa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int cartItemId)
    {
        var userId = GetUserId();
        var item = await _db.CartItems
            .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);

        if (item != null)
        {
            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
        }

        return RedirectToAction(nameof(Index));
    }
}