using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class ReviewCategoryScores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCategoryScore_Reviews_ReviewId",
                table: "ReviewCategoryScore");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCategoryScore_ScoreCategories_ScoreCategoryId",
                table: "ReviewCategoryScore");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCategoryScore_Scores_ScoreId",
                table: "ReviewCategoryScore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewCategoryScore",
                table: "ReviewCategoryScore");

            migrationBuilder.RenameTable(
                name: "ReviewCategoryScore",
                newName: "ReviewCategoryScores");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewCategoryScore_ScoreId",
                table: "ReviewCategoryScores",
                newName: "IX_ReviewCategoryScores_ScoreId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewCategoryScore_ScoreCategoryId",
                table: "ReviewCategoryScores",
                newName: "IX_ReviewCategoryScores_ScoreCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewCategoryScores",
                table: "ReviewCategoryScores",
                columns: new[] { "ReviewId", "ScoreCategoryId", "ScoreId" });

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: (short)189,
                column: "Name",
                value: "Serbia");

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[] { (short)240, "System", new DateTime(2022, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Montenegro", null });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCategoryScores_Reviews_ReviewId",
                table: "ReviewCategoryScores",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCategoryScores_ScoreCategories_ScoreCategoryId",
                table: "ReviewCategoryScores",
                column: "ScoreCategoryId",
                principalTable: "ScoreCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCategoryScores_Scores_ScoreId",
                table: "ReviewCategoryScores",
                column: "ScoreId",
                principalTable: "Scores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCategoryScores_Reviews_ReviewId",
                table: "ReviewCategoryScores");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCategoryScores_ScoreCategories_ScoreCategoryId",
                table: "ReviewCategoryScores");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCategoryScores_Scores_ScoreId",
                table: "ReviewCategoryScores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewCategoryScores",
                table: "ReviewCategoryScores");

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: (short)240);

            migrationBuilder.RenameTable(
                name: "ReviewCategoryScores",
                newName: "ReviewCategoryScore");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewCategoryScores_ScoreId",
                table: "ReviewCategoryScore",
                newName: "IX_ReviewCategoryScore_ScoreId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewCategoryScores_ScoreCategoryId",
                table: "ReviewCategoryScore",
                newName: "IX_ReviewCategoryScore_ScoreCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewCategoryScore",
                table: "ReviewCategoryScore",
                columns: new[] { "ReviewId", "ScoreCategoryId", "ScoreId" });

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: (short)189,
                column: "Name",
                value: "Serbia and Montenegro");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCategoryScore_Reviews_ReviewId",
                table: "ReviewCategoryScore",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCategoryScore_ScoreCategories_ScoreCategoryId",
                table: "ReviewCategoryScore",
                column: "ScoreCategoryId",
                principalTable: "ScoreCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCategoryScore_Scores_ScoreId",
                table: "ReviewCategoryScore",
                column: "ScoreId",
                principalTable: "Scores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
