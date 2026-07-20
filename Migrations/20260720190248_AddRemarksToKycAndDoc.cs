using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCPS.Migrations
{
    /// <inheritdoc />
    public partial class AddRemarksToKycAndDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Kyc",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Document",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Kyc");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Document");
        }
    }
}
