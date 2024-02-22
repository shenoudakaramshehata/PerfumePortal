﻿// <auto-generated />
using System;
using CRM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CRM.Migrations
{
    [DbContext(typeof(PerfumeContext))]
    [Migration("20230110085102_status")]
    partial class status
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CRM.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryPic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryTlar")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CategoryTLAR")
                        .HasDefaultValueSql("(N'')");

                    b.Property<string>("CategoryTlen")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CategoryTLEN")
                        .HasDefaultValueSql("(N'')");

                    b.Property<string>("DescriptionTlar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("DescriptionTLAR");

                    b.Property<string>("DescriptionTlen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("DescriptionTLEN");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<int?>("SortOrder")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CRM.Models.ContactUs", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ContactId"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Msg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("TransDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ContactId");

                    b.ToTable("ContactUs");
                });

            modelBuilder.Entity("CRM.Models.Country", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CountryId"), 1L, 1);

                    b.Property<string>("CountryTlar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CountryTLAR");

                    b.Property<string>("CountryTlen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CountryTLEN");

                    b.Property<int?>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("OrderIndex")
                        .HasColumnType("int");

                    b.Property<string>("Pic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("ShippingCost")
                        .HasColumnType("float");

                    b.HasKey("CountryId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("CRM.Models.Coupon", b =>
                {
                    b.Property<int>("CouponId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CouponId"), 1L, 1);

                    b.Property<double?>("Amount")
                        .HasColumnType("float");

                    b.Property<int>("CouponTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("IssueDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Serial")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Used")
                        .HasColumnType("bit");

                    b.HasKey("CouponId");

                    b.HasIndex("CouponTypeId");

                    b.ToTable("Coupon");
                });

            modelBuilder.Entity("CRM.Models.CouponType", b =>
                {
                    b.Property<int>("CouponTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CouponTypeId"), 1L, 1);

                    b.Property<string>("CouponTypeAR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CouponTypeEN")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CouponTypeId");

                    b.ToTable("CouponTypes");
                });

            modelBuilder.Entity("CRM.Models.Currency", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CurrencyId"), 1L, 1);

                    b.Property<string>("CurrencyPic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrencyTlar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CurrencyTLAR");

                    b.Property<string>("CurrencyTlen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CurrencyTLEN");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.HasKey("CurrencyId");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("CRM.Models.CustomerAddress", b =>
                {
                    b.Property<int>("CustomerAddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerAddressId"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AreaName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("BuildingNo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomerAddressId");

                    b.HasIndex("CountryId");

                    b.HasIndex("CustomerId");

                    b.ToTable("customerAddresses");
                });

            modelBuilder.Entity("CRM.Models.CustomerN", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"), 1L, 1);

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegisterDate")
                        .HasColumnType("datetime");

                    b.HasKey("CustomerId");

                    b.ToTable("CustomerNs");
                });

            modelBuilder.Entity("CRM.Models.FavouriteItem", b =>
                {
                    b.Property<int>("FavouriteItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FavouriteItemId"), 1L, 1);

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FavouriteItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("FavouriteItems");
                });

            modelBuilder.Entity("CRM.Models.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemId"), 1L, 1);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("ItemDescriptionAr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemDescriptionEn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemTitleAr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemTitleEn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderIndex")
                        .HasColumnType("int");

                    b.Property<bool>("OutOfStock")
                        .HasColumnType("bit");

                    b.Property<double?>("Weight")
                        .HasColumnType("float");

                    b.HasKey("ItemId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("CRM.Models.ItemImage", b =>
                {
                    b.Property<int>("ItemImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemImageId"), 1L, 1);

                    b.Property<string>("ImageName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.HasKey("ItemImageId");

                    b.HasIndex("ItemId");

                    b.ToTable("ItemImage");
                });

            modelBuilder.Entity("CRM.Models.ItemPriceN", b =>
                {
                    b.Property<int>("ItemPriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemPriceId"), 1L, 1);

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<double?>("Price")
                        .IsRequired()
                        .HasColumnType("float");

                    b.HasKey("ItemPriceId");

                    b.HasIndex("CountryId");

                    b.HasIndex("ItemId");

                    b.ToTable("itemPriceNs");
                });

            modelBuilder.Entity("CRM.Models.Newsletter", b =>
                {
                    b.Property<int>("NewsletterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NewsletterId"), 1L, 1);

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NewsletterId");

                    b.ToTable("Newsletters");
                });

            modelBuilder.Entity("CRM.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"), 1L, 1);

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<double?>("CouponAmount")
                        .HasColumnType("float");

                    b.Property<int?>("CouponId")
                        .HasColumnType("int");

                    b.Property<int?>("CouponTypeId")
                        .HasColumnType("int");

                    b.Property<int?>("CustomerAddressId")
                        .HasColumnType("int");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<int?>("CustomerNCustomerId")
                        .HasColumnType("int");

                    b.Property<double?>("Deliverycost")
                        .HasColumnType("float");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeliverd")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<double>("OrderDiscount")
                        .HasColumnType("float");

                    b.Property<double?>("OrderNet")
                        .HasColumnType("float");

                    b.Property<string>("OrderSerial")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderStatusId")
                        .HasColumnType("int");

                    b.Property<double>("OrderTotal")
                        .HasColumnType("float");

                    b.Property<string>("PaymentId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PaymentID");

                    b.Property<int?>("PaymentMethodId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PostDate")
                        .HasColumnType("datetime");

                    b.Property<string>("ShippingLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShippingNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UniqeId")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("CountryId");

                    b.HasIndex("CouponId");

                    b.HasIndex("CouponTypeId");

                    b.HasIndex("CustomerAddressId");

                    b.HasIndex("CustomerNCustomerId");

                    b.HasIndex("OrderStatusId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("CRM.Models.OrderItem", b =>
                {
                    b.Property<int>("OrderItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderItemId"), 1L, 1);

                    b.Property<int?>("ItemId")
                        .HasColumnType("int");

                    b.Property<double>("ItemPrice")
                        .HasColumnType("float");

                    b.Property<int>("ItemQuantity")
                        .HasColumnType("int");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<double>("Total")
                        .HasColumnType("float");

                    b.HasKey("OrderItemId");

                    b.HasIndex("ItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("CRM.Models.OrderStatus", b =>
                {
                    b.Property<int>("OrderStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderStatusId"), 1L, 1);

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderStatusId");

                    b.ToTable("OrderStatuses");

                    b.HasData(
                        new
                        {
                            OrderStatusId = 1,
                            Status = "Initiated"
                        },
                        new
                        {
                            OrderStatusId = 2,
                            Status = "Paid"
                        },
                        new
                        {
                            OrderStatusId = 3,
                            Status = "Paid Failed"
                        },
                        new
                        {
                            OrderStatusId = 4,
                            Status = "Canceled"
                        },
                        new
                        {
                            OrderStatusId = 5,
                            Status = "Delivered"
                        },
                        new
                        {
                            OrderStatusId = 6,
                            Status = "Processing"
                        },
                        new
                        {
                            OrderStatusId = 7,
                            Status = "Packing"
                        });
                });

            modelBuilder.Entity("CRM.Models.OrderTraking", b =>
                {
                    b.Property<int>("OrderTrakingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderTrakingId"), 1L, 1);

                    b.Property<DateTime>("ActionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("OrderStatusId")
                        .HasColumnType("int");

                    b.Property<string>("Remarks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderTrakingId");

                    b.HasIndex("OrderId");

                    b.HasIndex("OrderStatusId");

                    b.ToTable("OrderTrakings");
                });

            modelBuilder.Entity("CRM.Models.PageContent", b =>
                {
                    b.Property<int>("PageContentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PageContentId"), 1L, 1);

                    b.Property<string>("ContentAr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentEn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PageTitleAr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PageTitleEn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PageContentId");

                    b.ToTable("PageContents");

                    b.HasData(
                        new
                        {
                            PageContentId = 1,
                            ContentAr = "من نحن",
                            ContentEn = "About Page",
                            PageTitleAr = "من نحن",
                            PageTitleEn = "About"
                        },
                        new
                        {
                            PageContentId = 2,
                            ContentAr = "الشروط والاحكام",
                            ContentEn = "Condition and Terms Page",
                            PageTitleAr = "الشروط والاحكام",
                            PageTitleEn = "Condition and Terms"
                        },
                        new
                        {
                            PageContentId = 3,
                            ContentAr = "سياسة الخصوصية",
                            ContentEn = "Privacy Policy Page",
                            PageTitleAr = "سياسة الخصوصية",
                            PageTitleEn = "Privacy Policy"
                        });
                });

            modelBuilder.Entity("CRM.Models.PaymentMehod", b =>
                {
                    b.Property<int>("PaymentMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentMethodId"), 1L, 1);

                    b.Property<string>("PaymentMethodAR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethodEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethodPic")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentMethodId");

                    b.ToTable("paymentMehods");

                    b.HasData(
                        new
                        {
                            PaymentMethodId = 1,
                            PaymentMethodAR = "MyFattorah",
                            PaymentMethodEN = "MyFattorah",
                            PaymentMethodPic = ""
                        });
                });

            modelBuilder.Entity("CRM.Models.ShoppingCart", b =>
                {
                    b.Property<int>("ShoppingCartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ShoppingCartId"), 1L, 1);

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<double?>("Deliverycost")
                        .HasColumnType("float");

                    b.Property<int?>("ItemId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<double>("ItemPrice")
                        .HasColumnType("float");

                    b.Property<int>("ItemQty")
                        .HasColumnType("int");

                    b.Property<double>("ItemTotal")
                        .HasColumnType("float");

                    b.HasKey("ShoppingCartId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ItemId");

                    b.ToTable("ShoppingCart");
                });

            modelBuilder.Entity("CRM.Models.SocialMediaLink", b =>
                {
                    b.Property<int>("SocialMediaLinkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SocialMediaLinkId"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactMail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactPhone1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactPhone2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Facebook")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fax")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Instgram")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LinkedIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Twitter")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Youtube")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SocialMediaLinkId");

                    b.ToTable("SocialMediaLinks");

                    b.HasData(
                        new
                        {
                            SocialMediaLinkId = 1,
                            Address = "Kwait",
                            ContactMail = "",
                            ContactPhone1 = "",
                            ContactPhone2 = "",
                            Facebook = "Facebook@Example.com",
                            Fax = "9621",
                            Instgram = "Instgram@Example.com",
                            LinkedIn = "LinkedIn@Example.com",
                            Twitter = "Twitter@Example.com",
                            Youtube = ""
                        });
                });

            modelBuilder.Entity("CRM.Models.Country", b =>
                {
                    b.HasOne("CRM.Models.Currency", "Currency")
                        .WithMany("Country")
                        .HasForeignKey("CurrencyId")
                        .HasConstraintName("FK_Country_Currency");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("CRM.Models.Coupon", b =>
                {
                    b.HasOne("CRM.Models.CouponType", "CouponType")
                        .WithMany("Coupons")
                        .HasForeignKey("CouponTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CouponType");
                });

            modelBuilder.Entity("CRM.Models.CustomerAddress", b =>
                {
                    b.HasOne("CRM.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("CRM.Models.CustomerN", "CustomerN")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.Navigation("Country");

                    b.Navigation("CustomerN");
                });

            modelBuilder.Entity("CRM.Models.FavouriteItem", b =>
                {
                    b.HasOne("CRM.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("CRM.Models.Item", b =>
                {
                    b.HasOne("CRM.Models.Category", "Category")
                        .WithMany("Item")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_Item_Categories");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("CRM.Models.ItemImage", b =>
                {
                    b.HasOne("CRM.Models.Item", "Item")
                        .WithMany("ItemImageNavigation")
                        .HasForeignKey("ItemId")
                        .IsRequired()
                        .HasConstraintName("FK_ItemImage_Item");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("CRM.Models.ItemPriceN", b =>
                {
                    b.HasOne("CRM.Models.Country", "Country")
                        .WithMany("itemPriceNs")
                        .HasForeignKey("CountryId");

                    b.HasOne("CRM.Models.Item", "Item")
                        .WithMany("ItemPriceNs")
                        .HasForeignKey("ItemId")
                        .IsRequired()
                        .HasConstraintName("FK_ItemPriceN_Item");

                    b.Navigation("Country");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("CRM.Models.Order", b =>
                {
                    b.HasOne("CRM.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("CRM.Models.Coupon", "Coupon")
                        .WithMany()
                        .HasForeignKey("CouponId");

                    b.HasOne("CRM.Models.CouponType", "CouponType")
                        .WithMany()
                        .HasForeignKey("CouponTypeId");

                    b.HasOne("CRM.Models.CustomerAddress", "CustomerAddress")
                        .WithMany()
                        .HasForeignKey("CustomerAddressId");

                    b.HasOne("CRM.Models.CustomerN", "CustomerN")
                        .WithMany("Order")
                        .HasForeignKey("CustomerNCustomerId");

                    b.HasOne("CRM.Models.OrderStatus", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId");

                    b.Navigation("Country");

                    b.Navigation("Coupon");

                    b.Navigation("CouponType");

                    b.Navigation("CustomerAddress");

                    b.Navigation("CustomerN");

                    b.Navigation("OrderStatus");
                });

            modelBuilder.Entity("CRM.Models.OrderItem", b =>
                {
                    b.HasOne("CRM.Models.Item", "Item")
                        .WithMany("OrderItem")
                        .HasForeignKey("ItemId");

                    b.HasOne("CRM.Models.Order", "Order")
                        .WithMany("OrderItem")
                        .HasForeignKey("OrderId");

                    b.Navigation("Item");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CRM.Models.OrderTraking", b =>
                {
                    b.HasOne("CRM.Models.Order", "Order")
                        .WithMany("OrderTrakings")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CRM.Models.OrderStatus", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("OrderStatus");
                });

            modelBuilder.Entity("CRM.Models.ShoppingCart", b =>
                {
                    b.HasOne("CRM.Models.CustomerN", "CustomerN")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.HasOne("CRM.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CustomerN");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("CRM.Models.Category", b =>
                {
                    b.Navigation("Item");
                });

            modelBuilder.Entity("CRM.Models.Country", b =>
                {
                    b.Navigation("itemPriceNs");
                });

            modelBuilder.Entity("CRM.Models.CouponType", b =>
                {
                    b.Navigation("Coupons");
                });

            modelBuilder.Entity("CRM.Models.Currency", b =>
                {
                    b.Navigation("Country");
                });

            modelBuilder.Entity("CRM.Models.CustomerN", b =>
                {
                    b.Navigation("Order");
                });

            modelBuilder.Entity("CRM.Models.Item", b =>
                {
                    b.Navigation("ItemImageNavigation");

                    b.Navigation("ItemPriceNs");

                    b.Navigation("OrderItem");
                });

            modelBuilder.Entity("CRM.Models.Order", b =>
                {
                    b.Navigation("OrderItem");

                    b.Navigation("OrderTrakings");
                });
#pragma warning restore 612, 618
        }
    }
}
