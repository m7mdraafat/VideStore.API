using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class makeAdressNameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_AddressName",
                table: "UserAddresses");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressName_AppUserId",
                table: "UserAddresses",
                columns: new[] { "AddressName", "AppUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_AddressName_AppUserId",
                table: "UserAddresses");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressName",
                table: "UserAddresses",
                column: "AddressName");
        }
    }
}
