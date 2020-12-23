using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class ProjectEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckingAccountToBeOpened",
                table: "Projects",
                newName: "CheckingAccountToBeCreated");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CargoCurrency",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CargoPrice",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CatalogCode",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Consumable",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsumableCurrency",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "OrderItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "OrderItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProjectProduct",
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
                    ProjectId = table.Column<int>(nullable: false),
                    ProjectId1 = table.Column<long>(nullable: true),
                    CatalogCode = table.Column<string>(nullable: true),
                    LogoCode = table.Column<string>(nullable: true),
                    CatalogDescription = table.Column<string>(nullable: true),
                    TaxRate = table.Column<int>(nullable: true),
                    Consumable = table.Column<decimal>(nullable: true),
                    ConsumableCurrency = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    PriceCurrency = table.Column<string>(nullable: true),
                    ProductWeight = table.Column<int>(nullable: true),
                    GroupCode = table.Column<string>(nullable: true),
                    CargoPrice = table.Column<decimal>(nullable: true),
                    CargoCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProduct_Projects_ProjectId1",
                        column: x => x.ProjectId1,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProduct_ProjectId1",
                table: "ProjectProduct",
                column: "ProjectId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectProduct");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CargoCurrency",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CargoPrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CatalogCode",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Consumable",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ConsumableCurrency",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "CheckingAccountToBeCreated",
                table: "Projects",
                newName: "CheckingAccountToBeOpened");
        }
    }
}
