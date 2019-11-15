using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHome.Data.Migrations
{
    public partial class RevertedComponentName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Component",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Component");
        }
    }
}
