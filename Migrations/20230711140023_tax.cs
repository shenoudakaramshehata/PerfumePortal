using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class tax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "tax",
                table: "Country",
                type: "float",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "tax",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "tax",
                value: 15.0);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 3,
                column: "tax",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 4,
                column: "tax",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 5,
                column: "tax",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 6,
                column: "tax",
                value: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tax",
                table: "Country");
        }
    }
}
