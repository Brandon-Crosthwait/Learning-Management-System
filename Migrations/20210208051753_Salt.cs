using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Migrations
{
    public partial class Salt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "User",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "User",
                newName: "password");
        }
    }
}
