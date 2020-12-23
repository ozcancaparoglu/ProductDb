using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class IsAccountCreatedIsAccountShippinngCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsTransferred",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccountCreated",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccountShippingCreated",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountCreated",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsAccountShippingCreated",
                table: "Orders");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTransferred",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
