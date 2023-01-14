using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class RemoveSelectionAndUserFromTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Title_Applications_ApplicationId",
                table: "Title");

            migrationBuilder.DropForeignKey(
                name: "FK_Title_MvpType_MvpTypeId",
                table: "Title");

            migrationBuilder.DropForeignKey(
                name: "FK_Title_Selections_SelectionId",
                table: "Title");

            migrationBuilder.DropForeignKey(
                name: "FK_Title_Users_UserId",
                table: "Title");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Title",
                table: "Title");

            migrationBuilder.DropIndex(
                name: "IX_Title_SelectionId",
                table: "Title");

            migrationBuilder.DropIndex(
                name: "IX_Title_UserId",
                table: "Title");

            migrationBuilder.DropColumn(
                name: "SelectionId",
                table: "Title");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Title");

            migrationBuilder.RenameTable(
                name: "Title",
                newName: "Titles");

            migrationBuilder.RenameIndex(
                name: "IX_Title_MvpTypeId",
                table: "Titles",
                newName: "IX_Titles_MvpTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Title_ApplicationId",
                table: "Titles",
                newName: "IX_Titles_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Titles",
                table: "Titles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Titles_Applications_ApplicationId",
                table: "Titles",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Titles_MvpType_MvpTypeId",
                table: "Titles",
                column: "MvpTypeId",
                principalTable: "MvpType",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Titles_Applications_ApplicationId",
                table: "Titles");

            migrationBuilder.DropForeignKey(
                name: "FK_Titles_MvpType_MvpTypeId",
                table: "Titles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Titles",
                table: "Titles");

            migrationBuilder.RenameTable(
                name: "Titles",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Titles_MvpTypeId",
                table: "Title",
                newName: "IX_Title_MvpTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Titles_ApplicationId",
                table: "Title",
                newName: "IX_Title_ApplicationId");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectionId",
                table: "Title",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Title",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Title",
                table: "Title",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Title_SelectionId",
                table: "Title",
                column: "SelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Title_UserId",
                table: "Title",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Title_Applications_ApplicationId",
                table: "Title",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Title_MvpType_MvpTypeId",
                table: "Title",
                column: "MvpTypeId",
                principalTable: "MvpType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Title_Selections_SelectionId",
                table: "Title",
                column: "SelectionId",
                principalTable: "Selections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Title_Users_UserId",
                table: "Title",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
