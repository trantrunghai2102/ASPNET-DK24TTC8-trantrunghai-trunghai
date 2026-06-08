using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(r => r.RoleId);

            entity.Property(r => r.RoleId)
                  .HasColumnType("tinyint unsigned")
                  .ValueGeneratedOnAdd();

            entity.Property(r => r.RoleName)
                  .HasColumnType("varchar(30)")
                  .IsRequired();

            entity.HasIndex(r => r.RoleName)
                  .IsUnique();

            entity.Property(r => r.Description)
                  .HasColumnType("varchar(150)");

            entity.HasData(
                new Role { RoleId = 1, RoleName = "admin",    Description = "Quản trị viên hệ thống, toàn quyền." },
                new Role { RoleId = 2, RoleName = "staff",    Description = "Nhân viên, quản lý đơn hàng và sản phẩm." },
                new Role { RoleId = 3, RoleName = "customer", Description = "Khách hàng đăng ký tài khoản mua hàng." }
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.UserId);

            entity.Property(u => u.UserId)
                  .HasColumnType("int unsigned")
                  .ValueGeneratedOnAdd();

            entity.Property(u => u.RoleId)
                  .HasColumnType("tinyint unsigned")
                  .HasDefaultValue((byte)3);

            entity.Property(u => u.FullName)
                  .HasColumnType("varchar(100)")
                  .IsRequired();

            entity.Property(u => u.Email)
                  .HasColumnType("varchar(150)")
                  .IsRequired();

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.Phone)
                  .HasColumnType("varchar(15)");

            entity.Property(u => u.Address)
                  .HasColumnType("varchar(300)");

            entity.Property(u => u.AvatarUrl)
                  .HasColumnType("varchar(500)");

            entity.Property(u => u.PasswordHash)
                  .HasColumnType("varchar(255)");

            entity.Property(u => u.GoogleId)
                  .HasColumnType("varchar(100)");

            entity.HasIndex(u => u.GoogleId)
                  .IsUnique()
                  .HasFilter("`google_id` IS NOT NULL");

            entity.Property(u => u.FacebookId)
                  .HasColumnType("varchar(100)");

            entity.HasIndex(u => u.FacebookId)
                  .IsUnique()
                  .HasFilter("`facebook_id` IS NOT NULL");

            entity.Property(u => u.Status)
                  .HasColumnType("enum('active','locked','pending')")
                  .HasConversion<string>()
                  .HasDefaultValue(UserStatus.active)
                  .IsRequired();

            entity.Property(u => u.LastLoginAt)
                  .HasColumnType("datetime");

            entity.Property(u => u.CreatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.Property(u => u.UpdatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            // FK: users.role_id → roles.role_id
            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
