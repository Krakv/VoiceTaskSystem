using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class RemovedExternalEventIdForCalendarEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalEventId",
                table: "CalendarEvent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalEventId",
                table: "CalendarEvent",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
