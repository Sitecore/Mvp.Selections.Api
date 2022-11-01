using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class ScoreCategoryHierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileLinks_Users_UserId",
                table: "ProfileLinks");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId",
                table: "ScoreCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ProfileLinks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategories_ParentCategoryId",
                table: "ScoreCategories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileLinks_Users_UserId",
                table: "ProfileLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreCategories_ScoreCategories_ParentCategoryId",
                table: "ScoreCategories",
                column: "ParentCategoryId",
                principalTable: "ScoreCategories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileLinks_Users_UserId",
                table: "ProfileLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_ScoreCategories_ScoreCategories_ParentCategoryId",
                table: "ScoreCategories");

            migrationBuilder.DropIndex(
                name: "IX_ScoreCategories_ParentCategoryId",
                table: "ScoreCategories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "ScoreCategories");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ProfileLinks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileLinks_Users_UserId",
                table: "ProfileLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
