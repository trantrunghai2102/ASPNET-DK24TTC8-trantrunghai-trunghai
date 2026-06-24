using BanCaPheNuocGiaiKhat.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanCaPheNuocGiaiKhat.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(r => r.RoleId);

            entity.Property(r => r.RoleId)
                  .HasColumnType("tinyint")
                  .ValueGeneratedOnAdd();

            entity.Property(r => r.RoleName)
                  .HasColumnType("nvarchar(30)")
                  .IsRequired();

            entity.HasIndex(r => r.RoleName)
                  .IsUnique();

            entity.Property(r => r.Description)
                  .HasColumnType("nvarchar(150)");

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
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(u => u.RoleId)
                  .HasColumnType("tinyint")
                  .HasDefaultValue((byte)3);

            entity.Property(u => u.FullName)
                  .HasColumnType("nvarchar(100)")
                  .IsRequired();

            entity.Property(u => u.Email)
                  .HasColumnType("nvarchar(150)")
                  .IsRequired();

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.Phone)
                  .HasColumnType("nvarchar(15)");

            entity.Property(u => u.Address)
                  .HasColumnType("nvarchar(300)");

            entity.Property(u => u.AvatarUrl)
                  .HasColumnType("nvarchar(500)");

            entity.Property(u => u.PasswordHash)
                  .HasColumnType("nvarchar(255)");

            entity.Property(u => u.GoogleId)
                  .HasColumnType("nvarchar(100)");

            entity.HasIndex(u => u.GoogleId)
                  .IsUnique()
                  .HasFilter("[google_id] IS NOT NULL");

            entity.Property(u => u.FacebookId)
                  .HasColumnType("nvarchar(100)");

            entity.HasIndex(u => u.FacebookId)
                  .IsUnique()
                  .HasFilter("[facebook_id] IS NOT NULL");

            entity.Property(u => u.Status)
                  .HasColumnType("nvarchar(20)")
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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(c => c.CategoryId);
            
            entity.Property(c => c.CategoryId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(c => c.Name)
                  .HasColumnType("nvarchar(100)")
                  .IsRequired();

            entity.Property(c => c.ParentId)
                  .HasColumnType("int");

            entity.HasOne(c => c.Parent)
                  .WithMany(p => p.Children)
                  .HasForeignKey(c => c.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.ProductId);

            entity.Property(p => p.ProductId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(p => p.CategoryId)
                  .HasColumnType("int");

            entity.Property(p => p.Name)
                  .HasColumnType("nvarchar(200)")
                  .IsRequired();

            entity.Property(p => p.Slug)
                  .HasColumnType("nvarchar(200)")
                  .IsRequired();

            entity.HasIndex(p => p.Slug)
                  .IsUnique();

            entity.Property(p => p.BasePrice)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(p => p.PromotionPrice)
                  .HasColumnType("decimal(18,2)");

            entity.Property(p => p.ShortDesc)
                  .HasColumnType("nvarchar(500)");

            entity.Property(p => p.DetailDesc)
                  .HasColumnType("nvarchar(max)");

            entity.Property(p => p.ThumbnailUrl)
                  .HasColumnType("nvarchar(500)");

            entity.Property(p => p.StockQty)
                  .HasColumnType("int")
                  .HasDefaultValue(0)
                  .IsRequired();

            entity.Property(p => p.ViewCount)
                  .HasColumnType("int")
                  .HasDefaultValue(0)
                  .IsRequired();

            entity.Property(p => p.Status)
                  .HasColumnType("nvarchar(50)")
                  .HasDefaultValue("active")
                  .IsRequired();

            entity.Property(p => p.CreatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.Property(p => p.UpdatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("product_images");
            entity.HasKey(pi => pi.ImageId);

            entity.Property(pi => pi.ImageId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(pi => pi.ProductId)
                  .HasColumnType("int")
                  .IsRequired();

            entity.Property(pi => pi.Url)
                  .HasColumnType("nvarchar(500)")
                  .IsRequired();

            entity.Property(pi => pi.AltText)
                  .HasColumnType("nvarchar(200)");

            entity.Property(pi => pi.IsPrimary)
                  .HasColumnType("bit")
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(pi => pi.SortOrder)
                  .HasColumnType("int")
                  .HasDefaultValue(0)
                  .IsRequired();

            entity.HasOne(pi => pi.Product)
                  .WithMany(p => p.ProductImages)
                  .HasForeignKey(pi => pi.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
         modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(o => o.OrderId);

            entity.Property(o => o.OrderId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();
            
            entity.Property(o => o.CustomerId)
                  .HasColumnType("int");
            
            entity.Property(o => o.StaffId)
                  .HasColumnType("int");

            entity.Property(o => o.OrderType)
                  .HasColumnType("nvarchar(20)")
                  .HasDefaultValue("instore")
                  .IsRequired();

            entity.Property(o => o.TotalAmount)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(o => o.CashGiven)
                  .HasColumnType("decimal(18,2)");

            entity.Property(o => o.ChangeAmount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(o => o.Status)
                  .HasColumnType("nvarchar(30)")
                  .HasDefaultValue("pending")
                  .IsRequired();

            entity.Property(o => o.PaymentStatus)
                  .HasColumnType("nvarchar(20)")
                  .HasDefaultValue("unpaid")
                  .IsRequired();

            entity.Property(o => o.RecipientName)
                  .HasColumnType("nvarchar(100)");

            entity.Property(o => o.RecipientPhone)
                  .HasColumnType("nvarchar(15)");

            entity.Property(o => o.DeliveryAddress)
                  .HasColumnType("nvarchar(300)");


            entity.Property(o => o.Notes)
                  .HasColumnType("nvarchar(500)");

            entity.Property(o => o.CreatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.Property(o => o.UpdatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();
            
            entity.HasOne(o => o.Customer)
                  .WithMany()
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(o => o.Staff)
                  .WithMany()
                  .HasForeignKey(o => o.StaffId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.HasKey(oi => oi.OrderItemId);

            entity.Property(oi => oi.OrderItemId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(oi => oi.OrderId)
                  .HasColumnType("int")
                  .IsRequired();

            entity.Property(oi => oi.ProductId)
                  .HasColumnType("int");

            entity.Property(oi => oi.ProductName)
                  .HasColumnType("nvarchar(200)")
                  .IsRequired();

            entity.Property(oi => oi.UnitPrice)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(oi => oi.Quantity)
                  .HasColumnType("int")
                  .IsRequired();

            entity.Property(oi => oi.Subtotal)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.Product)
                  .WithMany()
                  .HasForeignKey(oi => oi.ProductId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(i => i.InvoiceId);

            entity.Property(i => i.InvoiceId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(i => i.OrderId)
                  .HasColumnType("int")
                  .IsRequired();

            entity.HasIndex(i => i.OrderId)
                  .IsUnique();

            entity.Property(i => i.InvoiceCode)
                  .HasColumnType("nvarchar(50)")
                  .IsRequired();

            entity.HasIndex(i => i.InvoiceCode)
                  .IsUnique();

            entity.Property(i => i.TotalAmount)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(i => i.CashGiven)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(i => i.ChangeAmount)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(i => i.PaidAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.Property(i => i.CreatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.HasOne(i => i.Order)
                  .WithOne(o => o.Invoice)
                  .HasForeignKey<Invoice>(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_items");
            entity.HasKey(c => c.CartItemId);

            entity.Property(c => c.CartItemId)
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(c => c.UserId)
                  .HasColumnType("int")
                  .IsRequired();

            entity.Property(c => c.ProductId)
                  .HasColumnType("int");

            entity.Property(c => c.Quantity)
                  .HasColumnType("int")
                  .HasDefaultValue(1)
                  .IsRequired();

            entity.Property(c => c.CreatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.Property(c => c.UpdatedAt)
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.HasOne(c => c.User)
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Product)
                  .WithMany()
                  .HasForeignKey(c => c.ProductId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
