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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    ReviewsEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    RegionId = table.Column<int>(type: "int", nullable: true)
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
                    SelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    ScoreCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    Status = table.Column<int>(type: "int", nullable: false)
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
                    Type = table.Column<int>(type: "int", nullable: false)
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
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Rights = table.Column<int>(type: "int", nullable: true)
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
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)1, "Afghanistan ", null },
                    { (short)2, "Albania ", null },
                    { (short)3, "Algeria ", null },
                    { (short)4, "American Samoa ", null },
                    { (short)5, "Andorra ", null },
                    { (short)6, "Angola ", null },
                    { (short)7, "Anguilla ", null },
                    { (short)8, "Antarctica ", null },
                    { (short)9, "Antigua and Barbuda ", null },
                    { (short)10, "Argentina ", null },
                    { (short)11, "Armenia ", null },
                    { (short)12, "Aruba ", null },
                    { (short)13, "Australia ", null },
                    { (short)14, "Austria ", null },
                    { (short)15, "Azerbaijan ", null },
                    { (short)16, "Bahamas ", null },
                    { (short)17, "Bahrain ", null },
                    { (short)18, "Bangladesh ", null },
                    { (short)19, "Barbados ", null },
                    { (short)20, "Belarus ", null },
                    { (short)21, "Belgium ", null },
                    { (short)22, "Belize ", null },
                    { (short)23, "Benin ", null },
                    { (short)24, "Bermuda ", null },
                    { (short)25, "Bhutan ", null },
                    { (short)26, "Bolivia ", null },
                    { (short)27, "Bosnia and Herzegovina ", null },
                    { (short)28, "Botswana ", null },
                    { (short)29, "Bouvet Island ", null },
                    { (short)30, "Brazil ", null },
                    { (short)31, "British Indian Ocean Territory ", null },
                    { (short)32, "Brunei Darussalam ", null },
                    { (short)33, "Bulgaria ", null },
                    { (short)34, "Burkina Faso ", null },
                    { (short)35, "Burundi ", null },
                    { (short)36, "Cambodia ", null },
                    { (short)37, "Cameroon ", null },
                    { (short)38, "Canada ", null },
                    { (short)39, "Cape Verde ", null },
                    { (short)40, "Cayman Islands ", null },
                    { (short)41, "Central African Republic ", null },
                    { (short)42, "Chad ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)43, "Chile ", null },
                    { (short)44, "China ", null },
                    { (short)45, "Christmas Island ", null },
                    { (short)46, "Cocos (Keeling) Islands ", null },
                    { (short)47, "Colombia ", null },
                    { (short)48, "Comoros ", null },
                    { (short)49, "Congo ", null },
                    { (short)50, "Congo, The Democratic Republic of The ", null },
                    { (short)51, "Cook Islands ", null },
                    { (short)52, "Costa Rica ", null },
                    { (short)53, "Cote D'ivoire ", null },
                    { (short)54, "Croatia ", null },
                    { (short)55, "Cuba ", null },
                    { (short)56, "Cyprus ", null },
                    { (short)57, "Czech Republic ", null },
                    { (short)58, "Denmark ", null },
                    { (short)59, "Djibouti ", null },
                    { (short)60, "Dominica ", null },
                    { (short)61, "Dominican Republic ", null },
                    { (short)62, "Ecuador ", null },
                    { (short)63, "Egypt ", null },
                    { (short)64, "El Salvador ", null },
                    { (short)65, "Equatorial Guinea ", null },
                    { (short)66, "Eritrea ", null },
                    { (short)67, "Estonia ", null },
                    { (short)68, "Ethiopia ", null },
                    { (short)69, "Falkland Islands (Malvinas) ", null },
                    { (short)70, "Faroe Islands ", null },
                    { (short)71, "Fiji ", null },
                    { (short)72, "Finland ", null },
                    { (short)73, "France ", null },
                    { (short)74, "French Guiana ", null },
                    { (short)75, "French Polynesia ", null },
                    { (short)76, "French Southern Territories ", null },
                    { (short)77, "Gabon ", null },
                    { (short)78, "Gambia ", null },
                    { (short)79, "Georgia ", null },
                    { (short)80, "Germany ", null },
                    { (short)81, "Ghana ", null },
                    { (short)82, "Gibraltar ", null },
                    { (short)83, "Greece ", null },
                    { (short)84, "Greenland ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)85, "Grenada ", null },
                    { (short)86, "Guadeloupe ", null },
                    { (short)87, "Guam ", null },
                    { (short)88, "Guatemala ", null },
                    { (short)89, "Guinea ", null },
                    { (short)90, "Guinea-bissau ", null },
                    { (short)91, "Guyana ", null },
                    { (short)92, "Haiti ", null },
                    { (short)93, "Heard Island and Mcdonald Islands ", null },
                    { (short)94, "Holy See (Vatican City State) ", null },
                    { (short)95, "Honduras ", null },
                    { (short)96, "Hong Kong ", null },
                    { (short)97, "Hungary ", null },
                    { (short)98, "Iceland ", null },
                    { (short)99, "India ", null },
                    { (short)100, "Indonesia ", null },
                    { (short)101, "Iran, Islamic Republic of ", null },
                    { (short)102, "Iraq ", null },
                    { (short)103, "Ireland ", null },
                    { (short)104, "Israel ", null },
                    { (short)105, "Italy ", null },
                    { (short)106, "Jamaica ", null },
                    { (short)107, "Japan ", null },
                    { (short)108, "Jordan ", null },
                    { (short)109, "Kazakhstan ", null },
                    { (short)110, "Kenya ", null },
                    { (short)111, "Kiribati ", null },
                    { (short)112, "Korea, Democratic People's Republic of ", null },
                    { (short)113, "Korea, Republic of ", null },
                    { (short)114, "Kuwait ", null },
                    { (short)115, "Kyrgyzstan ", null },
                    { (short)116, "Lao People's Democratic Republic ", null },
                    { (short)117, "Latvia ", null },
                    { (short)118, "Lebanon ", null },
                    { (short)119, "Lesotho ", null },
                    { (short)120, "Liberia ", null },
                    { (short)121, "Libyan Arab Jamahiriya ", null },
                    { (short)122, "Liechtenstein ", null },
                    { (short)123, "Lithuania ", null },
                    { (short)124, "Luxembourg ", null },
                    { (short)125, "Macao ", null },
                    { (short)126, "North Macedonia ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)127, "Madagascar ", null },
                    { (short)128, "Malawi ", null },
                    { (short)129, "Malaysia ", null },
                    { (short)130, "Maldives ", null },
                    { (short)131, "Mali ", null },
                    { (short)132, "Malta ", null },
                    { (short)133, "Marshall Islands ", null },
                    { (short)134, "Martinique ", null },
                    { (short)135, "Mauritania ", null },
                    { (short)136, "Mauritius ", null },
                    { (short)137, "Mayotte ", null },
                    { (short)138, "Mexico ", null },
                    { (short)139, "Micronesia, Federated States of ", null },
                    { (short)140, "Moldova, Republic of ", null },
                    { (short)141, "Monaco ", null },
                    { (short)142, "Mongolia ", null },
                    { (short)143, "Montserrat ", null },
                    { (short)144, "Morocco ", null },
                    { (short)145, "Mozambique ", null },
                    { (short)146, "Myanmar ", null },
                    { (short)147, "Namibia ", null },
                    { (short)148, "Nauru ", null },
                    { (short)149, "Nepal ", null },
                    { (short)150, "Netherlands ", null },
                    { (short)151, "Netherlands Antilles ", null },
                    { (short)152, "New Caledonia ", null },
                    { (short)153, "New Zealand ", null },
                    { (short)154, "Nicaragua ", null },
                    { (short)155, "Niger ", null },
                    { (short)156, "Nigeria ", null },
                    { (short)157, "Niue ", null },
                    { (short)158, "Norfolk Island ", null },
                    { (short)159, "Northern Mariana Islands ", null },
                    { (short)160, "Norway ", null },
                    { (short)161, "Oman ", null },
                    { (short)162, "Pakistan ", null },
                    { (short)163, "Palau ", null },
                    { (short)164, "Palestinian Territory, Occupied ", null },
                    { (short)165, "Panama ", null },
                    { (short)166, "Papua New Guinea ", null },
                    { (short)167, "Paraguay ", null },
                    { (short)168, "Peru ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)169, "Philippines ", null },
                    { (short)170, "Pitcairn ", null },
                    { (short)171, "Poland ", null },
                    { (short)172, "Portugal ", null },
                    { (short)173, "Puerto Rico ", null },
                    { (short)174, "Qatar ", null },
                    { (short)175, "Reunion ", null },
                    { (short)176, "Romania ", null },
                    { (short)177, "Russian Federation ", null },
                    { (short)178, "Rwanda ", null },
                    { (short)179, "Saint Helena ", null },
                    { (short)180, "Saint Kitts and Nevis ", null },
                    { (short)181, "Saint Lucia ", null },
                    { (short)182, "Saint Pierre and Miquelon ", null },
                    { (short)183, "Saint Vincent and The Grenadines ", null },
                    { (short)184, "Samoa ", null },
                    { (short)185, "San Marino ", null },
                    { (short)186, "Sao Tome and Principe ", null },
                    { (short)187, "Saudi Arabia ", null },
                    { (short)188, "Senegal ", null },
                    { (short)189, "Serbia and Montenegro ", null },
                    { (short)190, "Seychelles ", null },
                    { (short)191, "Sierra Leone ", null },
                    { (short)192, "Singapore ", null },
                    { (short)193, "Slovakia ", null },
                    { (short)194, "Slovenia ", null },
                    { (short)195, "Solomon Islands ", null },
                    { (short)196, "Somalia ", null },
                    { (short)197, "South Africa ", null },
                    { (short)198, "South Georgia and The South Sandwich Islands ", null },
                    { (short)199, "Spain ", null },
                    { (short)200, "Sri Lanka ", null },
                    { (short)201, "Sudan ", null },
                    { (short)202, "Suriname ", null },
                    { (short)203, "Svalbard and Jan Mayen ", null },
                    { (short)204, "Swaziland ", null },
                    { (short)205, "Sweden ", null },
                    { (short)206, "Switzerland ", null },
                    { (short)207, "Syrian Arab Republic ", null },
                    { (short)208, "Taiwan, Province of China ", null },
                    { (short)209, "Tajikistan ", null },
                    { (short)210, "Tanzania, United Republic of ", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { (short)211, "Thailand ", null },
                    { (short)212, "Timor-leste ", null },
                    { (short)213, "Togo ", null },
                    { (short)214, "Tokelau ", null },
                    { (short)215, "Tonga ", null },
                    { (short)216, "Trinidad and Tobago ", null },
                    { (short)217, "Tunisia ", null },
                    { (short)218, "Turkey ", null },
                    { (short)219, "Turkmenistan ", null },
                    { (short)220, "Turks and Caicos Islands ", null },
                    { (short)221, "Tuvalu ", null },
                    { (short)222, "Uganda ", null },
                    { (short)223, "Ukraine ", null },
                    { (short)224, "United Arab Emirates ", null },
                    { (short)225, "United Kingdom ", null },
                    { (short)226, "United States ", null },
                    { (short)227, "United States Minor Outlying Islands ", null },
                    { (short)228, "Uruguay ", null },
                    { (short)229, "Uzbekistan ", null },
                    { (short)230, "Vanuatu ", null },
                    { (short)231, "Venezuela ", null },
                    { (short)232, "Viet Nam ", null },
                    { (short)233, "Virgin Islands, British ", null },
                    { (short)234, "Virgin Islands, U.S. ", null },
                    { (short)235, "Wallis and Futuna ", null },
                    { (short)236, "Western Sahara ", null },
                    { (short)237, "Yemen ", null },
                    { (short)238, "Zambia ", null },
                    { (short)239, "Zimbabwe", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Discriminator", "Name", "Rights" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "SystemRole", "Admin", 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CountryId", "Identifier", "ImageType", "UserId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), (short)21, "00uid4BxXw6I6TV4m0g3", 0, null });

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
