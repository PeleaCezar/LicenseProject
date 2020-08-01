using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class UsersProjectDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BudgetId",
                table: "AspNetUsers",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Budgets_BudgetId",
                table: "AspNetUsers",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Budgets_BudgetId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BudgetId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "AspNetUsers");
        }
    }
}
