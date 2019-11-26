using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHome.Data.Migrations
{
    public partial class SmartHomeEntityName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SmartHomeEntity",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SmartHomeEntity");
        }
    }
}
