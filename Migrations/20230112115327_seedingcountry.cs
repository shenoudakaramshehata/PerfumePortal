using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class seedingcountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "CurrencyId", "CurrencyPic", "CurrencyTLAR", "CurrencyTLEN", "IsActive" },
                values: new object[,]
                {
                    { 1, "Images/Currency/065e4dcb-8da6-409c-bab2-c93c13eee243_denar.jpg", "د.ك.", "KD", true },
                    { 2, "Images/Currency/OMR.jpg", "ر.ع.", "OMR", true },
                    { 3, "Images/Currency/BH.jpg", "د.ب", "BD", true },
                    { 4, "Images/Currency/banknote.jpg", "د.إ.", "AED", true },
                    { 5, "Images/Currency/download.jpg", "ر.س.", "SR", true },
                    { 6, "Images/Currency/SA.jpg", "ر.ق.", "QAR", true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "CurrencyId",
                keyValue: 6);
        }
    }
}
