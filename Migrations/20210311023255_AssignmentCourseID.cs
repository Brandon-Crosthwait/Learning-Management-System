using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Migrations
{
    public partial class AssignmentCourseID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSubmission",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "TextSubmission",
                table: "Assignment");

            migrationBuilder.AddColumn<int>(
                name: "CourseID",
                table: "Assignment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "Assignment");

            migrationBuilder.AddColumn<string>(
                name: "FileSubmission",
                table: "Assignment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextSubmission",
                table: "Assignment",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
