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
                  .HasColumnType("int")
                  .ValueGeneratedOnAdd();

            entity.Property(u => u.RoleId)
                  .HasColumnType("tinyint")
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
                  .HasFilter("[google_id] IS NOT NULL");

            entity.Property(u => u.FacebookId)
                  .HasColumnType("varchar(100)");

            entity.HasIndex(u => u.FacebookId)
                  .IsUnique()
                  .HasFilter("[facebook_id] IS NOT NULL");

            entity.Property(u => u.Status)
                  .HasColumnType("varchar(20)")
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

            entity.HasData(
                new Category { CategoryId = 1, Name = "Cà phê hạt", ParentId = null },
                new Category { CategoryId = 2, Name = "Cà phê bột", ParentId = null },
                new Category { CategoryId = 3, Name = "Dụng cụ pha chế", ParentId = null }
            );
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
                  .HasColumnType("varchar(200)")
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
                  .HasColumnType("varchar(50)")
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

            var now = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new Product
                {
                    ProductId = 1,
                    CategoryId = 1,
                    Name = "Ethiopia Yirgacheffe",
                    Slug = "ethiopia-yirgacheffe",
                    BasePrice = 22.00m,
                    PromotionPrice = null,
                    ShortDesc = "Floral notes with hints of jasmine and bright citrus. Light roast.",
                    DetailDesc = "Ethiopian Yirgacheffe coffee is dry-processed, bringing out the bright, clean taste profile. Notes of citrus and floral aromas are dominant.",
                    ThumbnailUrl = "/images/product-ethiopian.png",
                    StockQty = 50,
                    ViewCount = 12,
                    Status = "active",
                    RoastLevel = "light",
                    Region = "africa",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 2,
                    CategoryId = 1,
                    Name = "Colombia Supremo",
                    Slug = "colombia-supremo",
                    BasePrice = 19.50m,
                    PromotionPrice = null,
                    ShortDesc = "Smooth, balanced body with caramel sweetness and a nutty finish. Medium roast.",
                    DetailDesc = "Colombia Supremo beans are carefully selected and roasted to a medium profile to balance the natural acidity with nutty tones.",
                    ThumbnailUrl = "/images/product-colombia.png",
                    StockQty = 35,
                    ViewCount = 8,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "south-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 3,
                    CategoryId = 1,
                    Name = "Sumatra Mandheling",
                    Slug = "sumatra-mandheling",
                    BasePrice = 21.00m,
                    PromotionPrice = null,
                    ShortDesc = "Full-bodied, earthy, and complex with low acidity. Dark roast.",
                    DetailDesc = "Sumatra Mandheling coffee is known for its heavy body and low acidity, featuring unique cedar wood and herbal notes.",
                    ThumbnailUrl = "/images/hero-coffee-beans.png",
                    StockQty = 40,
                    ViewCount = 20,
                    Status = "active",
                    RoastLevel = "dark",
                    Region = "asia-pacific",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 4,
                    CategoryId = 1,
                    Name = "Artisan Espresso Blend",
                    Slug = "artisan-espresso-blend",
                    BasePrice = 24.00m,
                    PromotionPrice = null,
                    ShortDesc = "Rich crema, notes of dark chocolate and toasted almonds. Espresso roast.",
                    DetailDesc = "Our signature espresso blend combines beans from South America and Africa, roasted dark to bring out rich cocoa and toasted nut flavors.",
                    ThumbnailUrl = "/images/product-artisan-espresso.png",
                    StockQty = 25,
                    ViewCount = 15,
                    Status = "active",
                    RoastLevel = "espresso",
                    Region = "south-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 5,
                    CategoryId = 1,
                    Name = "Guatemala Antigua",
                    Slug = "guatemala-antigua",
                    BasePrice = 20.50m,
                    PromotionPrice = null,
                    ShortDesc = "Spicy and smoky undertones with a crisp apple acidity. Medium roast.",
                    DetailDesc = "Guatemala Antigua beans are grown in volcanic soil, resulting in a unique smoky flavor combined with a light citrus brightness.",
                    ThumbnailUrl = "/images/product-guatemala.png",
                    StockQty = 30,
                    ViewCount = 5,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "central-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 6,
                    CategoryId = 1,
                    Name = "Kenya AA",
                    Slug = "kenya-aa",
                    BasePrice = 23.50m,
                    PromotionPrice = null,
                    ShortDesc = "Bright acidity with blackcurrant and berry notes.",
                    DetailDesc = "Kenya AA is processed using wet method, offering an exceptionally intense and clean cup.",
                    ThumbnailUrl = "/images/product-ethiopian.png",
                    StockQty = 45,
                    ViewCount = 28,
                    Status = "active",
                    RoastLevel = "light",
                    Region = "africa",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 7,
                    CategoryId = 1,
                    Name = "Brazil Santos",
                    Slug = "brazil-santos",
                    BasePrice = 18.00m,
                    PromotionPrice = null,
                    ShortDesc = "Low acidity, smooth body with sweet chocolate notes.",
                    DetailDesc = "Classic Brazilian coffee bean, roasted to highlight its natural sweetness.",
                    ThumbnailUrl = "/images/product-colombia.png",
                    StockQty = 60,
                    ViewCount = 33,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "south-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 8,
                    CategoryId = 1,
                    Name = "Costa Rica Tarrazu",
                    Slug = "costa-rica-tarrazu",
                    BasePrice = 20.00m,
                    PromotionPrice = null,
                    ShortDesc = "Crisp acidity with sweet honey and citrus flavors.",
                    DetailDesc = "Tarrazu region provides one of the finest beans in Central America.",
                    ThumbnailUrl = "/images/product-guatemala.png",
                    StockQty = 25,
                    ViewCount = 19,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "central-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 9,
                    CategoryId = 2,
                    Name = "Ethiopian Sidamo Ground",
                    Slug = "ethiopian-sidamo-ground",
                    BasePrice = 21.50m,
                    PromotionPrice = null,
                    ShortDesc = "Floral aroma with notes of lemon and cane sugar.",
                    DetailDesc = "Ground Ethiopian Sidamo coffee ready for drip brewing.",
                    ThumbnailUrl = "/images/product-ethiopian.png",
                    StockQty = 30,
                    ViewCount = 14,
                    Status = "active",
                    RoastLevel = "light",
                    Region = "africa",
                    GrindType = "medium",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 10,
                    CategoryId = 2,
                    Name = "French Roast Blend",
                    Slug = "french-roast-blend",
                    BasePrice = 19.00m,
                    PromotionPrice = null,
                    ShortDesc = "Intensely dark and smoky with a heavy body.",
                    DetailDesc = "Ground French roast ideal for French Press brewing.",
                    ThumbnailUrl = "/images/hero-coffee-beans.png",
                    StockQty = 40,
                    ViewCount = 11,
                    Status = "active",
                    RoastLevel = "dark",
                    Region = "south-america",
                    GrindType = "coarse",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 11,
                    CategoryId = 2,
                    Name = "Italian Roast Espresso Ground",
                    Slug = "italian-roast-espresso-ground",
                    BasePrice = 19.50m,
                    PromotionPrice = null,
                    ShortDesc = "Rich, dark roast optimized for espresso brewing.",
                    DetailDesc = "Fine ground Italian roast optimized for espresso machines.",
                    ThumbnailUrl = "/images/product-artisan-espresso.png",
                    StockQty = 50,
                    ViewCount = 22,
                    Status = "active",
                    RoastLevel = "espresso",
                    Region = "south-america",
                    GrindType = "fine",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 12,
                    CategoryId = 3,
                    Name = "Hario V60 Ceramic",
                    Slug = "hario-v60-ceramic",
                    BasePrice = 25.00m,
                    PromotionPrice = null,
                    ShortDesc = "Classic ceramic coffee dripper for pour-over brewing.",
                    DetailDesc = "Premium Japanese ceramic pour-over dripper.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 20,
                    ViewCount = 9,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 13,
                    CategoryId = 3,
                    Name = "Chemex 6-Cup",
                    Slug = "chemex-6-cup",
                    BasePrice = 45.00m,
                    PromotionPrice = null,
                    ShortDesc = "Elegant glass coffee maker for clean and pure flavor.",
                    DetailDesc = "Chemex glass coffeemaker with wood collar and leather tie.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 15,
                    ViewCount = 18,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 14,
                    CategoryId = 3,
                    Name = "Aeropress Go",
                    Slug = "aeropress-go",
                    BasePrice = 39.99m,
                    PromotionPrice = null,
                    ShortDesc = "Compact coffee press for travel and convenience.",
                    DetailDesc = "Aeropress travel version with convenient mug/carrying case.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 25,
                    ViewCount = 25,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 15,
                    CategoryId = 1,
                    Name = "Java Preanger",
                    Slug = "java-preanger",
                    BasePrice = 22.50m,
                    PromotionPrice = null,
                    ShortDesc = "Traditional Indonesian cup with sweet herbal nuances.",
                    DetailDesc = "Java Preanger beans from West Java volcanic highlands.",
                    ThumbnailUrl = "/images/hero-coffee-beans.png",
                    StockQty = 30,
                    ViewCount = 7,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "asia-pacific",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 16,
                    CategoryId = 1,
                    Name = "Vietnam Robusta Special",
                    Slug = "vietnam-robusta-special",
                    BasePrice = 16.50m,
                    PromotionPrice = null,
                    ShortDesc = "Bold, high-caffeine Robusta beans from Buon Ma Thuot.",
                    DetailDesc = "Buon Ma Thuot high-grade robusta beans with strong flavor.",
                    ThumbnailUrl = "/images/product-colombia.png",
                    StockQty = 80,
                    ViewCount = 42,
                    Status = "active",
                    RoastLevel = "dark",
                    Region = "asia-pacific",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 17,
                    CategoryId = 1,
                    Name = "Peru Cajamarca",
                    Slug = "peru-cajamarca",
                    BasePrice = 19.90m,
                    PromotionPrice = null,
                    ShortDesc = "Mellow body with subtle apple acidity and caramel notes.",
                    DetailDesc = "Single origin Peru coffee beans.",
                    ThumbnailUrl = "/images/product-guatemala.png",
                    StockQty = 35,
                    ViewCount = 13,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "south-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 18,
                    CategoryId = 2,
                    Name = "House Blend Ground",
                    Slug = "house-blend-ground",
                    BasePrice = 17.50m,
                    PromotionPrice = null,
                    ShortDesc = "Our balanced house blend for everyday brewing.",
                    DetailDesc = "Ground house blend of South American and African beans.",
                    ThumbnailUrl = "/images/product-artisan-espresso.png",
                    StockQty = 70,
                    ViewCount = 16,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "south-america",
                    GrindType = "medium",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 19,
                    CategoryId = 3,
                    Name = "Moka Pot Bialetti",
                    Slug = "moka-pot-bialetti",
                    BasePrice = 35.00m,
                    PromotionPrice = null,
                    ShortDesc = "Iconic Italian stovetop espresso maker.",
                    DetailDesc = "Bialetti Moka Express 3-Cup stovetop espresso maker.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 18,
                    ViewCount = 21,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 20,
                    CategoryId = 3,
                    Name = "Kalita Wave Stainless",
                    Slug = "kalita-wave-stainless",
                    BasePrice = 30.00m,
                    PromotionPrice = null,
                    ShortDesc = "Flat-bottom coffee dripper for even extraction.",
                    DetailDesc = "Kalita Wave stainless steel 185 pour-over dripper.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 12,
                    ViewCount = 10,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 21,
                    CategoryId = 3,
                    Name = "Porlex Mini Grinder",
                    Slug = "porlex-mini-grinder",
                    BasePrice = 55.00m,
                    PromotionPrice = null,
                    ShortDesc = "Portable ceramic burr hand grinder.",
                    DetailDesc = "Porlex Mini stainless steel hand coffee grinder with ceramic burrs.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 10,
                    ViewCount = 31,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 22,
                    CategoryId = 3,
                    Name = "Gooseneck Kettle",
                    Slug = "gooseneck-kettle",
                    BasePrice = 40.00m,
                    PromotionPrice = null,
                    ShortDesc = "Stainless steel kettle with precise pour control.",
                    DetailDesc = "Stovetop gooseneck kettle for precise water flow control.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 15,
                    ViewCount = 17,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 23,
                    CategoryId = 1,
                    Name = "Jamaica Blue Mountain",
                    Slug = "jamaica-blue-mountain",
                    BasePrice = 65.00m,
                    PromotionPrice = null,
                    ShortDesc = "Mild flavor, lack of bitterness, and sweet herbal notes.",
                    DetailDesc = "Authentic Jamaica Blue Mountain coffee with a smooth, clean finish.",
                    ThumbnailUrl = "/images/product-guatemala.png",
                    StockQty = 10,
                    ViewCount = 50,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "caribbean",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 24,
                    CategoryId = 1,
                    Name = "Hawaii Kona Extra Fancy",
                    Slug = "hawaii-kona-extra-fancy",
                    BasePrice = 55.00m,
                    PromotionPrice = null,
                    ShortDesc = "Rich, smooth, and deeply flavorful with a nutty aroma.",
                    DetailDesc = "Grown on the slopes of Hualalai and Mauna Loa in the North and South Kona Districts.",
                    ThumbnailUrl = "/images/product-colombia.png",
                    StockQty = 15,
                    ViewCount = 45,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "hawaii",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 25,
                    CategoryId = 1,
                    Name = "Tanzania Peaberry",
                    Slug = "tanzania-peaberry",
                    BasePrice = 24.00m,
                    PromotionPrice = null,
                    ShortDesc = "Bright acidity with notes of blackcurrant and chocolate.",
                    DetailDesc = "Peaberry coffee beans from the slopes of Mount Kilimanjaro.",
                    ThumbnailUrl = "/images/product-ethiopian.png",
                    StockQty = 25,
                    ViewCount = 12,
                    Status = "active",
                    RoastLevel = "light",
                    Region = "africa",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 26,
                    CategoryId = 1,
                    Name = "Rwanda Bourbon",
                    Slug = "rwanda-bourbon",
                    BasePrice = 21.00m,
                    PromotionPrice = null,
                    ShortDesc = "Sweet with floral notes and hints of caramelized sugar.",
                    DetailDesc = "Bourbon varietal grown in the high altitudes of Rwanda.",
                    ThumbnailUrl = "/images/hero-coffee-beans.png",
                    StockQty = 30,
                    ViewCount = 20,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "africa",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 27,
                    CategoryId = 1,
                    Name = "Burundi Ngozi",
                    Slug = "burundi-ngozi",
                    BasePrice = 22.00m,
                    PromotionPrice = null,
                    ShortDesc = "Complex fruit flavors with a clean, sweet finish.",
                    DetailDesc = "Sourced from the Ngozi province, featuring bright and juicy fruit notes.",
                    ThumbnailUrl = "/images/product-colombia.png",
                    StockQty = 20,
                    ViewCount = 18,
                    Status = "active",
                    RoastLevel = "light",
                    Region = "africa",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 28,
                    CategoryId = 1,
                    Name = "Yemen Mocha Mattari",
                    Slug = "yemen-mocha-mattari",
                    BasePrice = 35.00m,
                    PromotionPrice = null,
                    ShortDesc = "Earthy, complex with intense chocolate and wine notes.",
                    DetailDesc = "One of the oldest and most traditional coffees, grown on dry terraces in Yemen.",
                    ThumbnailUrl = "/images/product-artisan-espresso.png",
                    StockQty = 12,
                    ViewCount = 35,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "middle-east",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 29,
                    CategoryId = 1,
                    Name = "Panama Geisha",
                    Slug = "panama-geisha",
                    BasePrice = 85.00m,
                    PromotionPrice = null,
                    ShortDesc = "Incredibly delicate with jasmine aroma and bergamot flavors.",
                    DetailDesc = "A highly sought-after, premium coffee varietal known for its tea-like body and floral aroma.",
                    ThumbnailUrl = "/images/product-guatemala.png",
                    StockQty = 5,
                    ViewCount = 80,
                    Status = "active",
                    RoastLevel = "light",
                    Region = "central-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 30,
                    CategoryId = 1,
                    Name = "Decaf Swiss Water Blend",
                    Slug = "decaf-swiss-water",
                    BasePrice = 20.00m,
                    PromotionPrice = null,
                    ShortDesc = "Smooth and rich, decaffeinated without chemicals.",
                    DetailDesc = "Processed using the Swiss Water Method to retain 99.9% of the flavor without the caffeine.",
                    ThumbnailUrl = "/images/hero-coffee-beans.png",
                    StockQty = 40,
                    ViewCount = 15,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "south-america",
                    GrindType = "whole-bean",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 31,
                    CategoryId = 2,
                    Name = "Breakfast Blend Ground",
                    Slug = "breakfast-blend-ground",
                    BasePrice = 16.00m,
                    PromotionPrice = null,
                    ShortDesc = "A bright, crisp start to the day.",
                    DetailDesc = "A perfectly balanced blend of South American and Central American beans.",
                    ThumbnailUrl = "/images/product-colombia.png",
                    StockQty = 60,
                    ViewCount = 22,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "blend",
                    GrindType = "medium",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 32,
                    CategoryId = 2,
                    Name = "Vanilla Nut Ground",
                    Slug = "vanilla-nut-ground",
                    BasePrice = 17.00m,
                    PromotionPrice = null,
                    ShortDesc = "Sweet vanilla and toasted nut aromas.",
                    DetailDesc = "Premium ground coffee infused with natural vanilla and hazelnut flavors.",
                    ThumbnailUrl = "/images/product-artisan-espresso.png",
                    StockQty = 45,
                    ViewCount = 30,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "blend",
                    GrindType = "medium",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 33,
                    CategoryId = 2,
                    Name = "Hazelnut Cream Ground",
                    Slug = "hazelnut-cream-ground",
                    BasePrice = 17.00m,
                    PromotionPrice = null,
                    ShortDesc = "Rich, buttery hazelnut flavor.",
                    DetailDesc = "Smooth and aromatic ground coffee with a distinctive hazelnut cream finish.",
                    ThumbnailUrl = "/images/product-guatemala.png",
                    StockQty = 40,
                    ViewCount = 28,
                    Status = "active",
                    RoastLevel = "medium",
                    Region = "blend",
                    GrindType = "medium",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 34,
                    CategoryId = 2,
                    Name = "Cold Brew Coarse Ground",
                    Slug = "cold-brew-coarse-ground",
                    BasePrice = 19.00m,
                    PromotionPrice = null,
                    ShortDesc = "Specially ground for cold water extraction.",
                    DetailDesc = "A dark roasted blend ground coarsely for the perfect, smooth cold brew at home.",
                    ThumbnailUrl = "/images/hero-coffee-beans.png",
                    StockQty = 55,
                    ViewCount = 42,
                    Status = "active",
                    RoastLevel = "dark",
                    Region = "blend",
                    GrindType = "coarse",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 35,
                    CategoryId = 3,
                    Name = "French Press Maker (1L)",
                    Slug = "french-press-maker-1l",
                    BasePrice = 28.00m,
                    PromotionPrice = null,
                    ShortDesc = "Classic glass French Press with stainless steel filter.",
                    DetailDesc = "1 Liter capacity French press for a full-bodied coffee experience.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 25,
                    ViewCount = 18,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 36,
                    CategoryId = 3,
                    Name = "Digital Coffee Scale",
                    Slug = "digital-coffee-scale",
                    BasePrice = 22.00m,
                    PromotionPrice = null,
                    ShortDesc = "Precision scale with built-in timer.",
                    DetailDesc = "Accurate up to 0.1g, perfect for pour-over and espresso dialing in.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 30,
                    ViewCount = 34,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 37,
                    CategoryId = 3,
                    Name = "Coffee Knock Box",
                    Slug = "coffee-knock-box",
                    BasePrice = 18.00m,
                    PromotionPrice = null,
                    ShortDesc = "Durable knock box for espresso pucks.",
                    DetailDesc = "Sturdy plastic knock box with a removable rubber bar for easy cleaning.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 40,
                    ViewCount = 12,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 38,
                    CategoryId = 3,
                    Name = "Milk Frothing Pitcher",
                    Slug = "milk-frothing-pitcher",
                    BasePrice = 15.00m,
                    PromotionPrice = null,
                    ShortDesc = "Stainless steel pitcher (350ml) for latte art.",
                    DetailDesc = "Classic stainless steel pitcher with a precision spout for pouring latte art.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 50,
                    ViewCount = 21,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    ProductId = 39,
                    CategoryId = 3,
                    Name = "Espresso Tamper (58mm)",
                    Slug = "espresso-tamper-58mm",
                    BasePrice = 25.00m,
                    PromotionPrice = null,
                    ShortDesc = "Solid stainless steel tamper with ergonomic handle.",
                    DetailDesc = "Perfectly balanced 58mm tamper to ensure an even and consistent espresso puck.",
                    ThumbnailUrl = "/images/category-accessories.png",
                    StockQty = 20,
                    ViewCount = 26,
                    Status = "active",
                    RoastLevel = null,
                    Region = null,
                    GrindType = null,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            );
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
    }
}
