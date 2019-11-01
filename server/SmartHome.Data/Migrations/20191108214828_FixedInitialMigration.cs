using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHome.Data.Migrations
{
    public partial class FixedInitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComponentType",
                columns: table => new
                {
                    ComponentTypeId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 30, nullable: false),
                    IsSwitchable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentType", x => x.ComponentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SmartHomeEntity",
                columns: table => new
                {
                    SmartHomeEntityId = table.Column<Guid>(nullable: false),
                    RegisterTimestamp = table.Column<DateTime>(nullable: false),
                    IpAddress = table.Column<string>(fixedLength: true, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartHomeEntity", x => x.SmartHomeEntityId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(maxLength: 20, nullable: false),
                    Password = table.Column<string>(fixedLength: true, maxLength: 64, nullable: false),
                    Email = table.Column<string>(maxLength: 40, nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    SmartHomeEntityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Room_SmartHomeEntity_SmartHomeEntityId",
                        column: x => x.SmartHomeEntityId,
                        principalTable: "SmartHomeEntity",
                        principalColumn: "SmartHomeEntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSmartHomeEntity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    SmartHomeEntityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSmartHomeEntity", x => new { x.UserId, x.SmartHomeEntityId });
                    table.ForeignKey(
                        name: "FK_UserSmartHomeEntity_SmartHomeEntity_SmartHomeEntityId",
                        column: x => x.SmartHomeEntityId,
                        principalTable: "SmartHomeEntity",
                        principalColumn: "SmartHomeEntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSmartHomeEntity_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Component",
                columns: table => new
                {
                    ComponentId = table.Column<Guid>(nullable: false),
                    ComponentState = table.Column<string>(maxLength: 10, nullable: false),
                    ComponentTypeId = table.Column<Guid>(nullable: false),
                    ModuleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Component", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_Component_ComponentType_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentType",
                        principalColumn: "ComponentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Component_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentData",
                columns: table => new
                {
                    ComponentDataId = table.Column<Guid>(nullable: false),
                    Reading = table.Column<decimal>(type: "decimal(6, 2)", nullable: false),
                    Message = table.Column<string>(maxLength: 30, nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    ComponentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentData", x => x.ComponentDataId);
                    table.ForeignKey(
                        name: "FK_ComponentData_Component_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Component",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Component_ComponentTypeId",
                table: "Component",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_ModuleId",
                table: "Component",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentData_ComponentId",
                table: "ComponentData",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_RoomId",
                table: "Module",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_SmartHomeEntityId",
                table: "Room",
                column: "SmartHomeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSmartHomeEntity_SmartHomeEntityId",
                table: "UserSmartHomeEntity",
                column: "SmartHomeEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentData");

            migrationBuilder.DropTable(
                name: "UserSmartHomeEntity");

            migrationBuilder.DropTable(
                name: "Component");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "ComponentType");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "SmartHomeEntity");
        }
    }
}
