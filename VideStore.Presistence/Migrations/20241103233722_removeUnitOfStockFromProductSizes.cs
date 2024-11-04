using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removeUnitOfStockFromProductSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfStock",
                table: "ProductSizes");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "UnitOfStock",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
