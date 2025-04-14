using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sigma_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMealsAndMealFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2513c1dc-f3e7-4312-9338-8b41af0c5e54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dd9de46c-367f-4f29-967d-384fb726b068");

            migrationBuilder.CreateTable(
                name: "FoodNutrition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Column0 = table.Column<string>(type: "text", nullable: false),
                    Unnamed0 = table.Column<int>(type: "integer", nullable: false),
                    Food = table.Column<string>(type: "text", nullable: false),
                    CaloricValue = table.Column<float>(type: "real", nullable: false),
                    Fat = table.Column<float>(type: "real", nullable: false),
                    SaturatedFats = table.Column<float>(type: "real", nullable: false),
                    MonounsaturatedFats = table.Column<float>(type: "real", nullable: false),
                    PolyunsaturatedFats = table.Column<float>(type: "real", nullable: false),
                    Carbohydrates = table.Column<float>(type: "real", nullable: false),
                    Sugars = table.Column<float>(type: "real", nullable: false),
                    Protein = table.Column<float>(type: "real", nullable: false),
                    DietaryFiber = table.Column<float>(type: "real", nullable: false),
                    Cholesterol = table.Column<float>(type: "real", nullable: false),
                    Sodium = table.Column<float>(type: "real", nullable: false),
                    Water = table.Column<float>(type: "real", nullable: false),
                    VitaminA = table.Column<float>(type: "real", nullable: false),
                    VitaminB1 = table.Column<float>(type: "real", nullable: false),
                    VitaminB11 = table.Column<float>(type: "real", nullable: false),
                    VitaminB12 = table.Column<float>(type: "real", nullable: false),
                    VitaminB2 = table.Column<float>(type: "real", nullable: false),
                    VitaminB3 = table.Column<float>(type: "real", nullable: false),
                    VitaminB5 = table.Column<float>(type: "real", nullable: false),
                    VitaminB6 = table.Column<float>(type: "real", nullable: false),
                    VitaminC = table.Column<float>(type: "real", nullable: false),
                    VitaminD = table.Column<float>(type: "real", nullable: false),
                    VitaminE = table.Column<float>(type: "real", nullable: false),
                    VitaminK = table.Column<float>(type: "real", nullable: false),
                    Calcium = table.Column<float>(type: "real", nullable: false),
                    Copper = table.Column<float>(type: "real", nullable: false),
                    Iron = table.Column<float>(type: "real", nullable: false),
                    Magnesium = table.Column<float>(type: "real", nullable: false),
                    Manganese = table.Column<float>(type: "real", nullable: false),
                    Phosphorus = table.Column<float>(type: "real", nullable: false),
                    Potassium = table.Column<float>(type: "real", nullable: false),
                    Selenium = table.Column<float>(type: "real", nullable: false),
                    Zinc = table.Column<float>(type: "real", nullable: false),
                    NutritionDensity = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutrition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FoodId = table.Column<int>(type: "integer", nullable: false),
                    AmountInGrams = table.Column<double>(type: "double precision", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Meals_FoodNutrition_FoodId",
                        column: x => x.FoodId,
                        principalTable: "FoodNutrition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MealId = table.Column<int>(type: "integer", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealFiles_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "986d6b09-5519-45a4-a70f-a0542a60bf64", null, "User", "USER" },
                    { "c2e4a8aa-3bb4-471c-ab3e-69046839cb61", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealFiles_MealId",
                table: "MealFiles",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_FoodId",
                table: "Meals",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_UserId",
                table: "Meals",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealFiles");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "FoodNutrition");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "986d6b09-5519-45a4-a70f-a0542a60bf64");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2e4a8aa-3bb4-471c-ab3e-69046839cb61");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2513c1dc-f3e7-4312-9338-8b41af0c5e54", null, "User", "USER" },
                    { "dd9de46c-367f-4f29-967d-384fb726b068", null, "Admin", "ADMIN" }
                });
        }
    }
}
