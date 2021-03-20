using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Migrations
{
    public partial class SubmissionContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmissionFile",
                table: "Submission",
                newName: "Content");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Submission",
                newName: "SubmissionFile");
        }
    }
}
