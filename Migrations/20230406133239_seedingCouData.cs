using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class seedingCouData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "CountryTlar",
                value: "الإمارات");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "CountryTlar",
                value: "السعودية");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 3,
                column: "CountryTlar",
                value: "قطر");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 4,
                column: "CountryTlar",
                value: "الكويت");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 5,
                column: "CountryTlar",
                value: "عمان");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 6,
                column: "CountryTlar",
                value: "بحرين");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "CountryTlar",
                value: null);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "CountryTlar",
                value: null);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 3,
                column: "CountryTlar",
                value: null);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 4,
                column: "CountryTlar",
                value: null);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 5,
                column: "CountryTlar",
                value: null);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 6,
                column: "CountryTlar",
                value: null);
        }
    }
}
