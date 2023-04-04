using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class ApplicantSelectionUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicantId",
                table: "Applications");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantId_SelectionId",
                table: "Applications",
                columns: new[] { "ApplicantId", "SelectionId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicantId_SelectionId",
                table: "Applications");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantId",
                table: "Applications",
                column: "ApplicantId");
        }
    }
}
