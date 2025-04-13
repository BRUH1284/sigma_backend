using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sigma_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomDishAndIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21e65678-4070-4a57-8f1c-10d247c0e080");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e0dc5014-e56b-464b-86a7-cdb617df9bb7");

            migrationBuilder.CreateTable(
                name: "CustomDishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomDishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomDishIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomDishId = table.Column<int>(type: "integer", nullable: false),
                    FoodId = table.Column<int>(type: "integer", nullable: false),
                    AmountInGrams = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomDishIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomDishIngredients_CustomDishes_CustomDishId",
                        column: x => x.CustomDishId,
                        principalTable: "CustomDishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomDishIngredients_FoodNutrition_FoodId",
                        column: x => x.FoodId,
                        principalTable: "FoodNutrition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "24232a52-0bb3-4f55-9f5b-5d951ea4db9a", null, "Admin", "ADMIN" },
                    { "a6096734-65be-4295-beb9-412ce6970648", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomDishIngredients_CustomDishId",
                table: "CustomDishIngredients",
                column: "CustomDishId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomDishIngredients_FoodId",
                table: "CustomDishIngredients",
                column: "FoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomDishIngredients");

            migrationBuilder.DropTable(
                name: "CustomDishes");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "24232a52-0bb3-4f55-9f5b-5d951ea4db9a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6096734-65be-4295-beb9-412ce6970648");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21e65678-4070-4a57-8f1c-10d247c0e080", null, "User", "USER" },
                    { "e0dc5014-e56b-464b-86a7-cdb617df9bb7", null, "Admin", "ADMIN" }
                });
        }
    }
}
