using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductDb.Data.Migrations
{
    public partial class ProductFobPriceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FobCurrencyId",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FobPrice",
                table: "Product",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_FobCurrencyId",
                table: "Product",
                column: "FobCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Currency_FobCurrencyId",
                table: "Product",
                column: "FobCurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Currency_FobCurrencyId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_FobCurrencyId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "FobCurrencyId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "FobPrice",
                table: "Product");
        }
    }
}
