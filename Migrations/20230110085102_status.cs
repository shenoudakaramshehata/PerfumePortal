using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingLabel",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingNo",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "OrderStatusId", "Status" },
                values: new object[] { 6, "Processing" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "OrderStatusId", "Status" },
                values: new object[] { 7, "Packing" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "OrderStatusId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "OrderStatusId",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "ShippingLabel",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingNo",
                table: "Order");
        }
    }
}
