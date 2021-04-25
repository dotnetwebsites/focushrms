using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FocusHRMS.Migrations
{
    public partial class DocumentUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "OfferLetters");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "OfferLetters",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentLists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailLibraries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MailUserId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Host = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Port = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    EnableSsl = table.Column<bool>(nullable: false),
                    UseDefaultCredentials = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLibraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDocuments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentLists");

            migrationBuilder.DropTable(
                name: "MailLibraries");

            migrationBuilder.DropTable(
                name: "MyDocuments");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "OfferLetters");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "OfferLetters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
