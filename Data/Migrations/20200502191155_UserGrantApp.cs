using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class UserGrantApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MyUserId",
                table: "GrantApplications",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "GrantApplications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrantApplications_MyUserId",
                table: "GrantApplications",
                column: "MyUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GrantApplications_AspNetUsers_MyUserId",
                table: "GrantApplications",
                column: "MyUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrantApplications_AspNetUsers_MyUserId",
                table: "GrantApplications");

            migrationBuilder.DropIndex(
                name: "IX_GrantApplications_MyUserId",
                table: "GrantApplications");

            migrationBuilder.DropColumn(
                name: "MyUserId",
                table: "GrantApplications");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "GrantApplications");
        }
    }
}
