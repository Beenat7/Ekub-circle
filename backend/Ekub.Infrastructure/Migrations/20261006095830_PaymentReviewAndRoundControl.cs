using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ekub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentReviewAndRoundControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rounds_CircleId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RoundId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Payments",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByMemberId",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Payments\" SET \"BankName\" = 'Legacy payment; bank not recorded' WHERE \"BankName\" IS NULL; " +
                "UPDATE \"Payments\" SET \"Status\" = 1 WHERE \"Status\" IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "Payments",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_CircleId_RoundNumber",
                table: "Rounds",
                columns: new[] { "CircleId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReviewedByMemberId",
                table: "Payments",
                column: "ReviewedByMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RoundId_MemberId",
                table: "Payments",
                columns: new[] { "RoundId", "MemberId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Members_ReviewedByMemberId",
                table: "Payments",
                column: "ReviewedByMemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Members_ReviewedByMemberId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_CircleId_RoundNumber",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ReviewedByMemberId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RoundId_MemberId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ReviewedByMemberId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_CircleId",
                table: "Rounds",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RoundId",
                table: "Payments",
                column: "RoundId");
        }
    }
}
