using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BanCaPheNuocGiaiKhat.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "product_id", "base_price", "category_id", "created_at", "detail_desc", "grind_type", "name", "promotion_price", "region", "roast_level", "short_desc", "slug", "status", "stock_qty", "thumbnail_url", "updated_at", "view_count" },
                values: new object[,]
                {
                    { 23, 65.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Authentic Jamaica Blue Mountain coffee with a smooth, clean finish.", "whole-bean", "Jamaica Blue Mountain", null, "caribbean", "medium", "Mild flavor, lack of bitterness, and sweet herbal notes.", "jamaica-blue-mountain", "active", 10, "/images/product-guatemala.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 50 },
                    { 24, 55.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Grown on the slopes of Hualalai and Mauna Loa in the North and South Kona Districts.", "whole-bean", "Hawaii Kona Extra Fancy", null, "hawaii", "medium", "Rich, smooth, and deeply flavorful with a nutty aroma.", "hawaii-kona-extra-fancy", "active", 15, "/images/product-colombia.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 45 },
                    { 25, 24.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Peaberry coffee beans from the slopes of Mount Kilimanjaro.", "whole-bean", "Tanzania Peaberry", null, "africa", "light", "Bright acidity with notes of blackcurrant and chocolate.", "tanzania-peaberry", "active", 25, "/images/product-ethiopian.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 12 },
                    { 26, 21.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Bourbon varietal grown in the high altitudes of Rwanda.", "whole-bean", "Rwanda Bourbon", null, "africa", "medium", "Sweet with floral notes and hints of caramelized sugar.", "rwanda-bourbon", "active", 30, "/images/hero-coffee-beans.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 20 },
                    { 27, 22.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Sourced from the Ngozi province, featuring bright and juicy fruit notes.", "whole-bean", "Burundi Ngozi", null, "africa", "light", "Complex fruit flavors with a clean, sweet finish.", "burundi-ngozi", "active", 20, "/images/product-colombia.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 18 },
                    { 28, 35.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "One of the oldest and most traditional coffees, grown on dry terraces in Yemen.", "whole-bean", "Yemen Mocha Mattari", null, "middle-east", "medium", "Earthy, complex with intense chocolate and wine notes.", "yemen-mocha-mattari", "active", 12, "/images/product-artisan-espresso.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 35 },
                    { 29, 85.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "A highly sought-after, premium coffee varietal known for its tea-like body and floral aroma.", "whole-bean", "Panama Geisha", null, "central-america", "light", "Incredibly delicate with jasmine aroma and bergamot flavors.", "panama-geisha", "active", 5, "/images/product-guatemala.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 80 },
                    { 30, 20.00m, 1, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Processed using the Swiss Water Method to retain 99.9% of the flavor without the caffeine.", "whole-bean", "Decaf Swiss Water Blend", null, "south-america", "medium", "Smooth and rich, decaffeinated without chemicals.", "decaf-swiss-water", "active", 40, "/images/hero-coffee-beans.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 15 },
                    { 31, 16.00m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "A perfectly balanced blend of South American and Central American beans.", "medium", "Breakfast Blend Ground", null, "blend", "medium", "A bright, crisp start to the day.", "breakfast-blend-ground", "active", 60, "/images/product-colombia.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 22 },
                    { 32, 17.00m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Premium ground coffee infused with natural vanilla and hazelnut flavors.", "medium", "Vanilla Nut Ground", null, "blend", "medium", "Sweet vanilla and toasted nut aromas.", "vanilla-nut-ground", "active", 45, "/images/product-artisan-espresso.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 30 },
                    { 33, 17.00m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Smooth and aromatic ground coffee with a distinctive hazelnut cream finish.", "medium", "Hazelnut Cream Ground", null, "blend", "medium", "Rich, buttery hazelnut flavor.", "hazelnut-cream-ground", "active", 40, "/images/product-guatemala.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 28 },
                    { 34, 19.00m, 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "A dark roasted blend ground coarsely for the perfect, smooth cold brew at home.", "coarse", "Cold Brew Coarse Ground", null, "blend", "dark", "Specially ground for cold water extraction.", "cold-brew-coarse-ground", "active", 55, "/images/hero-coffee-beans.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 42 },
                    { 35, 28.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "1 Liter capacity French press for a full-bodied coffee experience.", null, "French Press Maker (1L)", null, null, null, "Classic glass French Press with stainless steel filter.", "french-press-maker-1l", "active", 25, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 18 },
                    { 36, 22.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Accurate up to 0.1g, perfect for pour-over and espresso dialing in.", null, "Digital Coffee Scale", null, null, null, "Precision scale with built-in timer.", "digital-coffee-scale", "active", 30, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 34 },
                    { 37, 18.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Sturdy plastic knock box with a removable rubber bar for easy cleaning.", null, "Coffee Knock Box", null, null, null, "Durable knock box for espresso pucks.", "coffee-knock-box", "active", 40, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 12 },
                    { 38, 15.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Classic stainless steel pitcher with a precision spout for pouring latte art.", null, "Milk Frothing Pitcher", null, null, null, "Stainless steel pitcher (350ml) for latte art.", "milk-frothing-pitcher", "active", 50, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 21 },
                    { 39, 25.00m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Perfectly balanced 58mm tamper to ensure an even and consistent espresso puck.", null, "Espresso Tamper (58mm)", null, null, null, "Solid stainless steel tamper with ergonomic handle.", "espresso-tamper-58mm", "active", 20, "/images/category-accessories.png", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 26 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 39);
        }
    }
}
