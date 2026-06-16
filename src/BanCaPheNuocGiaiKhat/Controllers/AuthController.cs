using System.Security.Claims;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using BanCaPheNuocGiaiKhat.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        model.Role = NormalizeRole(model.Role);

        if (!ModelState.IsValid)
            return View(model);

        // Kiểm tra role tồn tại trong DB
        var role = await _db.Roles.AsNoTracking()
                            .FirstOrDefaultAsync(r => r.RoleName == model.Role);
        if (role is null)
        {
            ModelState.AddModelError(nameof(model.Role), "Vai trò không tồn tại trong hệ thống.");
            return View(model);
        }

        // Kiểm tra email hoặc tên đăng nhập trùng
        var emailLower = model.Email.Trim().ToLowerInvariant();
        var alreadyExists = await _db.Users.AsNoTracking()
                                     .AnyAsync(u => u.Email == emailLower);
        if (alreadyExists)
        {
            ModelState.AddModelError(string.Empty, "Email đã được sử dụng.");
            return View(model);
        }

        var now = DateTime.UtcNow;
        var newUser = new User
        {
            RoleId      = role.RoleId,
            FullName    = model.FullName.Trim(),
            Email       = emailLower,
            Phone       = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
            PasswordHash = PasswordHasher.Hash(model.Password),
            Status      = UserStatus.active,
            CreatedAt   = now,
            UpdatedAt   = now
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        await SignInAsync(new AuthenticatedUser(
            newUser.UserId,
            newUser.FullName,
            newUser.Email,
            model.Role),
            isPersistent: false);

        return RedirectByRole(model.Role);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
            return View(model);

        var loginLower = model.LoginName.Trim().ToLowerInvariant();

        var user = await _db.Users
                            .Include(u => u.Role)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Email == loginLower);

        if (user is null || user.PasswordHash is null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không chính xác.");
            return View(model);
        }

        if (user.Status != UserStatus.active)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản không ở trạng thái hoạt động.");
            return View(model);
        }

        // Cập nhật last_login_at
        await _db.Users
                 .Where(u => u.UserId == user.UserId)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(u => u.LastLoginAt, DateTime.UtcNow)
                     .SetProperty(u => u.UpdatedAt,   DateTime.UtcNow));

        var roleName = user.Role?.RoleName ?? UserRoles.Customer;
        await SignInAsync(new AuthenticatedUser(
            user.UserId,
            user.FullName,
            user.Email,
            roleName),
            model.RememberMe);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectByRole(roleName);
    }

    // ── POST /Auth/Logout ────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // ── GET /Auth/ChangePassword ─────────────────────────────────────────
    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    // ── POST /Auth/ChangePassword ────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        var user = await _db.Users.AsNoTracking()
                            .Select(u => new { u.UserId, u.PasswordHash })
                            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user?.PasswordHash is null || !PasswordHasher.Verify(model.CurrentPassword, user.PasswordHash))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng.");
            return View(model);
        }

        await _db.Users
                 .Where(u => u.UserId == userId)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(u => u.PasswordHash, PasswordHasher.Hash(model.NewPassword))
                     .SetProperty(u => u.UpdatedAt,    DateTime.UtcNow));

        TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
        return RedirectToAction(nameof(ChangePassword));
    }

    // ── Role-based landing pages ──────────────────────────────────────────
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AdminHome(int? month, int? year, DateTime? fromDate, DateTime? toDate)
    {
        var today = DateTime.Today;
        var selectedYear = year ?? today.Year;
        var selectedMonth = month;

        DateTime periodStart;
        DateTime periodEnd;
        if (fromDate.HasValue || toDate.HasValue)
        {
            periodStart = (fromDate ?? today.AddDays(-30)).Date;
            periodEnd = (toDate ?? today).Date.AddDays(1);
        }
        else if (selectedMonth.HasValue)
        {
            periodStart = new DateTime(selectedYear, selectedMonth.Value, 1);
            periodEnd = periodStart.AddMonths(1);
        }
        else
        {
            periodStart = new DateTime(selectedYear, 1, 1);
            periodEnd = periodStart.AddYears(1);
        }

        var invoiceQuery = _db.Invoices
            .AsNoTracking()
            .Where(i => i.PaidAt >= periodStart && i.PaidAt < periodEnd);

        var totalRevenue = await invoiceQuery.SumAsync(i => (decimal?)i.TotalAmount) ?? 0m;
        var totalOrders = await invoiceQuery.CountAsync();

        var revenueByDay = await invoiceQuery
            .GroupBy(i => i.PaidAt.Date)
            .Select(g => new AdminRevenuePointViewModel
            {
                Date = g.Key,
                Revenue = g.Sum(i => i.TotalAmount),
                Orders = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var orderIds = await invoiceQuery.Select(i => i.OrderId).ToListAsync();
        var topProducts = orderIds.Count == 0
            ? new List<AdminTopProductViewModel>()
            : await _db.OrderItems
                .AsNoTracking()
                .Where(i => orderIds.Contains(i.OrderId))
                .GroupBy(i => i.ProductName)
                .Select(g => new AdminTopProductViewModel
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(6)
                .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            Month = selectedMonth,
            Year = selectedYear,
            FromDate = fromDate,
            ToDate = toDate,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd.AddDays(-1),
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            AverageOrderValue = totalOrders == 0 ? 0 : totalRevenue / totalOrders,
            TotalProductsSold = topProducts.Sum(p => p.Quantity),
            RevenueByDay = revenueByDay,
            TopProducts = topProducts,
            RecentInvoices = await invoiceQuery
                .OrderByDescending(i => i.PaidAt)
                .Take(5)
                .Select(i => new AdminRecentInvoiceViewModel
                {
                    InvoiceCode = i.InvoiceCode,
                    TotalAmount = i.TotalAmount,
                    PaidAt = i.PaidAt
                })
                .ToListAsync()
        };

        return View("~/Views/Admin/Home/Index.cshtml", model);
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Staff)]
    public IActionResult StaffHome() => RedirectToAction("Index", "BanHang");

    [HttpGet]
    [Authorize(Roles = UserRoles.Customer)]
    public IActionResult CustomerHome() => RedirectToAction("DonHangCuaToi", "DatHang");
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // ── Private helpers ───────────────────────────────────────────────────

    private async Task SignInAsync(AuthenticatedUser user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name,           user.FullName),
            new(ClaimTypes.Email,          user.Email),
            new(ClaimTypes.Role,           user.RoleName)
        };

        var identity   = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal  = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties { IsPersistent = isPersistent };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }

    private IActionResult RedirectByRole(string? role) =>
        NormalizeRole(role) switch
        {
            UserRoles.Admin  => RedirectToAction(nameof(AdminHome)),
            UserRoles.Staff  => RedirectToAction("Index", "BanHang"),
            _                => RedirectToAction("Index", "Home")
        };

    private static string NormalizeRole(string? role)
    {
        var normalized = role?.Trim().ToLowerInvariant();
        return UserRoles.All.Contains(normalized) ? normalized! : UserRoles.Customer;
    }

    private sealed record AuthenticatedUser(int UserId, string FullName, string Email, string RoleName);
}
