using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class findate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinDate",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinDate",
                table: "PayslipMasters");
        }
    }
}
