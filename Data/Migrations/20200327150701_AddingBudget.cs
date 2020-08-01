using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class AddingBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimatedBudget = table.Column<decimal>(nullable: false),
                    AmountSpent = table.Column<decimal>(nullable: false),
                    ProjectDuration = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Faculty = table.Column<string>(nullable: true),
                    ProjectDirector = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountSpent",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectDirector",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectDuration",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
