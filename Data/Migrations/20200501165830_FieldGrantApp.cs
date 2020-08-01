using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class FieldGrantApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoOfGrantApp",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "GrantApplications",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfGrantApp",
                table: "GrantApplications",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "GrantApplications");

            migrationBuilder.DropColumn(
                name: "NoOfGrantApp",
                table: "GrantApplications");

            migrationBuilder.AddColumn<int>(
                name: "NoOfGrantApp",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
