using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class Curenncy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 1,
                column: "CurrencyTLEN",
                value: "KWD");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 3,
                column: "CurrencyTLEN",
                value: "BHD");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 5,
                column: "CurrencyTLEN",
                value: "SAR");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 1,
                column: "CurrencyTLEN",
                value: "KD");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 3,
                column: "CurrencyTLEN",
                value: "BD");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 5,
                column: "CurrencyTLEN",
                value: "SR");
        }
    }
}
