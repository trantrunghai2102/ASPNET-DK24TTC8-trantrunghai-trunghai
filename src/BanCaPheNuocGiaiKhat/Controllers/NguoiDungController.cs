using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Index(string? search, byte? roleId)
    {
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

        var usersData = await query
            .OrderByDescending(u => u.UserId)
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
            SelectedRoleId = roleId
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
}
