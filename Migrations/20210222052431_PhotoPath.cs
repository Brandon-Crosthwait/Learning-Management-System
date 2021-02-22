using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Migrations
{
    public partial class PhotoPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureId",
                table: "User",
                newName: "PhotoPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "User",
                newName: "ProfilePictureId");
        }
    }
}
