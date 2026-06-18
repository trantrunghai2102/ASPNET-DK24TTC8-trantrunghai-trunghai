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
        List<string>? roast,
        List<string>? region,
        List<string>? grind,
        int page = 1)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();

        var query = _db.Products.Where(p => p.Status == "active");

        // Category Filter
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Search Filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchClean = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchClean) || 
                                     (p.ShortDesc != null && p.ShortDesc.ToLower().Contains(searchClean)));
        }

        // Roast level Filter
        if (roast != null && roast.Any())
        {
            query = query.Where(p => p.RoastLevel != null && roast.Contains(p.RoastLevel));
        }

        // Region Filter
        if (region != null && region.Any())
        {
            query = query.Where(p => p.Region != null && region.Contains(p.Region));
        }

        // Grind type Filter
        if (grind != null && grind.Any())
        {
            query = query.Where(p => p.GrindType != null && grind.Contains(p.GrindType));
        }

        // Sorting
        query = sortBy switch
        {
            "price-asc" => query.OrderBy(p => p.BasePrice),
            "price-desc" => query.OrderByDescending(p => p.BasePrice),
            "popularity" => query.OrderByDescending(p => p.ViewCount),
            _ => query.OrderByDescending(p => p.ViewCount) // default to popularity
        };

        // Pagination
        int pageSize = 3; 
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
                RoastLevel = p.RoastLevel,
                Region = p.Region,
                GrindType = p.GrindType,
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
            SelectedRoastLevels = roast ?? new List<string>(),
            SelectedRegions = region ?? new List<string>(),
            SelectedGrindTypes = grind ?? new List<string>(),
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
                RoastLevel = p.RoastLevel,
                Region = p.Region,
                GrindType = p.GrindType,
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
