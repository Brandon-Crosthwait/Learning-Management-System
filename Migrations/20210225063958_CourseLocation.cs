using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Migrations
{
    public partial class CourseLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "Building",
                table: "Course",
                newName: "Location");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Course",
                newName: "Building");

            migrationBuilder.AddColumn<int>(
                name: "Room",
                table: "Course",
                type: "int",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);
        }
    }
}
