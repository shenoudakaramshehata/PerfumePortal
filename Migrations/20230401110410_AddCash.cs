using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class AddCash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "paymentMehods",
                columns: new[] { "PaymentMethodId", "PaymentMethodAR", "PaymentMethodEN", "PaymentMethodPic" },
                values: new object[] { 2, "Cash", "Cash", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "paymentMehods",
                keyColumn: "PaymentMethodId",
                keyValue: 2);
        }
    }
}
