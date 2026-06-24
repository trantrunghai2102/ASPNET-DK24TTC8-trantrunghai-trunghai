using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using BanCaPheNuocGiaiKhat.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class NguoiDungController : Controller
{
    private readonly AppDbContext _db;

    public NguoiDungController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, byte? roleId, int page = 1)
    {
        const int pageSize = 10;
        var roles = await _db.Roles.OrderBy(r => r.RoleId).ToListAsync();
        
        var query = _db.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchClean = search.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(searchClean) || 
                                     u.Email.ToLower().Contains(searchClean) || 
                                     (u.Phone != null && u.Phone.ToLower().Contains(searchClean)));
        }

        if (roleId.HasValue && roleId.Value > 0)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var usersData = await query
            .OrderByDescending(u => u.UserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserItemViewModel
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                RoleName = u.Role != null ? u.Role.RoleName : "—",
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync();

        var model = new UserListViewModel
        {
            Users = usersData,
            Roles = roles,
            SearchQuery = search,
            SelectedRoleId = roleId,
            CurrentPage = page,
            TotalPages = Math.Max(1, totalPages)
        };

        return View("~/Views/Admin/NguoiDung/Index.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int userId, byte roleId)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user == null)
        {
            return NotFound();
        }

        if (user.Email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Bạn không thể tự thay đổi quyền hạn của chính mình.";
            return RedirectToAction(nameof(Index));
        }

        var roleExists = await _db.Roles.AnyAsync(r => r.RoleId == roleId);
        if (!roleExists)
        {
            TempData["Error"] = "Quyền hạn không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        user.RoleId = roleId;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã cập nhật quyền hạn của người dùng {user.FullName} thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int userId)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var user = await _db.Users.FindAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }

        if (user.Email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Bạn không thể tự khóa tài khoản của chính mình.";
            return RedirectToAction(nameof(Index));
        }

        if (user.Status == UserStatus.active)
        {
            user.Status = UserStatus.locked;
        }
        else
        {
            user.Status = UserStatus.active;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã thay đổi trạng thái tài khoản của {user.FullName} thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _db.Roles.Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToListAsync();
        return View("~/Views/Admin/NguoiDung/Create.cshtml", new UserCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _db.Roles.Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToListAsync();
            return View("~/Views/Admin/NguoiDung/Create.cshtml", model);
        }

        var emailLower = model.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == emailLower))
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
            ViewBag.Roles = await _db.Roles.Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToListAsync();
            return View("~/Views/Admin/NguoiDung/Create.cshtml", model);
        }

        var user = new User
        {
            FullName = model.FullName,
            Email = emailLower,
            Phone = model.Phone,
            PasswordHash = PasswordHasher.Hash(model.Password),
            RoleId = model.RoleId,
            Status = UserStatus.active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã tạo tài khoản mới thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var vm = new UserEditViewModel
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            RoleId = user.RoleId
        };

        ViewBag.Roles = await _db.Roles.Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToListAsync();
        return View("~/Views/Admin/NguoiDung/Edit.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserEditViewModel model)
    {
        if (id != model.UserId) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _db.Roles.Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToListAsync();
            return View("~/Views/Admin/NguoiDung/Edit.cshtml", model);
        }

        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Admin cannot change their own role here
        if (user.Email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase) && user.RoleId != model.RoleId)
        {
            ModelState.AddModelError("RoleId", "Bạn không thể thay đổi quyền của chính mình.");
            ViewBag.Roles = await _db.Roles.Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToListAsync();
            return View("~/Views/Admin/NguoiDung/Edit.cshtml", model);
        }

        user.FullName = model.FullName;
        user.Phone = model.Phone;
        user.RoleId = model.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật tài khoản thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (user.Email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Bạn không thể xóa chính mình.";
            return RedirectToAction(nameof(Index));
        }

        var hasOrders = await _db.Orders.AnyAsync(o => o.CustomerId == id);
        var hasStaffOrders = await _db.Orders.AnyAsync(o => o.StaffId == id);

        if (hasOrders || hasStaffOrders)
        {
            TempData["Error"] = $"Không thể xóa người dùng '{user.FullName}' vì họ đã có lịch sử hóa đơn/đơn hàng. Vui lòng sử dụng tính năng KHÓA thay vì XÓA.";
            return RedirectToAction(nameof(Index));
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa người dùng '{user.FullName}'.";
        return RedirectToAction(nameof(Index));
    }
}
