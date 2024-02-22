using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class shippingprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ShippingPrice",
                table: "itemPriceNs",
                type: "float",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "OrderStatusId",
                keyValue: 5,
                column: "Status",
                value: "OnDelivery");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "itemPriceNs");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "OrderStatusId",
                keyValue: 5,
                column: "Status",
                value: "Delivered");
        }
    }
}
