using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class updatecountrySeddingDa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "CountryTLEN",
                value: "Emirate");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "CountryTLEN",
                value: "Saudi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "CountryTLEN",
                value: "United Arab Emirates");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "CountryTLEN",
                value: "Saudi Arabia");
        }
    }
}
