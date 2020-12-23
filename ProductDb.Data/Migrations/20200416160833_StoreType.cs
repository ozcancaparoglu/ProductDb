using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductDb.Data.Migrations
{
    public partial class StoreType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreTypeId",
                table: "Store",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Store_StoreTypeId",
                table: "Store",
                column: "StoreTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_StoreType_StoreTypeId",
                table: "Store",
                column: "StoreTypeId",
                principalTable: "StoreType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Store_StoreType_StoreTypeId",
                table: "Store");

            migrationBuilder.DropTable(
                name: "StoreType");

            migrationBuilder.DropIndex(
                name: "IX_Store_StoreTypeId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "StoreTypeId",
                table: "Store");
        }
    }
}
