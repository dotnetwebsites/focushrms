using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class IsDocUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAadhaarUpload",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDegreeUpload",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPanUpload",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAadhaarUpload",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDegreeUpload",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsPanUpload",
                table: "AspNetUsers");
        }
    }
}
