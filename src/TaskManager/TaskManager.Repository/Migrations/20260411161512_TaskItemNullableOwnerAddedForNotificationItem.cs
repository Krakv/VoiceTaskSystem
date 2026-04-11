using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class TaskItemNullableOwnerAddedForNotificationItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Task_TaskId",
                table: "Notification");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Notification",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Notification",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OwnerId",
                table: "Notification",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_OwnerId",
                table: "Notification",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Task_TaskId",
                table: "Notification",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_OwnerId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Task_TaskId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_OwnerId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Notification");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Notification",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Task_TaskId",
                table: "Notification",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
