﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PMS.Data;

namespace PMS.Data.Migrations
{
    [DbContext(typeof(BiggOfficeContext))]
    [Migration("20200304152936_LogoCompanySetting1")]
    partial class LogoCompanySetting1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PMS.Data.Entities.Invoice.Invoice", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address1")
                        .HasMaxLength(200);

                    b.Property<string>("Address2")
                        .HasMaxLength(200);

                    b.Property<string>("City")
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("Explanation1")
                        .HasMaxLength(100);

                    b.Property<string>("Explanation2")
                        .HasMaxLength(100);

                    b.Property<string>("Explanation3")
                        .HasMaxLength(100);

                    b.Property<int>("Genexctyp");

                    b.Property<string>("InvoiceDate");

                    b.Property<string>("InvoiceNo")
                        .HasMaxLength(50);

                    b.Property<bool>("IsTransferred");

                    b.Property<int>("Lineexctyp");

                    b.Property<int>("LogoCompanyCode");

                    b.Property<int>("LogoTransferedId");

                    b.Property<string>("OrderNo")
                        .HasMaxLength(50);

                    b.Property<string>("ProjectCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShipmentCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShipmentDefination")
                        .HasMaxLength(200);

                    b.Property<string>("TaxNo")
                        .HasMaxLength(50);

                    b.Property<string>("TaxOffice")
                        .HasMaxLength(50);

                    b.Property<string>("Telephone1")
                        .HasMaxLength(50);

                    b.Property<string>("Telephone2")
                        .HasMaxLength(50);

                    b.Property<string>("Town")
                        .HasMaxLength(50);

                    b.Property<int>("Type");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.Property<bool>("isEINVOICE");

                    b.HasKey("Id");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("PMS.Data.Entities.Invoice.InvoiceItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Currency")
                        .HasMaxLength(5);

                    b.Property<string>("Defination");

                    b.Property<long>("InvoiceId");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<int>("Quantity");

                    b.Property<string>("SKU")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<decimal>("VAT")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceItems");
                });

            modelBuilder.Entity("PMS.Data.Entities.Item.ItemVatRate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Defination")
                        .HasMaxLength(200);

                    b.Property<string>("ItemCode")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<int>("VateRate");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.ToTable("ItemVatRates");
                });

            modelBuilder.Entity("PMS.Data.Entities.Logo.LogoCompany", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<int>("CompanyId");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name");

                    b.Property<string>("Setting1");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.ToTable("LogoCompanies");
                });

            modelBuilder.Entity("PMS.Data.Entities.Logs.Logs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CompanyId");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityKey");

                    b.Property<string>("Message");

                    b.Property<string>("Message2");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("PMS.Data.Entities.Order.ExOrder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BillingAddress")
                        .HasColumnType("NVARCHAR(MAX)");

                    b.Property<string>("BillingAddressName")
                        .HasMaxLength(100);

                    b.Property<string>("BillingCity")
                        .HasMaxLength(100);

                    b.Property<string>("BillingCompany")
                        .HasMaxLength(200);

                    b.Property<string>("BillingCountry")
                        .HasMaxLength(100);

                    b.Property<string>("BillingEmail")
                        .HasMaxLength(150);

                    b.Property<string>("BillingIdentityNumber")
                        .HasMaxLength(15);

                    b.Property<string>("BillingPostalCode")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTaxNumber")
                        .HasMaxLength(15);

                    b.Property<string>("BillingTaxOffice")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTelNo1")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTelNo2")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTelNo3")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTown")
                        .HasMaxLength(100);

                    b.Property<decimal>("CollectionViaCreditCard")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("CollectionViaTransfer")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("Explanation1")
                        .HasMaxLength(100);

                    b.Property<string>("Explanation2")
                        .HasMaxLength(100);

                    b.Property<string>("Explanation3")
                        .HasMaxLength(100);

                    b.Property<bool>("IsAccountCreated");

                    b.Property<bool>("IsAccountShippingCreated");

                    b.Property<bool>("IsTransferred");

                    b.Property<DateTime?>("LastTryDate");

                    b.Property<int>("LogoCompanyCode");

                    b.Property<int>("LogoTransferedId");

                    b.Property<DateTime?>("OrderDate");

                    b.Property<string>("OrderNo")
                        .HasMaxLength(50);

                    b.Property<string>("ProjectCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShipmentCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingAddress")
                        .HasColumnType("NVARCHAR(MAX)");

                    b.Property<string>("ShippingAddressName")
                        .HasMaxLength(100);

                    b.Property<string>("ShippingCity")
                        .HasMaxLength(100);

                    b.Property<decimal>("ShippingCost")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("ShippingCountry")
                        .HasMaxLength(100);

                    b.Property<string>("ShippingEmail")
                        .HasMaxLength(150);

                    b.Property<string>("ShippingPostalCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTelNo1")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTelNo2")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTelNo3")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTown")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.Property<bool>("isExternal");

                    b.HasKey("Id");

                    b.ToTable("ExOrder");
                });

            modelBuilder.Entity("PMS.Data.Entities.Order.ExOrderItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CargoCurrency");

                    b.Property<decimal?>("CargoPrice");

                    b.Property<string>("CatalogCode");

                    b.Property<decimal?>("Consumable");

                    b.Property<string>("ConsumableCurrency");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Currency")
                        .HasMaxLength(5);

                    b.Property<int>("Desi");

                    b.Property<bool>("EmailSent");

                    b.Property<bool>("IARSent");

                    b.Property<bool>("IsInChequeManage");

                    b.Property<long>("OrderId");

                    b.Property<int>("Points");

                    b.Property<decimal>("PointsValue")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("ProductName")
                        .HasMaxLength(200);

                    b.Property<int>("Quantity");

                    b.Property<string>("SKU")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<decimal>("VAT")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("ExOrderItems");
                });

            modelBuilder.Entity("PMS.Data.Entities.Order.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BillingAddress")
                        .HasColumnType("NVARCHAR(MAX)");

                    b.Property<string>("BillingAddressName")
                        .HasMaxLength(100);

                    b.Property<string>("BillingCity")
                        .HasMaxLength(100);

                    b.Property<string>("BillingCompany")
                        .HasMaxLength(200);

                    b.Property<string>("BillingCountry")
                        .HasMaxLength(100);

                    b.Property<string>("BillingEmail")
                        .HasMaxLength(150);

                    b.Property<string>("BillingIdentityNumber")
                        .HasMaxLength(15);

                    b.Property<string>("BillingPostalCode")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTaxNumber")
                        .HasMaxLength(15);

                    b.Property<string>("BillingTaxOffice")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTelNo1")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTelNo2")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTelNo3")
                        .HasMaxLength(50);

                    b.Property<string>("BillingTown")
                        .HasMaxLength(100);

                    b.Property<decimal>("CollectionViaCreditCard")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("CollectionViaTransfer")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("Explanation1")
                        .HasMaxLength(100);

                    b.Property<string>("Explanation2")
                        .HasMaxLength(100);

                    b.Property<string>("Explanation3")
                        .HasMaxLength(100);

                    b.Property<bool>("IsAccountCreated");

                    b.Property<bool>("IsAccountShippingCreated");

                    b.Property<bool>("IsTransferred");

                    b.Property<DateTime?>("LastTryDate");

                    b.Property<int>("LogoCompanyCode");

                    b.Property<int>("LogoTransferedId");

                    b.Property<DateTime?>("OrderDate");

                    b.Property<string>("OrderNo")
                        .HasMaxLength(50);

                    b.Property<string>("ProjectCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShipmentCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingAddress")
                        .HasColumnType("NVARCHAR(MAX)");

                    b.Property<string>("ShippingAddressName")
                        .HasMaxLength(100);

                    b.Property<string>("ShippingCity")
                        .HasMaxLength(100);

                    b.Property<decimal>("ShippingCost")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("ShippingCountry")
                        .HasMaxLength(100);

                    b.Property<string>("ShippingEmail")
                        .HasMaxLength(150);

                    b.Property<string>("ShippingPostalCode")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTelNo1")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTelNo2")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTelNo3")
                        .HasMaxLength(50);

                    b.Property<string>("ShippingTown")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PMS.Data.Entities.Order.OrderItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CargoCurrency");

                    b.Property<decimal?>("CargoPrice");

                    b.Property<string>("CatalogCode");

                    b.Property<decimal?>("Consumable");

                    b.Property<string>("ConsumableCurrency");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Currency")
                        .HasMaxLength(5);

                    b.Property<int>("Desi");

                    b.Property<bool>("EmailSent");

                    b.Property<bool>("IARSent");

                    b.Property<bool>("IsInChequeManage");

                    b.Property<long>("OrderId");

                    b.Property<int>("Points");

                    b.Property<decimal>("PointsValue")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("ProductName")
                        .HasMaxLength(200);

                    b.Property<int>("Quantity");

                    b.Property<string>("SKU")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<decimal>("VAT")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("PMS.Data.Entities.Project.Project", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CheckingAccount")
                        .HasMaxLength(200);

                    b.Property<string>("CheckingAccountCode")
                        .HasMaxLength(50);

                    b.Property<bool?>("CheckingAccountToBeCreated");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("DefaultCurrency");

                    b.Property<string>("Departmant")
                        .HasMaxLength(50);

                    b.Property<string>("Division")
                        .HasMaxLength(50);

                    b.Property<int>("DueDateDifference");

                    b.Property<string>("Factory")
                        .HasMaxLength(50);

                    b.Property<string>("LogoFirmCode")
                        .HasMaxLength(20);

                    b.Property<bool>("PointIsOrderDiscount");

                    b.Property<bool>("PriceControl");

                    b.Property<bool>("PriceDifference");

                    b.Property<string>("ProjectCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ProjectGroupCode")
                        .HasMaxLength(50);

                    b.Property<string>("ProjectName")
                        .HasMaxLength(200);

                    b.Property<string>("ProjectPrefix")
                        .HasMaxLength(50);

                    b.Property<int>("ProjectType");

                    b.Property<string>("Source_Cost_GRP");

                    b.Property<bool>("TaxIncluded");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<string>("Warehouse")
                        .HasMaxLength(100);

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("ProjectCode")
                        .IsUnique();

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("PMS.Data.Entities.Project.ProjectProduct", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CargoCurrency");

                    b.Property<decimal?>("CargoPrice");

                    b.Property<string>("CatalogCode");

                    b.Property<string>("CatalogDescription");

                    b.Property<decimal?>("Consumable");

                    b.Property<string>("ConsumableCurrency");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("GroupCode");

                    b.Property<string>("LogoCode");

                    b.Property<decimal?>("Price");

                    b.Property<string>("PriceCurrency");

                    b.Property<int?>("ProductWeight");

                    b.Property<int>("ProjectId");

                    b.Property<long?>("ProjectId1");

                    b.Property<int?>("TaxRate");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.Property<bool>("isActive");

                    b.Property<bool>("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId1");

                    b.ToTable("ProjectProduct");
                });

            modelBuilder.Entity("PMS.Data.Entities.Invoice.InvoiceItem", b =>
                {
                    b.HasOne("PMS.Data.Entities.Invoice.Invoice", "Invoice")
                        .WithMany("InvoiceItems")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.Data.Entities.Order.ExOrderItem", b =>
                {
                    b.HasOne("PMS.Data.Entities.Order.ExOrder", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.Data.Entities.Order.OrderItem", b =>
                {
                    b.HasOne("PMS.Data.Entities.Order.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PMS.Data.Entities.Project.ProjectProduct", b =>
                {
                    b.HasOne("PMS.Data.Entities.Project.Project", "Project")
                        .WithMany("ProjectProducts")
                        .HasForeignKey("ProjectId1");
                });
#pragma warning restore 612, 618
        }
    }
}
