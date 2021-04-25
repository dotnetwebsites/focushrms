using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class MailData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "MailLibraries",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailName",
                table: "MailLibraries",
                type: "nvarchar(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "MailLibraries");

            migrationBuilder.DropColumn(
                name: "EmailName",
                table: "MailLibraries");
        }
    }
}
