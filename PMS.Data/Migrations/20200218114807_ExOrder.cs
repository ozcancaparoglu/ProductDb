using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class ExOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isExternal",
                table: "Orders");

            migrationBuilder.CreateTable(
                name: "ExOrder",
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
                    OrderNo = table.Column<string>(maxLength: 50, nullable: true),
                    ProjectCode = table.Column<string>(maxLength: 50, nullable: true),
                    ShipmentCode = table.Column<string>(maxLength: 50, nullable: true),
                    BillingAddressName = table.Column<string>(maxLength: 100, nullable: true),
                    BillingCompany = table.Column<string>(maxLength: 200, nullable: true),
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
                    IsTransferred = table.Column<bool>(nullable: false),
                    IsAccountCreated = table.Column<bool>(nullable: false),
                    IsAccountShippingCreated = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    LastTryDate = table.Column<DateTime>(nullable: true),
                    LogoTransferedId = table.Column<int>(nullable: false),
                    LogoCompanyCode = table.Column<int>(nullable: false),
                    isExternal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExOrderItems",
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
                    EmailSent = table.Column<bool>(nullable: false),
                    IARSent = table.Column<bool>(nullable: false),
                    IsInChequeManage = table.Column<bool>(nullable: false),
                    CargoPrice = table.Column<decimal>(nullable: true),
                    Consumable = table.Column<decimal>(nullable: true),
                    CargoCurrency = table.Column<string>(nullable: true),
                    ConsumableCurrency = table.Column<string>(nullable: true),
                    CatalogCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExOrderItems_ExOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ExOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExOrderItems_OrderId",
                table: "ExOrderItems",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExOrderItems");

            migrationBuilder.DropTable(
                name: "ExOrder");

            migrationBuilder.AddColumn<bool>(
                name: "isExternal",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }
    }
}
