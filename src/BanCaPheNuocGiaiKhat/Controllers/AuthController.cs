using System.Security.Claims;
using System.Security.Cryptography;
using BanCaPheNuocGiaiKhat.Data;
using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Controllers;

public class AuthController : Controller
{
    private const int Pbkdf2Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;
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
            PasswordHash = HashPassword(model.Password),
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

        if (user is null || user.PasswordHash is null || !VerifyPassword(model.Password, user.PasswordHash))
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

        if (user?.PasswordHash is null || !VerifyPassword(model.CurrentPassword, user.PasswordHash))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng.");
            return View(model);
        }

        await _db.Users
                 .Where(u => u.UserId == userId)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(u => u.PasswordHash, HashPassword(model.NewPassword))
                     .SetProperty(u => u.UpdatedAt,    DateTime.UtcNow));

        TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
        return RedirectToAction(nameof(ChangePassword));
    }

    // ── Role-based landing pages ──────────────────────────────────────────
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public IActionResult AdminHome() => View("~/Views/Admin/Home/Index.cshtml");

    [HttpGet]
    [Authorize(Roles = UserRoles.Staff)]
    public IActionResult StaffHome() => View("~/Views/Staff/Home/Index.cshtml");

    [HttpGet]
    [Authorize(Roles = UserRoles.Customer)]
    public IActionResult CustomerHome() => View("~/Views/Customer/Home/Index.cshtml");

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
            UserRoles.Staff  => RedirectToAction(nameof(StaffHome)),
            _                => RedirectToAction("Index", "Home")
        };

    private static string NormalizeRole(string? role)
    {
        var normalized = role?.Trim().ToLowerInvariant();
        return UserRoles.All.Contains(normalized) ? normalized! : UserRoles.Customer;
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key  = Rfc2898DeriveBytes.Pbkdf2(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"PBKDF2-SHA256:{Pbkdf2Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256") return false;
        if (!int.TryParse(parts[1], out var iterations))      return false;

        var salt        = Convert.FromBase64String(parts[2]);
        var expectedKey = Convert.FromBase64String(parts[3]);
        var actualKey   = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length);
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    private sealed record AuthenticatedUser(int UserId, string FullName, string Email, string RoleName);
}
