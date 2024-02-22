using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class WholeContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(CONVERT([bit],(0)))"),
                    DescriptionTLEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryTLAR = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "(N'')"),
                    CategoryTLEN = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "(N'')"),
                    DescriptionTLAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Msg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "CouponTypes",
                columns: table => new
                {
                    CouponTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponTypeAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponTypeEN = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypes", x => x.CouponTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyTLAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyTLEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNs",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNs", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Newsletters",
                columns: table => new
                {
                    NewsletterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newsletters", x => x.NewsletterId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    PageContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.PageContentId);
                });

            migrationBuilder.CreateTable(
                name: "paymentMehods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethodEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethodPic = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentMehods", x => x.PaymentMethodId);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaLinks",
                columns: table => new
                {
                    SocialMediaLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Facebook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkedIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instgram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Twitter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Youtube = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaLinks", x => x.SocialMediaLinkId);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: true),
                    OutOfStock = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Item_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Serial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    CouponTypeId = table.Column<int>(type: "int", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.CouponId);
                    table.ForeignKey(
                        name: "FK_Coupon_CouponTypes_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponTypes",
                        principalColumn: "CouponTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryTLAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryTLEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    ShippingCost = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_Country_Currency",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "CurrencyId");
                });

            migrationBuilder.CreateTable(
                name: "FavouriteItems",
                columns: table => new
                {
                    FavouriteItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteItems", x => x.FavouriteItemId);
                    table.ForeignKey(
                        name: "FK_FavouriteItems_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemImage",
                columns: table => new
                {
                    ItemImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemImage", x => x.ItemImageId);
                    table.ForeignKey(
                        name: "FK_ItemImage_Item",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPrice = table.Column<double>(type: "float", nullable: false),
                    ItemQty = table.Column<int>(type: "int", nullable: false),
                    ItemTotal = table.Column<double>(type: "float", nullable: false),
                    Deliverycost = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.ShoppingCartId);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_CustomerNs_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerNs",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customerAddresses",
                columns: table => new
                {
                    CustomerAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerAddresses", x => x.CustomerAddressId);
                    table.ForeignKey(
                        name: "FK_customerAddresses_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_customerAddresses_CustomerNs_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerNs",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "itemPriceNs",
                columns: table => new
                {
                    ItemPriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemPriceNs", x => x.ItemPriceId);
                    table.ForeignKey(
                        name: "FK_ItemPriceN_Item",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_itemPriceNs_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    OrderSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    OrderStatusId = table.Column<int>(type: "int", nullable: true),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false),
                    IsDeliverd = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    CustomerAddressId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    OrderTotal = table.Column<double>(type: "float", nullable: false),
                    OrderDiscount = table.Column<double>(type: "float", nullable: false),
                    CouponId = table.Column<int>(type: "int", nullable: true),
                    CouponTypeId = table.Column<int>(type: "int", nullable: true),
                    CouponAmount = table.Column<double>(type: "float", nullable: true),
                    Deliverycost = table.Column<double>(type: "float", nullable: true),
                    OrderNet = table.Column<double>(type: "float", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    UniqeId = table.Column<int>(type: "int", nullable: false),
                    PaymentID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CustomerNCustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_Order_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "CouponId");
                    table.ForeignKey(
                        name: "FK_Order_CouponTypes_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponTypes",
                        principalColumn: "CouponTypeId");
                    table.ForeignKey(
                        name: "FK_Order_customerAddresses_CustomerAddressId",
                        column: x => x.CustomerAddressId,
                        principalTable: "customerAddresses",
                        principalColumn: "CustomerAddressId");
                    table.ForeignKey(
                        name: "FK_Order_CustomerNs_CustomerNCustomerId",
                        column: x => x.CustomerNCustomerId,
                        principalTable: "CustomerNs",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_Order_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "OrderStatusId");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    ItemPrice = table.Column<double>(type: "float", nullable: false),
                    ItemQuantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "OrderTrakings",
                columns: table => new
                {
                    OrderTrakingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderStatusId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTrakings", x => x.OrderTrakingId);
                    table.ForeignKey(
                        name: "FK_OrderTrakings_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTrakings_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "OrderStatusId", "Status" },
                values: new object[,]
                {
                    { 1, "Initiated" },
                    { 2, "Paid" },
                    { 3, "Paid Failed" },
                    { 4, "Canceled" },
                    { 5, "Delivered" }
                });

            migrationBuilder.InsertData(
                table: "PageContents",
                columns: new[] { "PageContentId", "ContentAr", "ContentEn", "PageTitleAr", "PageTitleEn" },
                values: new object[,]
                {
                    { 1, "من نحن", "About Page", "من نحن", "About" },
                    { 2, "الشروط والاحكام", "Condition and Terms Page", "الشروط والاحكام", "Condition and Terms" },
                    { 3, "سياسة الخصوصية", "Privacy Policy Page", "سياسة الخصوصية", "Privacy Policy" }
                });

            migrationBuilder.InsertData(
                table: "SocialMediaLinks",
                columns: new[] { "SocialMediaLinkId", "Address", "ContactMail", "ContactPhone1", "ContactPhone2", "Facebook", "Fax", "Instgram", "LinkedIn", "Twitter", "Youtube" },
                values: new object[] { 1, "Kwait", "", "", "", "Facebook@Example.com", "9621", "Instgram@Example.com", "LinkedIn@Example.com", "Twitter@Example.com", "" });

            migrationBuilder.InsertData(
                table: "paymentMehods",
                columns: new[] { "PaymentMethodId", "PaymentMethodAR", "PaymentMethodEN", "PaymentMethodPic" },
                values: new object[] { 1, "MyFattorah", "MyFattorah", "" });

            migrationBuilder.CreateIndex(
                name: "IX_Country_CurrencyId",
                table: "Country",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CouponTypeId",
                table: "Coupon",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_customerAddresses_CountryId",
                table: "customerAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_customerAddresses_CustomerId",
                table: "customerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteItems_ItemId",
                table: "FavouriteItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryId",
                table: "Item",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemImage_ItemId",
                table: "ItemImage",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_itemPriceNs_CountryId",
                table: "itemPriceNs",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_itemPriceNs_ItemId",
                table: "itemPriceNs",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CountryId",
                table: "Order",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CouponId",
                table: "Order",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CouponTypeId",
                table: "Order",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerAddressId",
                table: "Order",
                column: "CustomerAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerNCustomerId",
                table: "Order",
                column: "CustomerNCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusId",
                table: "Order",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ItemId",
                table: "OrderItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTrakings_OrderId",
                table: "OrderTrakings",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTrakings_OrderStatusId",
                table: "OrderTrakings",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CustomerId",
                table: "ShoppingCart",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_ItemId",
                table: "ShoppingCart",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "FavouriteItems");

            migrationBuilder.DropTable(
                name: "ItemImage");

            migrationBuilder.DropTable(
                name: "itemPriceNs");

            migrationBuilder.DropTable(
                name: "Newsletters");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "OrderTrakings");

            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "paymentMehods");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "SocialMediaLinks");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "customerAddresses");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "CouponTypes");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "CustomerNs");

            migrationBuilder.DropTable(
                name: "Currency");
        }
    }
}
