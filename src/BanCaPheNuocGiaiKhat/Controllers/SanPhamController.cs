using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

public class SanPhamController : Controller
{
    private readonly AppDbContext _db;

    public SanPhamController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? categoryId,
        string? search,
        string? sortBy,
        int page = 1)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();

        var query = _db.Products.Where(p => p.Status == "active");

        // Category Filter
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            var targetIds = new List<int> { categoryId.Value };
            
            var children = categories.Where(c => c.ParentId == categoryId.Value).Select(c => c.CategoryId).ToList();
            targetIds.AddRange(children);
            
            foreach (var childId in children)
            {
                var grandChildren = categories.Where(c => c.ParentId == childId).Select(c => c.CategoryId).ToList();
                targetIds.AddRange(grandChildren);
            }
            
            query = query.Where(p => p.CategoryId.HasValue && targetIds.Contains(p.CategoryId.Value));
        }

        // Search Filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchClean = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchClean) || 
                                     (p.ShortDesc != null && p.ShortDesc.ToLower().Contains(searchClean)));
        }

        // Sorting
        query = sortBy switch
        {
            "price-asc" => query.OrderBy(p => p.BasePrice),
            "price-desc" => query.OrderByDescending(p => p.BasePrice),
            "popularity" => query.OrderByDescending(p => p.ViewCount),
            _ => query.OrderByDescending(p => p.ViewCount) 
        };

        // Pagination
        int pageSize = 10; 
        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var productsData = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductCardViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Slug = p.Slug,
                BasePrice = p.BasePrice,
                PromotionPrice = p.PromotionPrice,
                ShortDesc = p.ShortDesc,
                ThumbnailUrl = p.ThumbnailUrl,
                IsNew = p.ProductId == 1, 
                IsBestseller = p.ProductId == 3 
            })
            .ToListAsync();

        var model = new ProductListViewModel
        {
            Products = productsData,
            Categories = categories,
            SelectedCategoryId = categoryId,
            SearchQuery = search,
            SortBy = sortBy ?? "popularity",
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(model);
    }

    [HttpGet("SanPham/Detail/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == "active");

        if (product == null)
        {
            return NotFound();
        }

        // Increase view count
        product.ViewCount++;
        await _db.SaveChangesAsync();

        // Fetch related products (same category, different product, max 4)
        var relatedProducts = await _db.Products
            .Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId && p.Status == "active")
            .Take(4)
            .Select(p => new ProductCardViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Slug = p.Slug,
                BasePrice = p.BasePrice,
                PromotionPrice = p.PromotionPrice,
                ShortDesc = p.ShortDesc,
                ThumbnailUrl = p.ThumbnailUrl,
                IsNew = p.ProductId == 1,
                IsBestseller = p.ProductId == 3
            })
            .ToListAsync();

        var viewModel = new ProductDetailViewModel
        {
            Product = product,
            RelatedProducts = relatedProducts
        };

        return View(viewModel);
    }
}
