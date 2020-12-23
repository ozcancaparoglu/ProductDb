using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductDb.Data.Migrations
{
    public partial class DbInitializer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    IsVariantable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Prefix = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CargoType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    ParentCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Abbrevation = table.Column<string>(maxLength: 10, nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LiveValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErpCompany",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ErpRef = table.Column<int>(nullable: false),
                    FirmNo = table.Column<int>(nullable: false),
                    FirmName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErpCompany", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormulaGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormulaGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Abbrevation = table.Column<string>(maxLength: 10, nullable: false),
                    LogoPath = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    name = table.Column<string>(maxLength: 200, nullable: false),
                    key = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Attribute = table.Column<string>(nullable: true),
                    AttributeValueId = table.Column<int>(nullable: true),
                    AttributeValue = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    ManufacturerPartNumber = table.Column<string>(maxLength: 100, nullable: true),
                    Prefix = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 750, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VatRate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatRate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseQuery",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    WarehouseTypeId = table.Column<int>(nullable: false),
                    WarehouseName = table.Column<string>(nullable: true),
                    Query = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LogoWarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributesValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    AttributesId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributesValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributesValue_Attributes_AttributesId",
                        column: x => x.AttributesId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAttributeMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: true),
                    AttributesId = table.Column<int>(nullable: true),
                    IsRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttributeMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAttributeMapping_Attributes_AttributesId",
                        column: x => x.AttributesId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryAttributeMapping_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Formula",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    FormulaGroupId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    FormulaStr = table.Column<string>(maxLength: 1000, nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Result = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formula", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formula_FormulaGroup_FormulaGroupId",
                        column: x => x.FormulaGroupId,
                        principalTable: "FormulaGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: true),
                    CargoCurrencyId = table.Column<int>(nullable: true),
                    FormulaGroupId = table.Column<int>(nullable: true),
                    CargoTypeId = table.Column<int>(nullable: true),
                    ErpCompanyId = table.Column<int>(nullable: true),
                    MinStock = table.Column<int>(nullable: false),
                    MaxStock = table.Column<int>(nullable: false),
                    MinPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinPoint = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxPoint = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DefaultMarj = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjectCode = table.Column<string>(nullable: true),
                    Sarf = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Store_Currency_CargoCurrencyId",
                        column: x => x.CargoCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_CargoType_CargoTypeId",
                        column: x => x.CargoTypeId,
                        principalTable: "CargoType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_ErpCompany_ErpCompanyId",
                        column: x => x.ErpCompanyId,
                        principalTable: "ErpCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_FormulaGroup_FormulaGroupId",
                        column: x => x.FormulaGroupId,
                        principalTable: "FormulaGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LanguageValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false),
                    TableName = table.Column<string>(nullable: true),
                    FieldName = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageValues_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Sku = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true),
                    BrandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentProduct_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentProduct_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentProduct_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDockCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    ParentCategoryId = table.Column<int>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDockCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDockCategory_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false),
                    UserRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Username = table.Column<string>(maxLength: 750, nullable: false),
                    Email = table.Column<string>(maxLength: 750, nullable: false),
                    Password = table.Column<byte[]>(nullable: true),
                    UserRoleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cargo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    MinDesi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxDesi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsLastDesi = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cargo_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Margin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    MarginTypeId = table.Column<int>(nullable: true),
                    Profit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntityId = table.Column<int>(nullable: false),
                    SecondEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Margin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Margin_MarginType_MarginTypeId",
                        column: x => x.MarginTypeId,
                        principalTable: "MarginType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Margin_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreCategoryMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    ErpCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategoryMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreCategoryMapping_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreCategoryMapping_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreWarehouseMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    WarehouseTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreWarehouseMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreWarehouseMapping_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreWarehouseMapping_WarehouseType_WarehouseTypeId",
                        column: x => x.WarehouseTypeId,
                        principalTable: "WarehouseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transportation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    TransportationTypeId = table.Column<int>(nullable: true),
                    EntityId = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrencyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transportation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transportation_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transportation_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transportation_TransportationType_TransportationTypeId",
                        column: x => x.TransportationTypeId,
                        principalTable: "TransportationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Sku = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Barcode = table.Column<string>(nullable: false),
                    Gtip = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    Alias = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    SearchEngineTerms = table.Column<string>(nullable: true),
                    Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AbroadDesi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BuyingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PsfPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CorporatePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DdpPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: true),
                    ParentProductId = table.Column<int>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: true),
                    VatRateId = table.Column<int>(nullable: true),
                    SupplierUniqueId = table.Column<int>(nullable: true),
                    ProductGroupId = table.Column<int>(nullable: true),
                    CorporateCurrencyId = table.Column<int>(nullable: true),
                    PsfCurrencyId = table.Column<int>(nullable: true),
                    DdpCurrencyId = table.Column<int>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true),
                    BrandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Currency_CorporateCurrencyId",
                        column: x => x.CorporateCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Currency_DdpCurrencyId",
                        column: x => x.DdpCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_ParentProduct_ParentProductId",
                        column: x => x.ParentProductId,
                        principalTable: "ParentProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_ProductGroup_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "ProductGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Currency_PsfCurrencyId",
                        column: x => x.PsfCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_VatRate_VatRateId",
                        column: x => x.VatRateId,
                        principalTable: "VatRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentProductDock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Sku = table.Column<string>(nullable: false),
                    ProductDockCategoryId = table.Column<int>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentProductDock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentProductDock_ProductDockCategory_ProductDockCategoryId",
                        column: x => x.ProductDockCategoryId,
                        principalTable: "ProductDockCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentProductDock_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    Alt = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    LocalPath = table.Column<string>(maxLength: 5000, nullable: false),
                    CdnPath = table.Column<string>(maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pictures_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    AttributesId = table.Column<int>(nullable: true),
                    AttributeValueId = table.Column<int>(nullable: true),
                    RequiredAttributeValue = table.Column<string>(nullable: true),
                    IsRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeMapping_AttributesValue_AttributeValueId",
                        column: x => x.AttributeValueId,
                        principalTable: "AttributesValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeMapping_Attributes_AttributesId",
                        column: x => x.AttributesId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeMapping_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductBuyingPriceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBuyingPriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBuyingPriceHistory_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductErpCompanyMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ErpCompanyId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductErpCompanyMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductErpCompanyMapping_ErpCompany_ErpCompanyId",
                        column: x => x.ErpCompanyId,
                        principalTable: "ErpCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductErpCompanyMapping_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariant",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ParentProductId = table.Column<int>(nullable: false),
                    BaseId = table.Column<int>(nullable: true),
                    ProductAttributes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariant_ParentProduct_ParentProductId",
                        column: x => x.ParentProductId,
                        principalTable: "ParentProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariant_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    Stock = table.Column<int>(nullable: false),
                    StoreProductId = table.Column<int>(nullable: false),
                    StoreParentProductId = table.Column<int>(nullable: false),
                    BaseStoreId = table.Column<int>(nullable: false),
                    StoreCategory = table.Column<string>(nullable: true),
                    IsRealStock = table.Column<bool>(nullable: false),
                    IsSend = table.Column<bool>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Point = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsFixed = table.Column<bool>(nullable: true),
                    IsFixedPoint = table.Column<bool>(nullable: true),
                    ErpPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ErpPoint = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CatalogCode = table.Column<string>(nullable: true),
                    CatalogName = table.Column<string>(nullable: true),
                    VatValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductMapping_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreProductMapping_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseProductStock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Sku = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    WarehouseTypeId = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseProductStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseProductStock_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseProductStock_WarehouseType_WarehouseTypeId",
                        column: x => x.WarehouseTypeId,
                        principalTable: "WarehouseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    FullDescription = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    Sku = table.Column<string>(nullable: true),
                    Gtin = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    SupplierUniqueId = table.Column<int>(nullable: true),
                    PsfPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ParentProductDockId = table.Column<int>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true),
                    ProductDockCategoryId = table.Column<int>(nullable: true),
                    VatRateId = table.Column<int>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CorporatePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BulletPoint1 = table.Column<string>(nullable: true),
                    BulletPoint2 = table.Column<string>(nullable: true),
                    BulletPoint3 = table.Column<string>(nullable: true),
                    BulletPoint4 = table.Column<string>(nullable: true),
                    BulletPoint5 = table.Column<string>(nullable: true),
                    Stock = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDock_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDock_ParentProductDock_ParentProductDockId",
                        column: x => x.ParentProductDockId,
                        principalTable: "ParentProductDock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDock_ProductDockCategory_ProductDockCategoryId",
                        column: x => x.ProductDockCategoryId,
                        principalTable: "ProductDockCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDock_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDock_VatRate_VatRateId",
                        column: x => x.VatRateId,
                        principalTable: "VatRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDockAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductDockId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDockAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDockAttribute_ProductDock_ProductDockId",
                        column: x => x.ProductDockId,
                        principalTable: "ProductDock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDockPictures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: true),
                    ProcessedBy = table.Column<int>(nullable: false),
                    ProductDockId = table.Column<int>(nullable: true),
                    DownloadUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDockPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDockPictures_ProductDock_ProductDockId",
                        column: x => x.ProductDockId,
                        principalTable: "ProductDock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributesValue_AttributesId",
                table: "AttributesValue",
                column: "AttributesId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargo_StoreId",
                table: "Cargo",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributeMapping_AttributesId",
                table: "CategoryAttributeMapping",
                column: "AttributesId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributeMapping_CategoryId",
                table: "CategoryAttributeMapping",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Formula_FormulaGroupId",
                table: "Formula",
                column: "FormulaGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageValues_LanguageId",
                table: "LanguageValues",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Margin_MarginTypeId",
                table: "Margin",
                column: "MarginTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Margin_StoreId",
                table: "Margin",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentProduct_BrandId",
                table: "ParentProduct",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentProduct_CategoryId",
                table: "ParentProduct",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentProduct_SupplierId",
                table: "ParentProduct",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentProductDock_ProductDockCategoryId",
                table: "ParentProductDock",
                column: "ProductDockCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentProductDock_SupplierId",
                table: "ParentProductDock",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_ProductId",
                table: "Pictures",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandId",
                table: "Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CorporateCurrencyId",
                table: "Product",
                column: "CorporateCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CurrencyId",
                table: "Product",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_DdpCurrencyId",
                table: "Product",
                column: "DdpCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ParentProductId",
                table: "Product",
                column: "ParentProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductGroupId",
                table: "Product",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_PsfCurrencyId",
                table: "Product",
                column: "PsfCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SupplierId",
                table: "Product",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_VatRateId",
                table: "Product",
                column: "VatRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeMapping_AttributeValueId",
                table: "ProductAttributeMapping",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeMapping_AttributesId",
                table: "ProductAttributeMapping",
                column: "AttributesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeMapping_ProductId",
                table: "ProductAttributeMapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBuyingPriceHistory_ProductId",
                table: "ProductBuyingPriceHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDock_CurrencyId",
                table: "ProductDock",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDock_ParentProductDockId",
                table: "ProductDock",
                column: "ParentProductDockId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDock_ProductDockCategoryId",
                table: "ProductDock",
                column: "ProductDockCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDock_SupplierId",
                table: "ProductDock",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDock_VatRateId",
                table: "ProductDock",
                column: "VatRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDockAttribute_ProductDockId",
                table: "ProductDockAttribute",
                column: "ProductDockId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDockCategory_SupplierId",
                table: "ProductDockCategory",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDockPictures_ProductDockId",
                table: "ProductDockPictures",
                column: "ProductDockId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductErpCompanyMapping_ErpCompanyId",
                table: "ProductErpCompanyMapping",
                column: "ErpCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductErpCompanyMapping_ProductId",
                table: "ProductErpCompanyMapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ParentProductId",
                table: "ProductVariant",
                column: "ParentProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductId",
                table: "ProductVariant",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_UserRoleId",
                table: "RolePermission",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_CargoCurrencyId",
                table: "Store",
                column: "CargoCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_CargoTypeId",
                table: "Store",
                column: "CargoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_CurrencyId",
                table: "Store",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_ErpCompanyId",
                table: "Store",
                column: "ErpCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_FormulaGroupId",
                table: "Store",
                column: "FormulaGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCategoryMapping_CategoryId",
                table: "StoreCategoryMapping",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCategoryMapping_StoreId",
                table: "StoreCategoryMapping",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductMapping_ProductId",
                table: "StoreProductMapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductMapping_StoreId",
                table: "StoreProductMapping",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreWarehouseMapping_StoreId",
                table: "StoreWarehouseMapping",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreWarehouseMapping_WarehouseTypeId",
                table: "StoreWarehouseMapping",
                column: "WarehouseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportation_CurrencyId",
                table: "Transportation",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportation_StoreId",
                table: "Transportation",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportation_TransportationTypeId",
                table: "Transportation",
                column: "TransportationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserRoleId",
                table: "User",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProductStock_ProductId",
                table: "WarehouseProductStock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProductStock_WarehouseTypeId",
                table: "WarehouseProductStock",
                column: "WarehouseTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "CategoryAttributeMapping");

            migrationBuilder.DropTable(
                name: "Formula");

            migrationBuilder.DropTable(
                name: "LanguageValues");

            migrationBuilder.DropTable(
                name: "Margin");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "ProductAttributeMapping");

            migrationBuilder.DropTable(
                name: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "ProductBuyingPriceHistory");

            migrationBuilder.DropTable(
                name: "ProductDockAttribute");

            migrationBuilder.DropTable(
                name: "ProductDockPictures");

            migrationBuilder.DropTable(
                name: "ProductErpCompanyMapping");

            migrationBuilder.DropTable(
                name: "ProductVariant");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "StoreCategoryMapping");

            migrationBuilder.DropTable(
                name: "StoreProductMapping");

            migrationBuilder.DropTable(
                name: "StoreWarehouseMapping");

            migrationBuilder.DropTable(
                name: "Transportation");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "WarehouseProductStock");

            migrationBuilder.DropTable(
                name: "WarehouseQuery");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "MarginType");

            migrationBuilder.DropTable(
                name: "AttributesValue");

            migrationBuilder.DropTable(
                name: "ProductDock");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "TransportationType");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "WarehouseType");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "ParentProductDock");

            migrationBuilder.DropTable(
                name: "CargoType");

            migrationBuilder.DropTable(
                name: "ErpCompany");

            migrationBuilder.DropTable(
                name: "FormulaGroup");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "ParentProduct");

            migrationBuilder.DropTable(
                name: "ProductGroup");

            migrationBuilder.DropTable(
                name: "VatRate");

            migrationBuilder.DropTable(
                name: "ProductDockCategory");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
