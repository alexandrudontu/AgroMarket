using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixedOrderCustomerRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_BuyerId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_FarmerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BuyerId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BuyerId1",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_FarmerId",
                table: "Products",
                column: "FarmerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_FarmerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BuyerId1",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BuyerId1",
                table: "Orders",
                column: "BuyerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_BuyerId1",
                table: "Orders",
                column: "BuyerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_FarmerId",
                table: "Products",
                column: "FarmerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
