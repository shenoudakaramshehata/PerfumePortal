using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class deletecountryTl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryTLAR",
                table: "Country");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryTLAR",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "CountryTLAR",
                value: "الإمارات العربية المتحدة ");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "CountryTLAR",
                value: "المملكة العربية السعودية");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 3,
                column: "CountryTLAR",
                value: "دولة قطر ");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 4,
                column: "CountryTLAR",
                value: "الكويت ");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 5,
                column: "CountryTLAR",
                value: "سلطنة عمان");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 6,
                column: "CountryTLAR",
                value: "البحرين");
        }
    }
}
