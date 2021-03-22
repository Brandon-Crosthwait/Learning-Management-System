using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Migrations
{
    public partial class UserPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "payment",
                table: "User",
                newName: "Payment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payment",
                table: "User",
                newName: "payment");
        }
    }
}
