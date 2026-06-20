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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user != null)
        {
            return RedirectToAction(nameof(ResetPassword), new { email = model.Email });
        }

        ModelState.AddModelError(nameof(model.Email), "Email không tồn tại trong hệ thống.");
        return View(model);
    }

    // ── GET /Auth/ResetPassword ──────────────────────────────────────────
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return RedirectToAction(nameof(ForgotPassword));

        return View(new ResetPasswordViewModel { Email = email });
    }

    // ── POST /Auth/ResetPassword ─────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng.");
            return View(model);
        }

        user.PasswordHash = PasswordHasher.Hash(model.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Khôi phục mật khẩu thành công. Bạn có thể đăng nhập bằng mật khẩu mới.";
        return RedirectToAction(nameof(Login));
    }

     [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return RedirectToAction(nameof(Login));

        var user = await _db.Users.Include(u => u.Role).AsNoTracking()
                            .FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null) return RedirectToAction(nameof(Login));

        return View(new ProfileViewModel
        {
            FullName  = user.FullName,
            Email     = user.Email,
            Phone     = user.Phone,
            Address   = user.Address,
            Role      = user.Role?.RoleName ?? UserRoles.Customer,
            CreatedAt = user.CreatedAt
        });
    }

    // ── POST /Auth/Profile ───────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return RedirectToAction(nameof(Login));

        var user = await _db.Users.Include(u => u.Role).AsNoTracking()
                            .FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null) return RedirectToAction(nameof(Login));

        model.Email     = user.Email;
        model.Role      = user.Role?.RoleName ?? UserRoles.Customer;
        model.CreatedAt = user.CreatedAt;

        if (!ModelState.IsValid)
            return View(model);

        await _db.Users
                 .Where(u => u.UserId == userId)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(u => u.FullName,  model.FullName.Trim())
                     .SetProperty(u => u.Phone,     string.IsNullOrWhiteSpace(model.Phone)   ? null : model.Phone.Trim())
                     .SetProperty(u => u.Address,   string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim())
                     .SetProperty(u => u.UpdatedAt, DateTime.UtcNow));

        TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công.";
        return RedirectToAction(nameof(Profile));
    }


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
            UserRoles.Admin  => RedirectToAction("Index", "AdminDashboard"),
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
