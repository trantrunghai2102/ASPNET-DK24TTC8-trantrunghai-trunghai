using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanCaPheNuocGiaiKhat.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendOrderAndAddCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_address",
                table: "orders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_type",
                table: "orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "instore");

            migrationBuilder.AddColumn<string>(
                name: "payment_status",
                table: "orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "unpaid");

            migrationBuilder.AddColumn<string>(
                name: "recipient_name",
                table: "orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_phone",
                table: "orders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cart_items",
                columns: table => new
                {
                    cart_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => x.cart_item_id);
                    table.ForeignKey(
                        name: "FK_cart_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cart_items_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_product_id",
                table: "cart_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_user_id",
                table: "cart_items",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_customer_id",
                table: "orders");

            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropIndex(
                name: "IX_orders_customer_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_address",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "order_type",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "recipient_name",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "recipient_phone",
                table: "orders");
        }
    }
}
