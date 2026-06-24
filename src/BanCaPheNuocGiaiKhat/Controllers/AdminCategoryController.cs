using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class AdminCategoryController : Controller
{
    private readonly AppDbContext _db;

    public AdminCategoryController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        const int pageSize = 10;
        var query = _db.Categories
            .Include(c => c.Parent)
            .Include(c => c.Products)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

        query = query.OrderBy(c => c.Name);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var categories = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryViewModel
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                ParentId = c.ParentId,
                ParentName = c.Parent != null ? c.Parent.Name : null,
                ProductCount = c.Products.Count
            })
            .ToListAsync();

        var vm = new CategoryListViewModel
        {
            Categories = categories,
            CurrentPage = page,
            TotalPages = Math.Max(1, totalPages),
            SearchTerm = search
        };

        return View("~/Views/Admin/Category/Index.cshtml", vm);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new CategoryViewModel();
        await PopulateParentCategories(vm);
        return View("~/Views/Admin/Category/Create.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            int newDepth = GetCategoryDepth(model.ParentId);
            if (newDepth > 3)
            {
                ModelState.AddModelError("ParentId", "Không thể tạo danh mục quá 3 cấp.");
            }
            else
            {
                var exists = await _db.Categories.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());
            if (exists)
            {
                ModelState.AddModelError("Name", "Danh mục này đã tồn tại.");
            }
            else
            {
                var category = new Category
                {
                    Name = model.Name,
                    ParentId = model.ParentId
                };
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm danh mục thành công.";
                return RedirectToAction(nameof(Index));
            }
            }
        }

        await PopulateParentCategories(model);
        return View("~/Views/Admin/Category/Create.cshtml", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        var vm = new CategoryViewModel
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            ParentId = category.ParentId
        };

        await PopulateParentCategories(vm, id);
        return View("~/Views/Admin/Category/Edit.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryViewModel model)
    {
        if (id != model.CategoryId) return BadRequest();

        if (ModelState.IsValid)
        {
            int newDepth = GetCategoryDepth(model.ParentId);
            int maxDescendantDepth = GetMaxDescendantDepth(id);
            if (newDepth + maxDescendantDepth > 3)
            {
                ModelState.AddModelError("ParentId", "Danh mục này hoặc danh mục con của nó sẽ vượt quá giới hạn 3 cấp nếu chuyển vào đây.");
            }
            else
            {
                var exists = await _db.Categories.AnyAsync(c => c.CategoryId != id && c.Name.ToLower() == model.Name.ToLower());
            if (exists)
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
            }
            else
            {
                var category = await _db.Categories.FindAsync(id);
                if (category == null) return NotFound();

                if (model.ParentId == id)
                {
                    ModelState.AddModelError("ParentId", "Danh mục không thể là cha của chính nó.");
                }
                else
                {
                    category.Name = model.Name;
                    category.ParentId = model.ParentId;
                    
                    await _db.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật danh mục thành công.";
                    return RedirectToAction(nameof(Index));
                }
            }
            }
        }

        await PopulateParentCategories(model, id);
        return View("~/Views/Admin/Category/Edit.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories
            .Include(c => c.Products)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return NotFound();

        if (category.Products.Any() || category.Children.Any())
        {
            TempData["Error"] = $"Không thể xóa danh mục '{category.Name}' vì đang có sản phẩm hoặc danh mục con liên kết.";
            return RedirectToAction(nameof(Index));
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa danh mục '{category.Name}'.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateParentCategories(CategoryViewModel vm, int? currentId = null)
    {
        var query = _db.Categories.AsQueryable();
        
        if (currentId.HasValue)
        {
            query = query.Where(c => c.CategoryId != currentId.Value);
        }

        var parents = await query.OrderBy(c => c.Name).ToListAsync();
        
        vm.ParentCategories = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "-- Không có (Danh mục gốc) --" }
        };

        vm.ParentCategories.AddRange(parents.Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = c.Name
        }));
    }

    private int GetCategoryDepth(int? parentId)
    {
        int depth = 1;
        var currentParentId = parentId;
        while (currentParentId.HasValue)
        {
            var parent = _db.Categories.FirstOrDefault(c => c.CategoryId == currentParentId.Value);
            if (parent != null)
            {
                depth++;
                currentParentId = parent.ParentId;
            }
            else
            {
                break;
            }
        }
        return depth;
    }

    private int GetMaxDescendantDepth(int categoryId)
    {
        var children = _db.Categories.Where(c => c.ParentId == categoryId).ToList();
        if (!children.Any()) return 0;
        
        return children.Max(c => GetMaxDescendantDepth(c.CategoryId)) + 1;
    }
}
