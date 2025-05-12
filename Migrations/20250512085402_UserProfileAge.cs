using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sigma_backend.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileAge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a9d03de-ab2b-4ecb-9bf8-f4a09d957728");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "906ec889-c10d-4a84-9993-75b43f046e16");

            migrationBuilder.AddColumn<float>(
                name: "Age",
                table: "UserProfiles",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "67ae9196-469c-4e3a-951e-e74cd2d5021a", null, "User", "USER" },
                    { "dea306aa-6a1a-4b80-ac8f-722f95f45685", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "67ae9196-469c-4e3a-951e-e74cd2d5021a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dea306aa-6a1a-4b80-ac8f-722f95f45685");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "UserProfiles");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4a9d03de-ab2b-4ecb-9bf8-f4a09d957728", null, "User", "USER" },
                    { "906ec889-c10d-4a84-9993-75b43f046e16", null, "Admin", "ADMIN" }
                });
        }
    }
}
