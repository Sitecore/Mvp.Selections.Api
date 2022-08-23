using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvp.Selections.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MvpType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MvpType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Selections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    ApplicationsActive = table.Column<bool>(type: "bit", nullable: true),
                    ApplicationsStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationsEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewsActive = table.Column<bool>(type: "bit", nullable: true),
                    ReviewsStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewsEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selections", x => x.Id);
                    table.UniqueConstraint("AK_Selections_Year", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScoreCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    MvpTypeId = table.Column<short>(type: "smallint", nullable: false),
                    SelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreCategories_MvpType_MvpTypeId",
                        column: x => x.MvpTypeId,
                        principalTable: "MvpType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreCategories_Selections_SelectionId",
                        column: x => x.SelectionId,
                        principalTable: "Selections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageType = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<short>(type: "smallint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Identifier", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_Users_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    ScoreCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_ScoreCategories_ScoreCategoryId",
                        column: x => x.ScoreCategoryId,
                        principalTable: "ScoreCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Eligibility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Objectives = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mentor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<short>(type: "smallint", nullable: false),
                    MvpTypeId = table.Column<short>(type: "smallint", nullable: false),
                    SelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_MvpType_MvpTypeId",
                        column: x => x.MvpTypeId,
                        principalTable: "MvpType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Selections_SelectionId",
                        column: x => x.SelectionId,
                        principalTable: "Selections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Users_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Consents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GivenOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationLinks_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CountryId = table.Column<short>(type: "smallint", nullable: true),
                    MvpTypeId = table.Column<short>(type: "smallint", nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    SelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rights = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_MvpType_MvpTypeId",
                        column: x => x.MvpTypeId,
                        principalTable: "MvpType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_Selections_SelectionId",
                        column: x => x.SelectionId,
                        principalTable: "Selections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Title",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Warning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MvpTypeId = table.Column<short>(type: "smallint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Title", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Title_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Title_MvpType_MvpTypeId",
                        column: x => x.MvpTypeId,
                        principalTable: "MvpType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Title_Selections_SelectionId",
                        column: x => x.SelectionId,
                        principalTable: "Selections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Title_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLinkProduct",
                columns: table => new
                {
                    ApplicationLinksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLinkProduct", x => new { x.ApplicationLinksId, x.RelatedProductsId });
                    table.ForeignKey(
                        name: "FK_ApplicationLinkProduct_ApplicationLinks_ApplicationLinksId",
                        column: x => x.ApplicationLinksId,
                        principalTable: "ApplicationLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationLinkProduct_Products_RelatedProductsId",
                        column: x => x.RelatedProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewCategoryScore",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScoreCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCategoryScore", x => new { x.ReviewId, x.ScoreCategoryId, x.ScoreId });
                    table.ForeignKey(
                        name: "FK_ReviewCategoryScore_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewCategoryScore_ScoreCategories_ScoreCategoryId",
                        column: x => x.ScoreCategoryId,
                        principalTable: "ScoreCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCategoryScore_Scores_ScoreId",
                        column: x => x.ScoreId,
                        principalTable: "Scores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)1, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2394), null, null, "Afghanistan ", null },
                    { (short)2, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2398), null, null, "Albania ", null },
                    { (short)3, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2399), null, null, "Algeria ", null },
                    { (short)4, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2400), null, null, "American Samoa ", null },
                    { (short)5, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2400), null, null, "Andorra ", null },
                    { (short)6, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2403), null, null, "Angola ", null },
                    { (short)7, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2404), null, null, "Anguilla ", null },
                    { (short)8, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2404), null, null, "Antarctica ", null },
                    { (short)9, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2405), null, null, "Antigua and Barbuda ", null },
                    { (short)10, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2406), null, null, "Argentina ", null },
                    { (short)11, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2407), null, null, "Armenia ", null },
                    { (short)12, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2408), null, null, "Aruba ", null },
                    { (short)13, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2408), null, null, "Australia ", null },
                    { (short)14, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2409), null, null, "Austria ", null },
                    { (short)15, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2410), null, null, "Azerbaijan ", null },
                    { (short)16, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2410), null, null, "Bahamas ", null },
                    { (short)17, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2411), null, null, "Bahrain ", null },
                    { (short)18, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2412), null, null, "Bangladesh ", null },
                    { (short)19, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2413), null, null, "Barbados ", null },
                    { (short)20, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2415), null, null, "Belarus ", null },
                    { (short)21, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2415), null, null, "Belgium ", null },
                    { (short)22, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2416), null, null, "Belize ", null },
                    { (short)23, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2417), null, null, "Benin ", null },
                    { (short)24, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2417), null, null, "Bermuda ", null },
                    { (short)25, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2418), null, null, "Bhutan ", null },
                    { (short)26, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2419), null, null, "Bolivia ", null },
                    { (short)27, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2419), null, null, "Bosnia and Herzegovina ", null },
                    { (short)28, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2420), null, null, "Botswana ", null },
                    { (short)29, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2421), null, null, "Bouvet Island ", null },
                    { (short)30, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2421), null, null, "Brazil ", null },
                    { (short)31, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2422), null, null, "British Indian Ocean Territory ", null },
                    { (short)32, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2423), null, null, "Brunei Darussalam ", null },
                    { (short)33, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2423), null, null, "Bulgaria ", null },
                    { (short)34, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2433), null, null, "Burkina Faso ", null },
                    { (short)35, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2434), null, null, "Burundi ", null },
                    { (short)36, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2434), null, null, "Cambodia ", null },
                    { (short)37, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2435), null, null, "Cameroon ", null },
                    { (short)38, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2436), null, null, "Canada ", null },
                    { (short)39, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2436), null, null, "Cape Verde ", null },
                    { (short)40, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2437), null, null, "Cayman Islands ", null },
                    { (short)41, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2438), null, null, "Central African Republic ", null },
                    { (short)42, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2438), null, null, "Chad ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)43, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2439), null, null, "Chile ", null },
                    { (short)44, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2440), null, null, "China ", null },
                    { (short)45, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2440), null, null, "Christmas Island ", null },
                    { (short)46, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2441), null, null, "Cocos (Keeling) Islands ", null },
                    { (short)47, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2442), null, null, "Colombia ", null },
                    { (short)48, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2442), null, null, "Comoros ", null },
                    { (short)49, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2443), null, null, "Congo ", null },
                    { (short)50, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2444), null, null, "Congo, The Democratic Republic of The ", null },
                    { (short)51, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2444), null, null, "Cook Islands ", null },
                    { (short)52, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2445), null, null, "Costa Rica ", null },
                    { (short)53, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2446), null, null, "Cote D'ivoire ", null },
                    { (short)54, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2446), null, null, "Croatia ", null },
                    { (short)55, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2447), null, null, "Cuba ", null },
                    { (short)56, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2448), null, null, "Cyprus ", null },
                    { (short)57, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2448), null, null, "Czech Republic ", null },
                    { (short)58, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2449), null, null, "Denmark ", null },
                    { (short)59, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2450), null, null, "Djibouti ", null },
                    { (short)60, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2450), null, null, "Dominica ", null },
                    { (short)61, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2451), null, null, "Dominican Republic ", null },
                    { (short)62, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2452), null, null, "Ecuador ", null },
                    { (short)63, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2453), null, null, "Egypt ", null },
                    { (short)64, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2453), null, null, "El Salvador ", null },
                    { (short)65, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2454), null, null, "Equatorial Guinea ", null },
                    { (short)66, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2455), null, null, "Eritrea ", null },
                    { (short)67, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2457), null, null, "Estonia ", null },
                    { (short)68, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2458), null, null, "Ethiopia ", null },
                    { (short)69, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2458), null, null, "Falkland Islands (Malvinas) ", null },
                    { (short)70, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2459), null, null, "Faroe Islands ", null },
                    { (short)71, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2460), null, null, "Fiji ", null },
                    { (short)72, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2460), null, null, "Finland ", null },
                    { (short)73, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2461), null, null, "France ", null },
                    { (short)74, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2462), null, null, "French Guiana ", null },
                    { (short)75, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2462), null, null, "French Polynesia ", null },
                    { (short)76, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2463), null, null, "French Southern Territories ", null },
                    { (short)77, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2464), null, null, "Gabon ", null },
                    { (short)78, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2465), null, null, "Gambia ", null },
                    { (short)79, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2465), null, null, "Georgia ", null },
                    { (short)80, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2466), null, null, "Germany ", null },
                    { (short)81, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2467), null, null, "Ghana ", null },
                    { (short)82, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2468), null, null, "Gibraltar ", null },
                    { (short)83, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2468), null, null, "Greece ", null },
                    { (short)84, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2469), null, null, "Greenland ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)85, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2469), null, null, "Grenada ", null },
                    { (short)86, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2470), null, null, "Guadeloupe ", null },
                    { (short)87, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2471), null, null, "Guam ", null },
                    { (short)88, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2472), null, null, "Guatemala ", null },
                    { (short)89, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2472), null, null, "Guinea ", null },
                    { (short)90, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2473), null, null, "Guinea-bissau ", null },
                    { (short)91, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2474), null, null, "Guyana ", null },
                    { (short)92, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2474), null, null, "Haiti ", null },
                    { (short)93, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2475), null, null, "Heard Island and Mcdonald Islands ", null },
                    { (short)94, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2480), null, null, "Holy See (Vatican City State) ", null },
                    { (short)95, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2480), null, null, "Honduras ", null },
                    { (short)96, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2481), null, null, "Hong Kong ", null },
                    { (short)97, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2482), null, null, "Hungary ", null },
                    { (short)98, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2482), null, null, "Iceland ", null },
                    { (short)99, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2483), null, null, "India ", null },
                    { (short)100, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2484), null, null, "Indonesia ", null },
                    { (short)101, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2485), null, null, "Iran, Islamic Republic of ", null },
                    { (short)102, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2485), null, null, "Iraq ", null },
                    { (short)103, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2486), null, null, "Ireland ", null },
                    { (short)104, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2487), null, null, "Israel ", null },
                    { (short)105, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2487), null, null, "Italy ", null },
                    { (short)106, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2488), null, null, "Jamaica ", null },
                    { (short)107, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2489), null, null, "Japan ", null },
                    { (short)108, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2489), null, null, "Jordan ", null },
                    { (short)109, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2490), null, null, "Kazakhstan ", null },
                    { (short)110, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2491), null, null, "Kenya ", null },
                    { (short)111, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2491), null, null, "Kiribati ", null },
                    { (short)112, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2492), null, null, "Korea, Democratic People's Republic of ", null },
                    { (short)113, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2493), null, null, "Korea, Republic of ", null },
                    { (short)114, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2494), null, null, "Kuwait ", null },
                    { (short)115, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2495), null, null, "Kyrgyzstan ", null },
                    { (short)116, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2495), null, null, "Lao People's Democratic Republic ", null },
                    { (short)117, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2496), null, null, "Latvia ", null },
                    { (short)118, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2497), null, null, "Lebanon ", null },
                    { (short)119, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2497), null, null, "Lesotho ", null },
                    { (short)120, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2498), null, null, "Liberia ", null },
                    { (short)121, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2499), null, null, "Libyan Arab Jamahiriya ", null },
                    { (short)122, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2499), null, null, "Liechtenstein ", null },
                    { (short)123, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2500), null, null, "Lithuania ", null },
                    { (short)124, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2501), null, null, "Luxembourg ", null },
                    { (short)125, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2502), null, null, "Macao ", null },
                    { (short)126, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2502), null, null, "North Macedonia ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)127, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2503), null, null, "Madagascar ", null },
                    { (short)128, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2504), null, null, "Malawi ", null },
                    { (short)129, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2504), null, null, "Malaysia ", null },
                    { (short)130, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2506), null, null, "Maldives ", null },
                    { (short)131, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2506), null, null, "Mali ", null },
                    { (short)132, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2507), null, null, "Malta ", null },
                    { (short)133, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2508), null, null, "Marshall Islands ", null },
                    { (short)134, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2509), null, null, "Martinique ", null },
                    { (short)135, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2509), null, null, "Mauritania ", null },
                    { (short)136, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2510), null, null, "Mauritius ", null },
                    { (short)137, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2511), null, null, "Mayotte ", null },
                    { (short)138, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2511), null, null, "Mexico ", null },
                    { (short)139, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2512), null, null, "Micronesia, Federated States of ", null },
                    { (short)140, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2513), null, null, "Moldova, Republic of ", null },
                    { (short)141, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2513), null, null, "Monaco ", null },
                    { (short)142, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2514), null, null, "Mongolia ", null },
                    { (short)143, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2515), null, null, "Montserrat ", null },
                    { (short)144, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2516), null, null, "Morocco ", null },
                    { (short)145, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2521), null, null, "Mozambique ", null },
                    { (short)146, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2522), null, null, "Myanmar ", null },
                    { (short)147, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2522), null, null, "Namibia ", null },
                    { (short)148, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2523), null, null, "Nauru ", null },
                    { (short)149, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2524), null, null, "Nepal ", null },
                    { (short)150, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2524), null, null, "Netherlands ", null },
                    { (short)151, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2525), null, null, "Netherlands Antilles ", null },
                    { (short)152, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2526), null, null, "New Caledonia ", null },
                    { (short)153, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2526), null, null, "New Zealand ", null },
                    { (short)154, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2527), null, null, "Nicaragua ", null },
                    { (short)155, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2527), null, null, "Niger ", null },
                    { (short)156, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2528), null, null, "Nigeria ", null },
                    { (short)157, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2529), null, null, "Niue ", null },
                    { (short)158, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2530), null, null, "Norfolk Island ", null },
                    { (short)159, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2530), null, null, "Northern Mariana Islands ", null },
                    { (short)160, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2531), null, null, "Norway ", null },
                    { (short)161, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2532), null, null, "Oman ", null },
                    { (short)162, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2533), null, null, "Pakistan ", null },
                    { (short)163, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2534), null, null, "Palau ", null },
                    { (short)164, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2535), null, null, "Palestinian Territory, Occupied ", null },
                    { (short)165, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2535), null, null, "Panama ", null },
                    { (short)166, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2536), null, null, "Papua New Guinea ", null },
                    { (short)167, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2537), null, null, "Paraguay ", null },
                    { (short)168, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2537), null, null, "Peru ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)169, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2538), null, null, "Philippines ", null },
                    { (short)170, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2539), null, null, "Pitcairn ", null },
                    { (short)171, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2539), null, null, "Poland ", null },
                    { (short)172, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2540), null, null, "Portugal ", null },
                    { (short)173, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2541), null, null, "Puerto Rico ", null },
                    { (short)174, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2542), null, null, "Qatar ", null },
                    { (short)175, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2542), null, null, "Reunion ", null },
                    { (short)176, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2543), null, null, "Romania ", null },
                    { (short)177, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2544), null, null, "Russian Federation ", null },
                    { (short)178, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2544), null, null, "Rwanda ", null },
                    { (short)179, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2545), null, null, "Saint Helena ", null },
                    { (short)180, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2546), null, null, "Saint Kitts and Nevis ", null },
                    { (short)181, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2546), null, null, "Saint Lucia ", null },
                    { (short)182, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2547), null, null, "Saint Pierre and Miquelon ", null },
                    { (short)183, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2548), null, null, "Saint Vincent and The Grenadines ", null },
                    { (short)184, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2548), null, null, "Samoa ", null },
                    { (short)185, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2549), null, null, "San Marino ", null },
                    { (short)186, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2550), null, null, "Sao Tome and Principe ", null },
                    { (short)187, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2550), null, null, "Saudi Arabia ", null },
                    { (short)188, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2551), null, null, "Senegal ", null },
                    { (short)189, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2552), null, null, "Serbia and Montenegro ", null },
                    { (short)190, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2552), null, null, "Seychelles ", null },
                    { (short)191, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2553), null, null, "Sierra Leone ", null },
                    { (short)192, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2554), null, null, "Singapore ", null },
                    { (short)193, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2554), null, null, "Slovakia ", null },
                    { (short)194, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2555), null, null, "Slovenia ", null },
                    { (short)195, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2556), null, null, "Solomon Islands ", null },
                    { (short)196, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2556), null, null, "Somalia ", null },
                    { (short)197, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2557), null, null, "South Africa ", null },
                    { (short)198, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2558), null, null, "South Georgia and The South Sandwich Islands ", null },
                    { (short)199, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2558), null, null, "Spain ", null },
                    { (short)200, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2559), null, null, "Sri Lanka ", null },
                    { (short)201, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2560), null, null, "Sudan ", null },
                    { (short)202, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2560), null, null, "Suriname ", null },
                    { (short)203, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2561), null, null, "Svalbard and Jan Mayen ", null },
                    { (short)204, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2562), null, null, "Swaziland ", null },
                    { (short)205, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2562), null, null, "Sweden ", null },
                    { (short)206, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2563), null, null, "Switzerland ", null },
                    { (short)207, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2564), null, null, "Syrian Arab Republic ", null },
                    { (short)208, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2564), null, null, "Taiwan, Province of China ", null },
                    { (short)209, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2565), null, null, "Tajikistan ", null },
                    { (short)210, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2566), null, null, "Tanzania, United Republic of ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)211, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2566), null, null, "Thailand ", null },
                    { (short)212, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2567), null, null, "Timor-leste ", null },
                    { (short)213, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2572), null, null, "Togo ", null },
                    { (short)214, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2572), null, null, "Tokelau ", null },
                    { (short)215, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2573), null, null, "Tonga ", null },
                    { (short)216, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2574), null, null, "Trinidad and Tobago ", null },
                    { (short)217, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2574), null, null, "Tunisia ", null },
                    { (short)218, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2575), null, null, "Turkey ", null },
                    { (short)219, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2576), null, null, "Turkmenistan ", null },
                    { (short)220, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2576), null, null, "Turks and Caicos Islands ", null },
                    { (short)221, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2577), null, null, "Tuvalu ", null },
                    { (short)222, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2577), null, null, "Uganda ", null },
                    { (short)223, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2578), null, null, "Ukraine ", null },
                    { (short)224, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2579), null, null, "United Arab Emirates ", null },
                    { (short)225, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2579), null, null, "United Kingdom ", null },
                    { (short)226, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2580), null, null, "United States ", null },
                    { (short)227, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2581), null, null, "United States Minor Outlying Islands ", null },
                    { (short)228, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2581), null, null, "Uruguay ", null },
                    { (short)229, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2582), null, null, "Uzbekistan ", null },
                    { (short)230, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2583), null, null, "Vanuatu ", null },
                    { (short)231, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2583), null, null, "Venezuela ", null },
                    { (short)232, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2584), null, null, "Viet Nam ", null },
                    { (short)233, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2585), null, null, "Virgin Islands, British ", null },
                    { (short)234, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2585), null, null, "Virgin Islands, U.S. ", null },
                    { (short)235, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2586), null, null, "Wallis and Futuna ", null },
                    { (short)236, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2587), null, null, "Western Sahara ", null },
                    { (short)237, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2587), null, null, "Yemen ", null },
                    { (short)238, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2588), null, null, "Zambia ", null },
                    { (short)239, "", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(2589), null, null, "Zimbabwe", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Discriminator", "ModifiedBy", "ModifiedOn", "Name", "Rights" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "System", new DateTime(2022, 8, 23, 15, 27, 44, 707, DateTimeKind.Utc).AddTicks(3056), "SystemRole", null, null, "Admin", 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CountryId", "CreatedBy", "CreatedOn", "Identifier", "ImageType", "ModifiedBy", "ModifiedOn", "UserId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), (short)21, "System", new DateTime(2022, 8, 23, 15, 27, 44, 704, DateTimeKind.Utc).AddTicks(3200), "00uid4BxXw6I6TV4m0g3", 0, null, null, null });

            migrationBuilder.InsertData(
                table: "RoleUser",
                columns: new[] { "RolesId", "UsersId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationLinkProduct_RelatedProductsId",
                table: "ApplicationLinkProduct",
                column: "RelatedProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationLinks_ApplicationId",
                table: "ApplicationLinks",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantId",
                table: "Applications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CountryId",
                table: "Applications",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_MvpTypeId",
                table: "Applications",
                column: "MvpTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SelectionId",
                table: "Applications",
                column: "SelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UserId",
                table: "Consents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_RegionId",
                table: "Countries",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLinks_UserId",
                table: "ProfileLinks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCategoryScore_ScoreCategoryId",
                table: "ReviewCategoryScore",
                column: "ScoreCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCategoryScore_ScoreId",
                table: "ReviewCategoryScore",
                column: "ScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ApplicationId",
                table: "Reviews",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ApplicationId",
                table: "Roles",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CountryId",
                table: "Roles",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_MvpTypeId",
                table: "Roles",
                column: "MvpTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RegionId",
                table: "Roles",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_SelectionId",
                table: "Roles",
                column: "SelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategories_MvpTypeId",
                table: "ScoreCategories",
                column: "MvpTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategories_SelectionId",
                table: "ScoreCategories",
                column: "SelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_ScoreCategoryId",
                table: "Scores",
                column: "ScoreCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Title_ApplicationId",
                table: "Title",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Title_MvpTypeId",
                table: "Title",
                column: "MvpTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Title_SelectionId",
                table: "Title",
                column: "SelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Title_UserId",
                table: "Title",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryId",
                table: "Users",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationLinkProduct");

            migrationBuilder.DropTable(
                name: "Consents");

            migrationBuilder.DropTable(
                name: "ProfileLinks");

            migrationBuilder.DropTable(
                name: "ReviewCategoryScore");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "Title");

            migrationBuilder.DropTable(
                name: "ApplicationLinks");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ScoreCategories");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "MvpType");

            migrationBuilder.DropTable(
                name: "Selections");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
