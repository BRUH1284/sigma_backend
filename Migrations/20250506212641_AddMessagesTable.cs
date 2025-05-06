using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sigma_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c9604fc-d1f4-46b8-b732-29d1e021854c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca8534a4-7af4-419c-a969-032b5de4e404");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    SenderUsername = table.Column<string>(type: "text", nullable: false),
                    ReceiverUsername = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "42945e1d-76b6-4c89-9a91-6814da8ce6cc", null, "Admin", "ADMIN" },
                    { "df494e25-0a93-401f-ab51-656f2e1075f7", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverUsername",
                table: "Messages",
                column: "ReceiverUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUsername",
                table: "Messages",
                column: "SenderUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42945e1d-76b6-4c89-9a91-6814da8ce6cc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "df494e25-0a93-401f-ab51-656f2e1075f7");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5c9604fc-d1f4-46b8-b732-29d1e021854c", null, "User", "USER" },
                    { "ca8534a4-7af4-419c-a969-032b5de4e404", null, "Admin", "ADMIN" }
                });
        }
    }
}
