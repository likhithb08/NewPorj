using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCPS.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_Users_CreatedByUserId",
                table: "LoanProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_Users_CreatedByUserId",
                table: "LoanProducts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_Users_CreatedByUserId",
                table: "LoanProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_Users_CreatedByUserId",
                table: "LoanProducts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
