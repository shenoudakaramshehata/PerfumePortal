using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    public partial class roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "015966ed-c4e5-4753-8de6-8af489ae3f6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0a6d67bd-1d9f-4684-a070-f51dd4176345");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26bcd044-3af9-4ea8-84d2-0fb674b62cf0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c439497f-940b-40b2-a740-371c4632144c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1aac80d2-2484-457c-9c62-46f0c60f011b", "b5dfe8b6-9790-4c58-80f3-4cec49d5be33", "Operator", "OPERATOR" },
                    { "8ed306c6-f699-46b0-9ee7-013c8de56694", "23ec0b64-be7d-4f23-94ee-68d5407ee149", "Store", "STORE" },
                    { "97193a65-508e-4034-baf9-009acd5828f5", "960a954f-0e73-4104-99fc-05e5d4fa6e8a", "Customer", "CUSTOMER" },
                    { "db0c5da0-95b3-41f2-848f-f96a5909b834", "84e19c63-46be-4c04-885b-98779a855b94", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1aac80d2-2484-457c-9c62-46f0c60f011b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ed306c6-f699-46b0-9ee7-013c8de56694");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97193a65-508e-4034-baf9-009acd5828f5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "db0c5da0-95b3-41f2-848f-f96a5909b834");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "015966ed-c4e5-4753-8de6-8af489ae3f6e", "f8ed56b3-18a6-427c-9b23-6f6895b11abf", "Admin", "ADMIN" },
                    { "0a6d67bd-1d9f-4684-a070-f51dd4176345", "7c7db335-5678-4f2a-8a62-9d69eeed571e", "Store", "STORE" },
                    { "26bcd044-3af9-4ea8-84d2-0fb674b62cf0", "c0c88e31-d3bb-4e59-b5b6-c6b5784e1b2c", "Customer", "CUSTOMER" },
                    { "c439497f-940b-40b2-a740-371c4632144c", "93fc1e46-c869-405c-954e-df7223311990", "Operator", "OPERATOR" }
                });
        }
    }
}
