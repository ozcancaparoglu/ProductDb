using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    OrderNo = table.Column<string>(maxLength: 50, nullable: true),
                    ProjectCode = table.Column<string>(maxLength: 50, nullable: true),
                    ShipmentCode = table.Column<string>(maxLength: 50, nullable: true),
                    BillingAddressName = table.Column<string>(maxLength: 100, nullable: true),
                    BillingAddress = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    BillingTown = table.Column<string>(maxLength: 100, nullable: true),
                    BillingCity = table.Column<string>(maxLength: 100, nullable: true),
                    BillingCountry = table.Column<string>(maxLength: 100, nullable: true),
                    BillingTelNo1 = table.Column<string>(maxLength: 50, nullable: true),
                    BillingTelNo2 = table.Column<string>(maxLength: 50, nullable: true),
                    BillingTelNo3 = table.Column<string>(maxLength: 50, nullable: true),
                    BillingPostalCode = table.Column<string>(maxLength: 50, nullable: true),
                    BillingIdentityNumber = table.Column<string>(maxLength: 15, nullable: true),
                    BillingTaxOffice = table.Column<string>(maxLength: 50, nullable: true),
                    BillingTaxNumber = table.Column<string>(maxLength: 15, nullable: true),
                    BillingEmail = table.Column<string>(maxLength: 150, nullable: true),
                    ShippingAddressName = table.Column<string>(maxLength: 100, nullable: true),
                    ShippingAddress = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ShippingTown = table.Column<string>(maxLength: 100, nullable: true),
                    ShippingCity = table.Column<string>(maxLength: 100, nullable: true),
                    ShippingCountry = table.Column<string>(maxLength: 100, nullable: true),
                    ShippingTelNo1 = table.Column<string>(maxLength: 50, nullable: true),
                    ShippingTelNo2 = table.Column<string>(maxLength: 50, nullable: true),
                    ShippingTelNo3 = table.Column<string>(maxLength: 50, nullable: true),
                    ShippingPostalCode = table.Column<string>(maxLength: 50, nullable: true),
                    ShippingEmail = table.Column<string>(maxLength: 150, nullable: true),
                    CollectionViaCreditCard = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    CollectionViaTransfer = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Explanation1 = table.Column<string>(maxLength: 100, nullable: true),
                    Explanation2 = table.Column<string>(maxLength: 100, nullable: true),
                    Explanation3 = table.Column<string>(maxLength: 100, nullable: true),
                    IsTransferred = table.Column<bool>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    LastTryDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ProjectPrefix = table.Column<string>(maxLength: 50, nullable: true),
                    ProjectCode = table.Column<string>(maxLength: 50, nullable: true),
                    ProjectName = table.Column<string>(maxLength: 200, nullable: true),
                    CheckingAccount = table.Column<string>(maxLength: 200, nullable: true),
                    CheckingAccountToBeOpened = table.Column<bool>(nullable: true),
                    CheckingAccountCode = table.Column<string>(maxLength: 50, nullable: true),
                    LogoFirmCode = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<long>(nullable: false),
                    SKU = table.Column<string>(maxLength: 50, nullable: true),
                    ProductName = table.Column<string>(maxLength: 200, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    VAT = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Currency = table.Column<string>(maxLength: 5, nullable: true),
                    Desi = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    PointsValue = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    EmailSent = table.Column<bool>(nullable: true),
                    IARSent = table.Column<bool>(nullable: true),
                    IsInChequeManage = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
