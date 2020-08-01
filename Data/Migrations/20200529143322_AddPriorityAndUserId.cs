using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class AddPriorityAndUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTasks");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "UserTasks",
                columns: table => new
                {
                    MyUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasks", x => new { x.MyUserId, x.TaskID });
                    table.ForeignKey(
                        name: "FK_UserTasks_AspNetUsers_MyUserId",
                        column: x => x.MyUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTasks_Tasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "Tasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TaskID",
                table: "UserTasks",
                column: "TaskID");
        }
    }
}
