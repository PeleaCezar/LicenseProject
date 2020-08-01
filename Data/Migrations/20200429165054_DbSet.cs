using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class DbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrantApplication_Projects_ProjectID",
                table: "GrantApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GrantApplication",
                table: "GrantApplication");

            migrationBuilder.RenameTable(
                name: "GrantApplication",
                newName: "GrantApplications");

            migrationBuilder.RenameIndex(
                name: "IX_GrantApplication_ProjectID",
                table: "GrantApplications",
                newName: "IX_GrantApplications_ProjectID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrantApplications",
                table: "GrantApplications",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GrantApplications_Projects_ProjectID",
                table: "GrantApplications",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrantApplications_Projects_ProjectID",
                table: "GrantApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GrantApplications",
                table: "GrantApplications");

            migrationBuilder.RenameTable(
                name: "GrantApplications",
                newName: "GrantApplication");

            migrationBuilder.RenameIndex(
                name: "IX_GrantApplications_ProjectID",
                table: "GrantApplication",
                newName: "IX_GrantApplication_ProjectID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrantApplication",
                table: "GrantApplication",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GrantApplication_Projects_ProjectID",
                table: "GrantApplication",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
