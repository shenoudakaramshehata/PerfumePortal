using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class ordercaptures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ZIPCode",
                table: "customerAddresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrdersCaptures",
                columns: table => new
                {
                    OrdersCapturesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<double>(type: "float", nullable: false),
                    captureId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Createdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersCaptures", x => x.OrdersCapturesId);
                    table.ForeignKey(
                        name: "FK_OrdersCaptures_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCaptures_OrderId",
                table: "OrdersCaptures",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdersCaptures");

            migrationBuilder.DropColumn(
                name: "ZIPCode",
                table: "customerAddresses");
        }
    }
}
