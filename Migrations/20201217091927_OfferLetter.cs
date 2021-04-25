using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class OfferLetter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duductions",
                table: "PayslipMasters");

            migrationBuilder.AddColumn<double>(
                name: "Deductions",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "OfferLetters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    OfferId = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLetters", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferLetters");

            migrationBuilder.DropColumn(
                name: "Deductions",
                table: "PayslipMasters");

            migrationBuilder.AddColumn<double>(
                name: "Duductions",
                table: "PayslipMasters",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
