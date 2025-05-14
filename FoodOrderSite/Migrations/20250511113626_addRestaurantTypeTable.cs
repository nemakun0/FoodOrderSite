using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSite.Migrations
{
    /// <inheritdoc />
    public partial class addRestaurantTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantTypes",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantTypes", x => x.UserId);
                });


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "RestaurantTypes");

        }
    }
}
