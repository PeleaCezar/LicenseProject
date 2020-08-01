using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class AddingProjectForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BudgetId",
                table: "Projects",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Budgets_BudgetId",
                table: "Projects",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Budgets_BudgetId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BudgetId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Projects");
        }
    }
}
