using System.Security.Claims;
using System.Security.Cryptography;
using BanCaPheNuocGiaiKhat.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BanCaPheNuocGiaiKhat.Controllers;

public class AuthController : Controller
{
    private const int Pbkdf2Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private readonly MySqlConnection _connection;

    public AuthController(MySqlConnection connection)
    {
        _connection = connection;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        model.Role = NormalizeRole(model.Role);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _connection.OpenAsync();

        var roleId = await GetRoleIdAsync(model.Role);
        if (roleId is null)
        {
            ModelState.AddModelError(nameof(model.Role), "Vai tro khong ton tai trong he thong.");
            return View(model);
        }

        if (await AccountExistsAsync(model.Email, model.Username))
        {
            ModelState.AddModelError(string.Empty, "Email hoac ten dang nhap da duoc su dung.");
            return View(model);
        }

        using var command = _connection.CreateCommand();
        command.CommandText = """
            INSERT INTO users
                (role_id, full_name, email, phone, username, password_hash, status, created_at, updated_at)
            VALUES
                (@role_id, @full_name, @email, @phone, @username, @password_hash, 'active', NOW(), NOW());
            SELECT LAST_INSERT_ID();
            """;
        command.Parameters.AddWithValue("@role_id", roleId.Value);
        command.Parameters.AddWithValue("@full_name", model.FullName.Trim());
        command.Parameters.AddWithValue("@email", model.Email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(model.Phone) ? DBNull.Value : model.Phone.Trim());
        command.Parameters.AddWithValue("@username", model.Username.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@password_hash", HashPassword(model.Password));

        var userId = Convert.ToUInt32(await command.ExecuteScalarAsync());

        await SignInAsync(new AuthenticatedUser(
            userId,
            model.FullName.Trim(),
            model.Email.Trim().ToLowerInvariant(),
            model.Username.Trim().ToLowerInvariant(),
            model.Role),
            isPersistent: false);

        return RedirectByRole(model.Role);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

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
        {
            return View(model);
        }

        await _connection.OpenAsync();
        var user = await FindUserForLoginAsync(model.LoginName);

        if (user is null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Thong tin dang nhap khong chinh xac.");
            return View(model);
        }

        if (!string.Equals(user.Status, "active", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Tai khoan khong o trang thai hoat dong.");
            return View(model);
        }

        await UpdateLastLoginAsync(user.UserId);
        await SignInAsync(user.ToAuthenticatedUser(), model.RememberMe);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectByRole(user.RoleName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!uint.TryParse(userId, out var parsedUserId))
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        await _connection.OpenAsync();
        var currentHash = await GetPasswordHashAsync(parsedUserId);
        if (currentHash is null || !VerifyPassword(model.CurrentPassword, currentHash))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Mat khau hien tai khong dung.");
            return View(model);
        }

        using var command = _connection.CreateCommand();
        command.CommandText = "UPDATE users SET password_hash = @password_hash, updated_at = NOW() WHERE user_id = @user_id";
        command.Parameters.AddWithValue("@password_hash", HashPassword(model.NewPassword));
        command.Parameters.AddWithValue("@user_id", parsedUserId);
        await command.ExecuteNonQueryAsync();

        TempData["SuccessMessage"] = "Doi mat khau thanh cong.";
        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public IActionResult AdminHome()
    {
        return View("~/Views/Admin/Home/Index.cshtml");
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Staff)]
    public IActionResult StaffHome()
    {
        return View("~/Views/Staff/Home/Index.cshtml");
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Customer)]
    public IActionResult CustomerHome()
    {
        return View("~/Views/Customer/Home/Index.cshtml");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task<byte?> GetRoleIdAsync(string role)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT role_id FROM roles WHERE role_name = @role_name LIMIT 1";
        command.Parameters.AddWithValue("@role_name", role);
        var result = await command.ExecuteScalarAsync();
        return result is null || result == DBNull.Value ? null : Convert.ToByte(result);
    }

    private async Task<bool> AccountExistsAsync(string email, string username)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM users
            WHERE LOWER(email) = @email OR LOWER(username) = @username
            """;
        command.Parameters.AddWithValue("@email", email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@username", username.Trim().ToLowerInvariant());
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private async Task<LoginUser?> FindUserForLoginAsync(string loginName)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = """
            SELECT u.user_id, u.full_name, u.email, u.username, u.password_hash, u.status, r.role_name
            FROM users u
            INNER JOIN roles r ON r.role_id = u.role_id
            WHERE LOWER(u.email) = @login_name OR LOWER(u.username) = @login_name
            LIMIT 1
            """;
        command.Parameters.AddWithValue("@login_name", loginName.Trim().ToLowerInvariant());

        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new LoginUser(
            Convert.ToUInt32(reader.GetValue(reader.GetOrdinal("user_id"))),
            reader.GetString(reader.GetOrdinal("full_name")),
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("username")),
            reader.IsDBNull(reader.GetOrdinal("password_hash")) ? string.Empty : reader.GetString(reader.GetOrdinal("password_hash")),
            reader.GetString(reader.GetOrdinal("status")),
            reader.GetString(reader.GetOrdinal("role_name")));
    }

    private async Task<string?> GetPasswordHashAsync(uint userId)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT password_hash FROM users WHERE user_id = @user_id LIMIT 1";
        command.Parameters.AddWithValue("@user_id", userId);
        var result = await command.ExecuteScalarAsync();
        return result is null || result == DBNull.Value ? null : Convert.ToString(result);
    }

    private async Task UpdateLastLoginAsync(uint userId)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "UPDATE users SET last_login_at = NOW(), updated_at = NOW() WHERE user_id = @user_id";
        command.Parameters.AddWithValue("@user_id", userId);
        await command.ExecuteNonQueryAsync();
    }

    private async Task SignInAsync(AuthenticatedUser user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.RoleName),
            new("username", user.Username)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties { IsPersistent = isPersistent };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }

    private IActionResult RedirectByRole(string? role)
    {
        return NormalizeRole(role) switch
        {
            UserRoles.Admin => RedirectToAction(nameof(AdminHome)),
            UserRoles.Staff => RedirectToAction(nameof(StaffHome)),
            _ => RedirectToAction(nameof(CustomerHome))
        };
    }

    private static string NormalizeRole(string? role)
    {
        var normalized = role?.Trim().ToLowerInvariant();
        return UserRoles.All.Contains(normalized) ? normalized! : UserRoles.Customer;
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"PBKDF2-SHA256:{Pbkdf2Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256")
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[2]);
        var expectedKey = Convert.FromBase64String(parts[3]);
        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length);
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    private sealed record AuthenticatedUser(uint UserId, string FullName, string Email, string Username, string RoleName);

    private sealed record LoginUser(
        uint UserId,
        string FullName,
        string Email,
        string Username,
        string PasswordHash,
        string Status,
        string RoleName)
    {
        public AuthenticatedUser ToAuthenticatedUser() => new(UserId, FullName, Email, Username, RoleName);
    }
}
