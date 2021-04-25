using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class IsDocsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsAadhaarConfirmed",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDegreeConfirmed",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPanConfirmed",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAadhaarConfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDegreeConfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsPanConfirmed",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAadhaarUpload",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDegreeUpload",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPanUpload",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
