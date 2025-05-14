using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSite.Migrations
{
    /// <inheritdoc />
    public partial class RenameTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RestaurantTypes",
                newName: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "RestaurantTypes",
                newName: "UserId");
        }
    }
}
