using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletionRulesForExternalAccountAndTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent",
                column: "ExternalAccountId",
                principalTable: "ExternalCalendarAccount",
                principalColumn: "ExternalCalendarAccountId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task",
                column: "ParentTaskId",
                principalTable: "Task",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent",
                column: "ExternalAccountId",
                principalTable: "ExternalCalendarAccount",
                principalColumn: "ExternalCalendarAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task",
                column: "ParentTaskId",
                principalTable: "Task",
                principalColumn: "TaskId");
        }
    }
}
