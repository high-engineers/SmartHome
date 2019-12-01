using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHome.Data.Migrations
{
    public partial class GlobalFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "SmartHomeEntity");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "ComponentData");

            migrationBuilder.AddColumn<Guid>(
                name: "SmartHomeEntityId",
                table: "Component",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Component_SmartHomeEntityId",
                table: "Component",
                column: "SmartHomeEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_SmartHomeEntity_SmartHomeEntityId",
                table: "Component",
                column: "SmartHomeEntityId",
                principalTable: "SmartHomeEntity",
                principalColumn: "SmartHomeEntityId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_SmartHomeEntity_SmartHomeEntityId",
                table: "Component");

            migrationBuilder.DropIndex(
                name: "IX_Component_SmartHomeEntityId",
                table: "Component");

            migrationBuilder.DropColumn(
                name: "SmartHomeEntityId",
                table: "Component");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "SmartHomeEntity",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ComponentData",
                maxLength: 30,
                nullable: true);
        }
    }
}
