using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCPS.Migrations
{
    /// <inheritdoc />
    public partial class Disbursment_MOdelCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disbursments",
                columns: table => new
                {
                    DisbursmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    AmountApproved = table.Column<int>(type: "int", precision: 13, scale: 2, nullable: false),
                    DisbursmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisbursmentMode = table.Column<int>(type: "int", nullable: false),
                    BankAccountNumber = table.Column<long>(type: "bigint", precision: 12, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedByUserID = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disbursments", x => x.DisbursmentId);
                    table.ForeignKey(
                        name: "FK_Disbursments_LoanApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Disbursments_Users_ProcessedByUserID",
                        column: x => x.ProcessedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disbursments_ApplicationId",
                table: "Disbursments",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Disbursments_ProcessedByUserID",
                table: "Disbursments",
                column: "ProcessedByUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disbursments");
        }
    }
}
