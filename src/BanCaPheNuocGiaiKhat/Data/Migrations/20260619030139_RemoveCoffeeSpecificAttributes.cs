using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanCaPheNuocGiaiKhat.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoffeeSpecificAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grind_type",
                table: "products");

            migrationBuilder.DropColumn(
                name: "region",
                table: "products");

            migrationBuilder.DropColumn(
                name: "roast_level",
                table: "products");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 8,
                column: "detail_desc",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "grind_type",
                table: "products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "region",
                table: "products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roast_level",
                table: "products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 1,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "africa", "light" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 2,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "south-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 3,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "asia-pacific", "dark" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 4,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "south-america", "espresso" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 5,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "central-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 6,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "africa", "light" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 7,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "south-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 8,
                columns: new[] { "detail_desc", "grind_type", "region", "roast_level" },
                values: new object[] { "Tarrazu region provides one of the finest beans in Central America.", "whole-bean", "central-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 9,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "medium", "africa", "light" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 10,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "coarse", "south-america", "dark" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 11,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "fine", "south-america", "espresso" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 12,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 13,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 14,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 15,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "asia-pacific", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 16,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "asia-pacific", "dark" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 17,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "south-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 18,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "medium", "south-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 19,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 20,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 21,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 22,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 23,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "caribbean", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 24,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "hawaii", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 25,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "africa", "light" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 26,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "africa", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 27,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "africa", "light" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 28,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "middle-east", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 29,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "central-america", "light" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 30,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "whole-bean", "south-america", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 31,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "medium", "blend", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 32,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "medium", "blend", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 33,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "medium", "blend", "medium" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 34,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { "coarse", "blend", "dark" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 35,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 36,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 37,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 38,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_id",
                keyValue: 39,
                columns: new[] { "grind_type", "region", "roast_level" },
                values: new object[] { null, null, null });
        }
    }
}
