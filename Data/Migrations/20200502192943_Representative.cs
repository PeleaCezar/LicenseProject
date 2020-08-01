using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class Representative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "GrantApplications");

            migrationBuilder.AddColumn<string>(
                name: "Representantive",
                table: "GrantApplications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Representantive",
                table: "GrantApplications");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "GrantApplications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
