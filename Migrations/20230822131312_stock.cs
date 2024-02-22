using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class stock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BeforePrice",
                table: "itemPriceNs",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Item",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeforePrice",
                table: "itemPriceNs");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Item");
        }
    }
}
