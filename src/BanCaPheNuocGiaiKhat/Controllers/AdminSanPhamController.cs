using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Staff)]
public class AdminSanPhamController : Controller
{
    private readonly AppDbContext _db;

    public AdminSanPhamController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
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
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _db.Categories.ToListAsync();
        return View("~/Views/Admin/SanPham/Create.cshtml", new AdminProductCreateViewModel());
    }

    [HttpPost]
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
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminProductEditViewModel model, List<IFormFile> images)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
        return RedirectToAction(nameof(Index));
    }
}
