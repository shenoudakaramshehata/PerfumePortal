using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class seedingCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "CountryId", "CountryTLAR", "CountryTLEN", "CurrencyId", "IsActive", "OrderIndex", "Pic", "ShippingCost" },
                values: new object[,]
                {
                    { 1, "الإمارات العربية المتحدة ", "United Arab Emirates", 4, true, 1, "Images/Country/download.png", 20.0 },
                    { 2, "المملكة العربية السعودية", "Saudi Arabia", 5, true, 1, "Images/Country/Saudi.png", 20.0 },
                    { 3, "دولة قطر ", "Qatar", 6, true, 1, "Images/Country/flag-of-qatar_1.png", 20.0 },
                    { 4, "الكويت ", "Kuwait", 1, true, 1, "Images/Country/Kwait.png", 20.0 },
                    { 5, "سلطنة عمان", "Oman", 2, true, 1, "Images/Country/png-transparent-flag-of-oman-flag-of-chile-flag-of-oman-flag-photography-logo.png", 20.0 },
                    { 6, "البحرين", "Bahrain", 3, true, 1, "Images/Country/Bahrain.png", 20.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "CountryId",
                keyValue: 6);
        }
    }
}
