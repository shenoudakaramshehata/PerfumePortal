using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class SessionShoppingCartId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itemPriceNs_Country_CountryId",
                table: "itemPriceNs");

            migrationBuilder.AddColumn<string>(
                name: "SessionShoppingCartId",
                table: "ShoppingCart",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "itemPriceNs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_itemPriceNs_Country_CountryId",
                table: "itemPriceNs",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itemPriceNs_Country_CountryId",
                table: "itemPriceNs");

            migrationBuilder.DropColumn(
                name: "SessionShoppingCartId",
                table: "ShoppingCart");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "itemPriceNs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_itemPriceNs_Country_CountryId",
                table: "itemPriceNs",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId");
        }
    }
}
