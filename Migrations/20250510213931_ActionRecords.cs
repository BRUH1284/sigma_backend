using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sigma_backend.Migrations
{
    /// <inheritdoc />
    public partial class ActionRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42945e1d-76b6-4c89-9a91-6814da8ce6cc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "df494e25-0a93-401f-ab51-656f2e1075f7");

            migrationBuilder.CreateTable(
                name: "ActivityRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<float>(type: "real", nullable: false),
                    Kcal = table.Column<float>(type: "real", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ActivityCode = table.Column<int>(type: "integer", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityRecords_Activities_ActivityCode",
                        column: x => x.ActivityCode,
                        principalTable: "Activities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityRecords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityRecords_UserActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4a9d03de-ab2b-4ecb-9bf8-f4a09d957728", null, "User", "USER" },
                    { "906ec889-c10d-4a84-9993-75b43f046e16", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecords_ActivityCode",
                table: "ActivityRecords",
                column: "ActivityCode");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecords_ActivityId",
                table: "ActivityRecords",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecords_UserId",
                table: "ActivityRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecords_UserId_LastModified",
                table: "ActivityRecords",
                columns: new[] { "UserId", "LastModified" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRecords");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a9d03de-ab2b-4ecb-9bf8-f4a09d957728");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "906ec889-c10d-4a84-9993-75b43f046e16");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "42945e1d-76b6-4c89-9a91-6814da8ce6cc", null, "Admin", "ADMIN" },
                    { "df494e25-0a93-401f-ab51-656f2e1075f7", null, "User", "USER" }
                });
        }
    }
}
