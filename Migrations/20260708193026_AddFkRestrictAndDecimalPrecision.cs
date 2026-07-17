using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCPS.Migrations
{
    /// <inheritdoc />
    public partial class AddFkRestrictAndDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_LoanProducts_ProductId",
                table: "LoanApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_Users_CreatedByUserId",
                table: "LoanApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_Users_CustomerId",
                table: "LoanApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_LoanProducts_ProductId",
                table: "LoanApplications",
                column: "ProductId",
                principalTable: "LoanProducts",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_Users_CreatedByUserId",
                table: "LoanApplications",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_Users_CustomerId",
                table: "LoanApplications",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_LoanProducts_ProductId",
                table: "LoanApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_Users_CreatedByUserId",
                table: "LoanApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_Users_CustomerId",
                table: "LoanApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_LoanProducts_ProductId",
                table: "LoanApplications",
                column: "ProductId",
                principalTable: "LoanProducts",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_Users_CreatedByUserId",
                table: "LoanApplications",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_Users_CustomerId",
                table: "LoanApplications",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
