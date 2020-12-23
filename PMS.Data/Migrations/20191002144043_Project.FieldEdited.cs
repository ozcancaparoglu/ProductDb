using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class ProjectFieldEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Departmant",
                table: "Projects",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Projects",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DueDateDifference",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Factory",
                table: "Projects",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PointIsOrderDiscount",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PriceControl",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PriceDifference",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectGroupCode",
                table: "Projects",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectType",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Source_Cost_GRP",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TaxIncluded",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Warehouse",
                table: "Projects",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogoTransferedId",
                table: "Orders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departmant",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DueDateDifference",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Factory",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PointIsOrderDiscount",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PriceControl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PriceDifference",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectGroupCode",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectType",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Source_Cost_GRP",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TaxIncluded",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Warehouse",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LogoTransferedId",
                table: "Orders");
        }
    }
}
