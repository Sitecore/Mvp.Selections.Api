using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class TitlesUniqueApplicationIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Discriminator", "ModifiedBy", "ModifiedOn", "Name", "Rights" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SystemRole", null, null, "Scorer", 8 });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Discriminator", "ModifiedBy", "ModifiedOn", "Name", "Rights" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SystemRole", null, null, "Commenter", 16 });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Discriminator", "ModifiedBy", "ModifiedOn", "Name", "Rights" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), "System", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SystemRole", null, null, "Awarder", 32 });

            migrationBuilder.CreateIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles",
                column: "ApplicationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.CreateIndex(
                name: "IX_Titles_ApplicationId",
                table: "Titles",
                column: "ApplicationId");
        }
    }
}
