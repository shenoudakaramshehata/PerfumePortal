using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9263ff12-dee5-4fb1-bac0-16b6cfe85a95");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f9f7ae2b-2fad-4b96-b667-c8e3f6798852");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e1b0d8da-8c1f-4d27-a070-2cd92010eea0", "c7e7e9f4-a52d-496b-9dcc-9d9787b45019", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f01c30df-0c03-4ddc-85e9-7e43962cd8c2", "7e8fe60f-9e76-4e45-b99e-e10d4e1f6b4e", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b0d8da-8c1f-4d27-a070-2cd92010eea0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f01c30df-0c03-4ddc-85e9-7e43962cd8c2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9263ff12-dee5-4fb1-bac0-16b6cfe85a95", "790760ad-46ab-4e16-b72e-4515a21a4568", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f9f7ae2b-2fad-4b96-b667-c8e3f6798852", "4ec495c8-5a51-4bc1-94c4-2f1963b3ee34", "Customer", "CUSTOMER" });
        }
    }
}
