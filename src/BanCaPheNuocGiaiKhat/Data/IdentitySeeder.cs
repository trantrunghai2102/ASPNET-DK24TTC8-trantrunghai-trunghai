using BanCaPheNuocGiaiKhat.Models;
using BanCaPheNuocGiaiKhat.Models.Entities;
using BanCaPheNuocGiaiKhat.Utils;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Data;

public static class IdentitySeeder
{
    private static readonly SeedAccount[] Accounts =
    [
        new(
            RoleName: UserRoles.Admin,
            FullName: "System Admin",
            Email: "admin@thedrinkvn.local",
            Phone: "0900000001",
            Password: "Admin@123"),
        new(
            RoleName: UserRoles.Staff,
            FullName: "Store Staff",
            Email: "staff@thedrinkvn.local",
            Phone: "0900000002",
            Password: "Staff@123"),
        new(
            RoleName: UserRoles.Staff,
            FullName: "Nhân viên 2",
            Email: "trunghai.2102@gmail.com",
            Phone: "0900000003",
            Password: "Demo@123"),
        new(
            RoleName: UserRoles.Staff,
            FullName: "Nhân viên 3",
            Email: "trunghai.pn@gmail.com",
            Phone: "0900000004",
            Password: "Demo@123")
    ];

    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        var roles = await db.Roles
            .ToDictionaryAsync(r => r.RoleName, r => r.RoleId);

        foreach (var account in Accounts)
        {
            if (!roles.TryGetValue(account.RoleName, out var roleId))
            {
                logger.LogWarning("Khong tim thay role {RoleName}, bo qua seed tai khoan {Email}.", account.RoleName, account.Email);
                continue;
            }

            var email = account.Email.ToLowerInvariant();
            var exists = await db.Users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                continue;
            }

            var now = DateTime.UtcNow;
            db.Users.Add(new User
            {
                RoleId = roleId,
                FullName = account.FullName,
                Email = email,
                Phone = account.Phone,
                PasswordHash = PasswordHasher.Hash(account.Password),
                Status = UserStatus.active,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        await db.SaveChangesAsync();
    }

    private sealed record SeedAccount(string RoleName, string FullName, string Email, string Phone, string Password);
}
