using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCPS.Migrations
{
    /// <inheritdoc />
    public partial class NewModels_KYC_and_CreditEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditEvaluation",
                columns: table => new
                {
                    CreditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    CreditScore = table.Column<int>(type: "int", nullable: false),
                    DebitToIncomeRatio = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PaymentHistoryScore = table.Column<int>(type: "int", nullable: false),
                    ExistingLiabilities = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditRecomendations = table.Column<int>(type: "int", nullable: true),
                    EvaluatedByUserId = table.Column<int>(type: "int", nullable: true),
                    EvaluatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditEvaluation", x => x.CreditId);
                    table.ForeignKey(
                        name: "FK_CreditEvaluation_LoanApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditEvaluation_Users_EvaluatedByUserId",
                        column: x => x.EvaluatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kyc",
                columns: table => new
                {
                    KycId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    AdhaarNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PanNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    AddressProof = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdentityProof = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IncomeProof = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    VerifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kyc", x => x.KycId);
                    table.ForeignKey(
                        name: "FK_Kyc_LoanApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kyc_Users_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditEvaluation_ApplicationId",
                table: "CreditEvaluation",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditEvaluation_EvaluatedByUserId",
                table: "CreditEvaluation",
                column: "EvaluatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Kyc_ApplicationId",
                table: "Kyc",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Kyc_VerifiedByUserId",
                table: "Kyc",
                column: "VerifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditEvaluation");

            migrationBuilder.DropTable(
                name: "Kyc");
        }
    }
}
