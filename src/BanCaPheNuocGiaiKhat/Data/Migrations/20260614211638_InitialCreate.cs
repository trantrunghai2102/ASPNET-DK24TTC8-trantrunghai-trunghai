using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BanCaPheNuocGiaiKhat.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category_id);
                    table.ForeignKey(
                        name: "FK_categories_categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    base_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    promotion_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    short_desc = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    detail_desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thumbnail_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    stock_qty = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    view_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "active"),
                    roast_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    grind_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_products_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)3),
                    full_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    avatar_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    google_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    facebook_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "active"),
                    last_login_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    is_primary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    sort_order = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_product_images_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "category_id", "name", "parent_id" },
                values: new object[,]
                {
                    { 1, "Cà phê hạt", null },
                    { 2, "Cà phê bột", null },
                    { 3, "Dụng cụ pha chế", null }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "description", "role_name" },
                values: new object[,]
                {
                    { (byte)1, "Quản trị viên hệ thống, toàn quyền.", "admin" },
                    { (byte)2, "Nhân viên, quản lý đơn hàng và sản phẩm.", "staff" },
                    { (byte)3, "Khách hàng đăng ký tài khoản mua hàng.", "customer" }
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "product_id", "base_price", "category_id", "created_at", "detail_desc", "grind_type", "name", "promotion_price", "region", "roast_level", "short_desc", "slug", "status", "stock_qty", "thumbnail_url", "updated_at", "view_count" },
                values: new object[,]
                {
                    { 1, 22.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ethiopian Yirgacheffe coffee is dry-processed, bringing out the bright, clean taste profile. Notes of citrus and floral aromas are dominant.", "whole-bean", "Ethiopia Yirgacheffe", null, "africa", "light", "Floral notes with hints of jasmine and bright citrus. Light roast.", "ethiopia-yirgacheffe", "active", 50, "/images/product-ethiopian.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 12 },
                    { 2, 19.50m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Colombia Supremo beans are carefully selected and roasted to a medium profile to balance the natural acidity with nutty tones.", "whole-bean", "Colombia Supremo", null, "south-america", "medium", "Smooth, balanced body with caramel sweetness and a nutty finish. Medium roast.", "colombia-supremo", "active", 35, "/images/product-colombia.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 8 },
                    { 3, 21.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Sumatra Mandheling coffee is known for its heavy body and low acidity, featuring unique cedar wood and herbal notes.", "whole-bean", "Sumatra Mandheling", null, "asia-pacific", "dark", "Full-bodied, earthy, and complex with low acidity. Dark roast.", "sumatra-mandheling", "active", 40, "/images/hero-coffee-beans.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 20 },
                    { 4, 24.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Our signature espresso blend combines beans from South America and Africa, roasted dark to bring out rich cocoa and toasted nut flavors.", "whole-bean", "Artisan Espresso Blend", null, "south-america", "espresso", "Rich crema, notes of dark chocolate and toasted almonds. Espresso roast.", "artisan-espresso-blend", "active", 25, "/images/product-artisan-espresso.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 15 },
                    { 5, 20.50m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Guatemala Antigua beans are grown in volcanic soil, resulting in a unique smoky flavor combined with a light citrus brightness.", "whole-bean", "Guatemala Antigua", null, "central-america", "medium", "Spicy and smoky undertones with a crisp apple acidity. Medium roast.", "guatemala-antigua", "active", 30, "/images/product-guatemala.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 6, 23.50m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Kenya AA is processed using wet method, offering an exceptionally intense and clean cup.", "whole-bean", "Kenya AA", null, "africa", "light", "Bright acidity with blackcurrant and berry notes.", "kenya-aa", "active", 45, "/images/product-ethiopian.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 28 },
                    { 7, 18.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Classic Brazilian coffee bean, roasted to highlight its natural sweetness.", "whole-bean", "Brazil Santos", null, "south-america", "medium", "Low acidity, smooth body with sweet chocolate notes.", "brazil-santos", "active", 60, "/images/product-colombia.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 33 },
                    { 8, 20.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Tarrazu region provides one of the finest beans in Central America.", "whole-bean", "Costa Rica Tarrazu", null, "central-america", "medium", "Crisp acidity with sweet honey and citrus flavors.", "costa-rica-tarrazu", "active", 25, "/images/product-guatemala.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 19 },
                    { 9, 21.50m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ground Ethiopian Sidamo coffee ready for drip brewing.", "medium", "Ethiopian Sidamo Ground", null, "africa", "light", "Floral aroma with notes of lemon and cane sugar.", "ethiopian-sidamo-ground", "active", 30, "/images/product-ethiopian.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 14 },
                    { 10, 19.00m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ground French roast ideal for French Press brewing.", "coarse", "French Roast Blend", null, "south-america", "dark", "Intensely dark and smoky with a heavy body.", "french-roast-blend", "active", 40, "/images/hero-coffee-beans.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 11 },
                    { 11, 19.50m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Fine ground Italian roast optimized for espresso machines.", "fine", "Italian Roast Espresso Ground", null, "south-america", "espresso", "Rich, dark roast optimized for espresso brewing.", "italian-roast-espresso-ground", "active", 50, "/images/product-artisan-espresso.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 22 },
                    { 12, 25.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Premium Japanese ceramic pour-over dripper.", null, "Hario V60 Ceramic", null, null, null, "Classic ceramic coffee dripper for pour-over brewing.", "hario-v60-ceramic", "active", 20, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 9 },
                    { 13, 45.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Chemex glass coffeemaker with wood collar and leather tie.", null, "Chemex 6-Cup", null, null, null, "Elegant glass coffee maker for clean and pure flavor.", "chemex-6-cup", "active", 15, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 18 },
                    { 14, 39.99m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Aeropress travel version with convenient mug/carrying case.", null, "Aeropress Go", null, null, null, "Compact coffee press for travel and convenience.", "aeropress-go", "active", 25, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 25 },
                    { 15, 22.50m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Java Preanger beans from West Java volcanic highlands.", "whole-bean", "Java Preanger", null, "asia-pacific", "medium", "Traditional Indonesian cup with sweet herbal nuances.", "java-preanger", "active", 30, "/images/hero-coffee-beans.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 7 },
                    { 16, 16.50m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Buon Ma Thuot high-grade robusta beans with strong flavor.", "whole-bean", "Vietnam Robusta Special", null, "asia-pacific", "dark", "Bold, high-caffeine Robusta beans from Buon Ma Thuot.", "vietnam-robusta-special", "active", 80, "/images/product-colombia.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 42 },
                    { 17, 19.90m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Single origin Peru coffee beans.", "whole-bean", "Peru Cajamarca", null, "south-america", "medium", "Mellow body with subtle apple acidity and caramel notes.", "peru-cajamarca", "active", 35, "/images/product-guatemala.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 13 },
                    { 18, 17.50m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ground house blend of South American and African beans.", "medium", "House Blend Ground", null, "south-america", "medium", "Our balanced house blend for everyday brewing.", "house-blend-ground", "active", 70, "/images/product-artisan-espresso.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 16 },
                    { 19, 35.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Bialetti Moka Express 3-Cup stovetop espresso maker.", null, "Moka Pot Bialetti", null, null, null, "Iconic Italian stovetop espresso maker.", "moka-pot-bialetti", "active", 18, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 21 },
                    { 20, 30.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Kalita Wave stainless steel 185 pour-over dripper.", null, "Kalita Wave Stainless", null, null, null, "Flat-bottom coffee dripper for even extraction.", "kalita-wave-stainless", "active", 12, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 10 },
                    { 21, 55.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Porlex Mini stainless steel hand coffee grinder with ceramic burrs.", null, "Porlex Mini Grinder", null, null, null, "Portable ceramic burr hand grinder.", "porlex-mini-grinder", "active", 10, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 31 },
                    { 22, 40.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Stovetop gooseneck kettle for precise water flow control.", null, "Gooseneck Kettle", null, null, null, "Stainless steel kettle with precise pour control.", "gooseneck-kettle", "active", 15, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 17 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_parent_id",
                table: "categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_product_id",
                table: "product_images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_slug",
                table: "products",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_role_name",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_facebook_id",
                table: "users",
                column: "facebook_id",
                unique: true,
                filter: "[facebook_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_google_id",
                table: "users",
                column: "google_id",
                unique: true,
                filter: "[google_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
