using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTitleApplicationIdUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles",
                column: "ApplicationId",
                unique: true);
        }
    }
}
