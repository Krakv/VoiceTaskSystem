using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNullableFieldsForCalendarEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_Task_TaskId",
                table: "CalendarEvent");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "CalendarEvent",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalAccountId",
                table: "CalendarEvent",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "CalendarEvent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_OwnerId",
                table: "CalendarEvent",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_AspNetUsers_OwnerId",
                table: "CalendarEvent",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent",
                column: "ExternalAccountId",
                principalTable: "ExternalCalendarAccount",
                principalColumn: "ExternalCalendarAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_Task_TaskId",
                table: "CalendarEvent",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_AspNetUsers_OwnerId",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_Task_TaskId",
                table: "CalendarEvent");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvent_OwnerId",
                table: "CalendarEvent");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "CalendarEvent");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "CalendarEvent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalAccountId",
                table: "CalendarEvent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_ExternalCalendarAccount_ExternalAccountId",
                table: "CalendarEvent",
                column: "ExternalAccountId",
                principalTable: "ExternalCalendarAccount",
                principalColumn: "ExternalCalendarAccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_Task_TaskId",
                table: "CalendarEvent",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
