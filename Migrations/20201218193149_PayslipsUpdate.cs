using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class PayslipsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reimbursement",
                table: "PayslipMasters");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "PayslipMasters",
                newName: "UserName");

            migrationBuilder.AddColumn<double>(
                name: "Arrears",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Basic",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CEA",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EmpEPF",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EmpESI",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "PayslipMasters",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HRA",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LTA",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyGross",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SPL",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TA",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TAX",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WorkingDays",
                table: "PayslipMasters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arrears",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "Basic",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "CEA",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "EmpEPF",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "EmpESI",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "HRA",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "LTA",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "MonthlyGross",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "SPL",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "TA",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "TAX",
                table: "PayslipMasters");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "PayslipMasters");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "PayslipMasters",
                newName: "Username");

            migrationBuilder.AddColumn<double>(
                name: "Reimbursement",
                table: "PayslipMasters",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
