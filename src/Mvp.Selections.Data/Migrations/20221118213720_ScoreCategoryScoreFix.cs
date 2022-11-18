using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class ScoreCategoryScoreFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_ScoreCategories_ScoreCategoryId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_ScoreCategoryId",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "ScoreCategoryId",
                table: "Scores");

            migrationBuilder.CreateTable(
                name: "ScoreScoreCategory",
                columns: table => new
                {
                    ScoreCategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScoreOptionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreScoreCategory", x => new { x.ScoreCategoriesId, x.ScoreOptionsId });
                    table.ForeignKey(
                        name: "FK_ScoreScoreCategory_ScoreCategories_ScoreCategoriesId",
                        column: x => x.ScoreCategoriesId,
                        principalTable: "ScoreCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreScoreCategory_Scores_ScoreOptionsId",
                        column: x => x.ScoreOptionsId,
                        principalTable: "Scores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreScoreCategory_ScoreOptionsId",
                table: "ScoreScoreCategory",
                column: "ScoreOptionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreScoreCategory");

            migrationBuilder.AddColumn<Guid>(
                name: "ScoreCategoryId",
                table: "Scores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_ScoreCategoryId",
                table: "Scores",
                column: "ScoreCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_ScoreCategories_ScoreCategoryId",
                table: "Scores",
                column: "ScoreCategoryId",
                principalTable: "ScoreCategories",
                principalColumn: "Id");
        }
    }
}
