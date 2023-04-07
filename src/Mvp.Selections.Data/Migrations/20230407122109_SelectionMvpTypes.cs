using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class SelectionMvpTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MvpTypeSelection",
                columns: table => new
                {
                    MvpTypesId = table.Column<short>(type: "smallint", nullable: false),
                    SelectionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MvpTypeSelection", x => new { x.MvpTypesId, x.SelectionsId });
                    table.ForeignKey(
                        name: "FK_MvpTypeSelection_MvpType_MvpTypesId",
                        column: x => x.MvpTypesId,
                        principalTable: "MvpType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MvpTypeSelection_Selections_SelectionsId",
                        column: x => x.SelectionsId,
                        principalTable: "Selections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MvpTypeSelection_SelectionsId",
                table: "MvpTypeSelection",
                column: "SelectionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MvpTypeSelection");
        }
    }
}
