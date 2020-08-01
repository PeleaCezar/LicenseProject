using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class ExternalFinanceandCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalCurrency",
                table: "Budgets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExternalFinance",
                table: "Budgets",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalCurrency",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "ExternalFinance",
                table: "Budgets");
        }
    }
}
