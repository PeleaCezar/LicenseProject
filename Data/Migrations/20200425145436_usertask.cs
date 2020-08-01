using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseProject.Data.Migrations
{
    public partial class usertask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Projects_ProjectID",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Task");

            migrationBuilder.RenameTable(
                name: "Task",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_Task_ProjectID",
                table: "Tasks",
                newName: "IX_Tasks_ProjectID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "UserTasks",
                columns: table => new
                {
                    MyUserId = table.Column<string>(nullable: false),
                    TaskID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasks", x => new { x.MyUserId, x.TaskID });
                    table.ForeignKey(
                        name: "FK_UserTasks_AspNetUsers_MyUserId",
                        column: x => x.MyUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTasks_Tasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "Tasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TaskID",
                table: "UserTasks",
                column: "TaskID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectID",
                table: "Tasks",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectID",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "Task");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ProjectID",
                table: "Task",
                newName: "IX_Task_ProjectID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Task",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Projects_ProjectID",
                table: "Task",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
