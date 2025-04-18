using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sigma_backend.Migrations
{
    /// <inheritdoc />
    public partial class ProfilePicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cb18a81c-c15b-46ed-a666-78bbf2017374");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eca93617-c66d-4ca9-87e0-09d874073248");

            migrationBuilder.CreateTable(
                name: "ProfilePictures",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePictures", x => new { x.UserId, x.FileName });
                    table.ForeignKey(
                        name: "FK_ProfilePictures_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "58c7a88e-e377-40b3-8cc4-a41677b9b454", null, "User", "USER" },
                    { "c1810357-d561-415f-96a8-6bd21f558f24", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilePictures");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58c7a88e-e377-40b3-8cc4-a41677b9b454");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1810357-d561-415f-96a8-6bd21f558f24");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "cb18a81c-c15b-46ed-a666-78bbf2017374", null, "Admin", "ADMIN" },
                    { "eca93617-c66d-4ca9-87e0-09d874073248", null, "User", "USER" }
                });
        }
    }
}
