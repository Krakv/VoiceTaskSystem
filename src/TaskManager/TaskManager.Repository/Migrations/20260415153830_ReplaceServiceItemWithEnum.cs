using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceServiceItemWithEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Service_ServiceId",
                table: "Notification");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ServiceId",
                table: "Notification");

            migrationBuilder.Sql(
                @"UPDATE ""AspNetUsers""
                  SET ""TelegramChatId"" = NULL
                  WHERE ""TelegramChatId"" !~ '^\d+$';"
            );

            migrationBuilder.Sql(
                @"ALTER TABLE ""AspNetUsers"" 
                  ALTER COLUMN ""TelegramChatId"" 
                  TYPE bigint 
                  USING ""TelegramChatId""::bigint;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TelegramChatId",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.ServiceId);
                });

            migrationBuilder.InsertData(
                table: "Service",
                columns: new[] { "ServiceId", "ServiceName" },
                values: new object[,]
                {
                    { 1, "Telegram" },
                    { 2, "Email" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ServiceId",
                table: "Notification",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceName",
                table: "Service",
                column: "ServiceName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Service_ServiceId",
                table: "Notification",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
