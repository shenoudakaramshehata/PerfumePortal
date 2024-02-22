using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    public partial class newrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[,]
                {
                    { "015966ed-c4e5-4753-8de6-8af489ae3f6e", "f8ed56b3-18a6-427c-9b23-6f6895b11abf", "Admin", "ADMIN" },
                    { "0a6d67bd-1d9f-4684-a070-f51dd4176345", "7c7db335-5678-4f2a-8a62-9d69eeed571e", "Store", "STORE" },
                    { "26bcd044-3af9-4ea8-84d2-0fb674b62cf0", "c0c88e31-d3bb-4e59-b5b6-c6b5784e1b2c", "Customer", "CUSTOMER" },
                    { "c439497f-940b-40b2-a740-371c4632144c", "93fc1e46-c869-405c-954e-df7223311990", "Operator", "OPERATOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "1dc24c59-00b1-4473-a58c-f4b4daf2312e", "23271c15-55f2-4511-bf92-51588898672c", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "88debf5c-5fed-4829-bace-ee936b7ea5ee", "11641a68-faf2-4f62-9768-c9e92ed6a808", "Admin", "ADMIN" });
        }
    }
}
