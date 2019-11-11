using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHome.Data.Migrations
{
    public partial class RemovedModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Module_ModuleId",
                table: "Component");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "Component",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Component_ModuleId",
                table: "Component",
                newName: "IX_Component_RoomId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Component",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Room_RoomId",
                table: "Component",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Room_RoomId",
                table: "Component");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Component");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Component",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Component_RoomId",
                table: "Component",
                newName: "IX_Component_ModuleId");

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    ModuleId = table.Column<Guid>(nullable: false),
                    IsConnected = table.Column<bool>(nullable: false),
                    RoomId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_Module_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Module_RoomId",
                table: "Module",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Module_ModuleId",
                table: "Component",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
