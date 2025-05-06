using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFoodItemCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodItemCategoriesTables",
                columns: table => new
                {
                    FoodItemId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItemCategoriesTables", x => new { x.FoodItemId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_FoodItemCategoriesTables_CategoriesTables_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoriesTables",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodItemCategoriesTables_FoodItemTables_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItemTables",
                        principalColumn: "FoodItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodItemCategoriesTables_CategoryId",
                table: "FoodItemCategoriesTables",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodItemCategoriesTables");
        }
    }
}
