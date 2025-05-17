using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSite.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToCustomerDeliveryAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CustomerDeliveryAdderss",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "CustomerDeliveryAdderss");
        }
    }
}
