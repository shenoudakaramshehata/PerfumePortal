using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    public partial class Whatsapp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhatSapp",
                table: "SocialMediaLinks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SocialMediaLinks",
                keyColumn: "SocialMediaLinkId",
                keyValue: 1,
                column: "WhatSapp",
                value: "0096598050646");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhatSapp",
                table: "SocialMediaLinks");
        }
    }
}
