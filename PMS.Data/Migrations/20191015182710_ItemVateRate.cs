using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class ItemVateRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShipmentDefination",
                table: "Invoices",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation1",
                table: "Invoices",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Invoices",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Invoices",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNo",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone1",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone2",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemVatRates",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    isDeleted = table.Column<bool>(nullable: false),
                    ItemCode = table.Column<string>(maxLength: 50, nullable: true),
                    VateRate = table.Column<int>(nullable: false),
                    Defination = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVatRates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemVatRates");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxNo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Telephone1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Telephone2",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Invoices");

            migrationBuilder.AlterColumn<string>(
                name: "ShipmentDefination",
                table: "Invoices",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation1",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
