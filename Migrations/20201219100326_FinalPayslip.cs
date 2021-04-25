using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class FinalPayslip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deductions",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "GrossPay",
                table: "PayslipMasters");

            migrationBuilder.AddColumn<double>(
                name: "OtherDeduction",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TDS",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalDeductions",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherDeduction",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "TDS",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "TotalDeductions",
                table: "PayslipMasters");

            migrationBuilder.AddColumn<double>(
                name: "Deductions",
                table: "PayslipMasters",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GrossPay",
                table: "PayslipMasters",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
