using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class BudgetField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountSpent",
                table: "Projects",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Projects",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectDirector",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectDuration",
                table: "Projects",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountSpent",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectDirector",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectDuration",
                table: "Projects");
        }
    }
}
