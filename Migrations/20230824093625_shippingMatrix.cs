using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class shippingMatrix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingsMatrix",
                columns: table => new
                {
                    ShippingMatrixId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    FromWeight = table.Column<double>(type: "float", nullable: false),
                    ToWeight = table.Column<double>(type: "float", nullable: false),
                    AramexPrice = table.Column<double>(type: "float", nullable: false),
                    ActualPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingsMatrix", x => x.ShippingMatrixId);
                    table.ForeignKey(
                        name: "FK_ShippingsMatrix_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingsMatrix_CountryId",
                table: "ShippingsMatrix",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingsMatrix");
        }
    }
}
