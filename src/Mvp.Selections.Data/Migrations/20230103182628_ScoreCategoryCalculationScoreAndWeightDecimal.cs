using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class ScoreCategoryCalculationScoreAndWeightDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "ScoreCategories",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "CalculationScoreId",
                table: "ScoreCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategories_CalculationScoreId",
                table: "ScoreCategories",
                column: "CalculationScoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreCategories_Scores_CalculationScoreId",
                table: "ScoreCategories",
                column: "CalculationScoreId",
                principalTable: "Scores",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreCategories_Scores_CalculationScoreId",
                table: "ScoreCategories");

            migrationBuilder.DropIndex(
                name: "IX_ScoreCategories_CalculationScoreId",
                table: "ScoreCategories");

            migrationBuilder.DropColumn(
                name: "CalculationScoreId",
                table: "ScoreCategories");

            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "ScoreCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
