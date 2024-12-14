using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeAddressNameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressName",
                table: "UserAddresses",
                column: "AddressName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_AddressName",
                table: "UserAddresses");
        }
    }
}
