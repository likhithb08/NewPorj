using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOCPS.Migrations
{
    /// <inheritdoc />
    public partial class AddScoringConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoringConfigs",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    MinCreditScore = table.Column<int>(type: "int", nullable: false),
                    BureauScoreWeight = table.Column<int>(type: "int", nullable: false),
                    DebtToIncomeWeight = table.Column<int>(type: "int", nullable: false),
                    CreditHistoryAgeWeight = table.Column<int>(type: "int", nullable: false),
                    RepaymentConsistencyWeight = table.Column<int>(type: "int", nullable: false),
                    CreditUtilizationWeight = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringConfigs", x => x.ConfigId);
                    table.ForeignKey(
                        name: "FK_ScoringConfigs_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringConfigs_UpdatedByUserId",
                table: "ScoringConfigs",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoringConfigs");
        }
    }
}
