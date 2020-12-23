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
    [Migration("20191001115352_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PMS.Data.Entities.Order", b =>
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

                    b.Property<bool?>("IsTransferred");

                    b.Property<DateTime?>("LastTryDate");

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

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PMS.Data.Entities.OrderItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Currency")
                        .HasMaxLength(5);

                    b.Property<int>("Desi");

                    b.Property<bool?>("EmailSent");

                    b.Property<bool?>("IARSent");

                    b.Property<bool?>("IsInChequeManage");

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

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("PMS.Data.Entities.Project", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CheckingAccount")
                        .HasMaxLength(200);

                    b.Property<string>("CheckingAccountCode")
                        .HasMaxLength(50);

                    b.Property<bool?>("CheckingAccountToBeOpened");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("LogoFirmCode")
                        .HasMaxLength(20);

                    b.Property<string>("ProjectCode")
                        .HasMaxLength(50);

                    b.Property<string>("ProjectName")
                        .HasMaxLength(200);

                    b.Property<string>("ProjectPrefix")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<int?>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("PMS.Data.Entities.OrderItem", b =>
                {
                    b.HasOne("PMS.Data.Entities.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
