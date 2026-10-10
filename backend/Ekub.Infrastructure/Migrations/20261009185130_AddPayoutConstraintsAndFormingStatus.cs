using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ekub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutConstraintsAndFormingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payouts_CircleId",
                table: "Payouts");

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_CircleId_MemberId",
                table: "Payouts",
                columns: new[] { "CircleId", "MemberId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payouts_CircleId_MemberId",
                table: "Payouts");

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_CircleId",
                table: "Payouts",
                column: "CircleId");
        }
    }
}
