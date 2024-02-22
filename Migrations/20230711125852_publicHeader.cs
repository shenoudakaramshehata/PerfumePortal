using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class publicHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicHeader",
                columns: table => new
                {
                    PublicHeaderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicHeader", x => x.PublicHeaderId);
                });

            migrationBuilder.InsertData(
                table: "PublicHeader",
                columns: new[] { "PublicHeaderId", "pic" },
                values: new object[] { 1, "Images/PublicHeader/BannerOrg.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicHeader");
        }
    }
}
