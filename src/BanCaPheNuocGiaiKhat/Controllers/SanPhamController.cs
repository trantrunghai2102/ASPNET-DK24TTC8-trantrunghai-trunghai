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

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
    public async Task<IActionResult> AdminIndex()
    {
        var products = await _db.Products
            .Where(p => p.Status != "deleted")
            .Include(p => p.Category)
            .OrderByDescending(p => p.ProductId)
            .Select(p => new AdminProductListViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Slug = p.Slug,
                CategoryName = p.Category != null ? p.Category.Name : "—",
                BasePrice = p.BasePrice,
                StockQty = p.StockQty,
                Status = p.Status,
                ThumbnailUrl = p.ThumbnailUrl
            })
            .ToListAsync();
        
        return View("~/Views/Admin/SanPham/Index.cshtml", products);
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _db.Categories.ToListAsync();
        return View("~/Views/Admin/SanPham/Create.cshtml", new AdminProductCreateViewModel());
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminProductCreateViewModel model, List<IFormFile> images)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View("~/Views/Admin/SanPham/Create.cshtml", model);
        }

        var product = new BanCaPheNuocGiaiKhat.Models.Entities.Product
        {
            Name = model.Name,
            Slug = model.Slug,
            CategoryId = model.CategoryId,
            ShortDesc = model.ShortDesc,
            DetailDesc = model.DetailDesc,
            BasePrice = model.BasePrice,
            PromotionPrice = model.PromotionPrice,
            StockQty = model.StockQty,
            RoastLevel = model.RoastLevel,
            Region = model.Region,
            GrindType = model.GrindType,
            Status = model.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        if (images != null && images.Any())
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);
            
            int displayOrder = 1;
            foreach (var file in images)
            {
                if (file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var newFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, newFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    var imgUrl = $"/uploads/products/{newFileName}";
                    
                    if (displayOrder == 1)
                    {
                        product.ThumbnailUrl = imgUrl;
                    }
                    
                    _db.ProductImages.Add(new BanCaPheNuocGiaiKhat.Models.Entities.ProductImage
                    {
                        ProductId = product.ProductId,
                        Url = imgUrl,
                        SortOrder = displayOrder++
                    });
                }
            }
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = $"Đã tạo sản phẩm {product.Name} thành công.";
        return RedirectToAction(nameof(AdminIndex));
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Categories = await _db.Categories.ToListAsync();

        var model = new AdminProductEditViewModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Slug = product.Slug,
            CategoryId = product.CategoryId ?? 0,
            ShortDesc = product.ShortDesc,
            DetailDesc = product.DetailDesc,
            BasePrice = product.BasePrice,
            PromotionPrice = product.PromotionPrice,
            StockQty = product.StockQty,
            RoastLevel = product.RoastLevel,
            Region = product.Region,
            GrindType = product.GrindType,
            Status = product.Status,
            ExistingImages = product.ProductImages
                .OrderBy(img => img.SortOrder)
                .Select(img => new ProductImageViewModel
                {
                    ImageId = img.ImageId,
                    Url = img.Url
                })
                .ToList()
        };

        return View("~/Views/Admin/SanPham/Edit.cshtml", model);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminProductEditViewModel model, List<IFormFile> images)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            // Re-load existing images from database
            var productImages = await _db.ProductImages
                .Where(img => img.ProductId == model.ProductId)
                .OrderBy(img => img.SortOrder)
                .Select(img => new ProductImageViewModel
                {
                    ImageId = img.ImageId,
                    Url = img.Url
                })
                .ToListAsync();
            model.ExistingImages = productImages;
            return View("~/Views/Admin/SanPham/Edit.cshtml", model);
        }

        var product = await _db.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

        if (product == null)
        {
            return NotFound();
        }

        product.Name = model.Name;
        product.Slug = model.Slug;
        product.CategoryId = model.CategoryId;
        product.ShortDesc = model.ShortDesc;
        product.DetailDesc = model.DetailDesc;
        product.BasePrice = model.BasePrice;
        product.PromotionPrice = model.PromotionPrice;
        product.StockQty = model.StockQty;
        product.RoastLevel = model.RoastLevel;
        product.Region = model.Region;
        product.GrindType = model.GrindType;
        product.Status = model.Status;
        product.UpdatedAt = DateTime.UtcNow;

        // Delete selected images
        if (model.DeleteImageIds != null && model.DeleteImageIds.Any())
        {
            var imagesToDelete = product.ProductImages
                .Where(img => model.DeleteImageIds.Contains(img.ImageId))
                .ToList();

            foreach (var img in imagesToDelete)
            {
                var relativePath = img.Url.TrimStart('/');
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                if (System.IO.File.Exists(physicalPath))
                {
                    try
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                    catch { /* ignore */ }
                }
                _db.ProductImages.Remove(img);
            }
        }

        // Upload new images
        if (images != null && images.Any())
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);

            int displayOrder = product.ProductImages.Any() 
                ? product.ProductImages.Max(img => img.SortOrder) + 1 
                : 1;

            foreach (var file in images)
            {
                if (file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var newFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var imgUrl = $"/uploads/products/{newFileName}";

                    var newImg = new BanCaPheNuocGiaiKhat.Models.Entities.ProductImage
                    {
                        ProductId = product.ProductId,
                        Url = imgUrl,
                        SortOrder = displayOrder++
                    };
                    _db.ProductImages.Add(newImg);
                    product.ProductImages.Add(newImg);
                }
            }
        }

        await _db.SaveChangesAsync();

        // Update thumbnail url based on the remaining image with lowest SortOrder
        var remainingImages = await _db.ProductImages
            .Where(img => img.ProductId == product.ProductId)
            .OrderBy(img => img.SortOrder)
            .ToListAsync();

        if (remainingImages.Any())
        {
            product.ThumbnailUrl = remainingImages.First().Url;
        }
        else
        {
            product.ThumbnailUrl = null;
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã cập nhật sản phẩm {product.Name} thành công.";
        return RedirectToAction(nameof(AdminIndex));
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Status = "deleted";
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa sản phẩm {product.Name} thành công.";
        return RedirectToAction(nameof(AdminIndex));
    }
}
