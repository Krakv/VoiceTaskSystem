using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class ProviderReplacedByBaseUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExternalCalendarAccount_OwnerId_Provider",
                table: "ExternalCalendarAccount");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "ExternalCalendarAccount");

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "ExternalCalendarAccount",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarAccount_OwnerId_BaseUrl",
                table: "ExternalCalendarAccount",
                columns: new[] { "OwnerId", "BaseUrl" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExternalCalendarAccount_OwnerId_BaseUrl",
                table: "ExternalCalendarAccount");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                table: "ExternalCalendarAccount");

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "ExternalCalendarAccount",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarAccount_OwnerId_Provider",
                table: "ExternalCalendarAccount",
                columns: new[] { "OwnerId", "Provider" },
                unique: true);
        }
    }
}
