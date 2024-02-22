using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { "1dc24c59-00b1-4473-a58c-f4b4daf2312e", "23271c15-55f2-4511-bf92-51588898672c", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "88debf5c-5fed-4829-bace-ee936b7ea5ee", "11641a68-faf2-4f62-9768-c9e92ed6a808", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dc24c59-00b1-4473-a58c-f4b4daf2312e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88debf5c-5fed-4829-bace-ee936b7ea5ee");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e1b0d8da-8c1f-4d27-a070-2cd92010eea0", "c7e7e9f4-a52d-496b-9dcc-9d9787b45019", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f01c30df-0c03-4ddc-85e9-7e43962cd8c2", "7e8fe60f-9e76-4e45-b99e-e10d4e1f6b4e", "Admin", "ADMIN" });
        }
    }
}
