using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data
{
    public class Context : DbContext
    {
        public Context()
        {
        }

        public Context(DbContextOptions<Context> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<ProfileLink> ProfileLinks => Set<ProfileLink>();

        public DbSet<Consent> Consents => Set<Consent>();

        public DbSet<Selection> Selections => Set<Selection>();

        public DbSet<Application> Applications => Set<Application>();

        public DbSet<ApplicationLink> ApplicationLinks => Set<ApplicationLink>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<Region> Regions => Set<Region>();

        public DbSet<Country> Countries => Set<Country>();

        public DbSet<ScoreCategory> ScoreCategories => Set<ScoreCategory>();

        public DbSet<Score> Scores => Set<Score>();

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Guid adminSystemRoleId = new ("00000000-0000-0000-0000-000000000001");
            Guid adminUserId = new ("00000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Country>()
                .HasData(SeedCountries());

            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Identifier);
            modelBuilder.Entity<User>()
                .HasData(new { Id = adminUserId, Identifier = "00uid4BxXw6I6TV4m0g3", CountryId = (short)21, ImageType = ImageType.Anonymous });

            modelBuilder.Entity("RoleUser")
                .HasData(new { UsersId = adminUserId, RolesId = adminSystemRoleId });

            modelBuilder.Entity<SelectionRole>();

            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = adminSystemRoleId, Name = "Admin", Rights = Right.Admin | Right.Any });

            modelBuilder.Entity<ReviewCategoryScore>()
                .HasKey(rcs => new { rcs.ReviewId, rcs.ScoreCategoryId, rcs.ScoreId });
            modelBuilder.Entity<ReviewCategoryScore>()
                .HasOne(rcs => rcs.Review)
                .WithMany(r => r.CategoryScores)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Applicant)
                .WithMany(u => u.Applications)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.Reviews)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Title>()
                .HasOne(t => t.MvpType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Title>()
                .HasOne(t => t.Selection)
                .WithMany(s => s.Titles)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Title>()
                .HasOne(t => t.User)
                .WithMany(u => u.Titles)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Selection>()
                .HasAlternateKey(s => s.Year);
        }

#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // NOTE [ILs] This is used to make it easy to run dotnet ef core cli commands
                optionsBuilder.UseSqlServer(
                        "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=C:\\Code\\Mvp.Selections\\data\\Temp.mdf");
            }
        }
#endif

        private IEnumerable<Country> SeedCountries()
        {
            return new List<Country>
            {
                new (1) { Name = "Afghanistan " },
                new (2) { Name = "Albania " },
                new (3) { Name = "Algeria " },
                new (4) { Name = "American Samoa " },
                new (5) { Name = "Andorra " },
                new (6) { Name = "Angola " },
                new (7) { Name = "Anguilla " },
                new (8) { Name = "Antarctica " },
                new (9) { Name = "Antigua and Barbuda " },
                new (10) { Name = "Argentina " },
                new (11) { Name = "Armenia " },
                new (12) { Name = "Aruba " },
                new (13) { Name = "Australia " },
                new (14) { Name = "Austria " },
                new (15) { Name = "Azerbaijan " },
                new (16) { Name = "Bahamas " },
                new (17) { Name = "Bahrain " },
                new (18) { Name = "Bangladesh " },
                new (19) { Name = "Barbados " },
                new (20) { Name = "Belarus " },
                new (21) { Name = "Belgium " },
                new (22) { Name = "Belize " },
                new (23) { Name = "Benin " },
                new (24) { Name = "Bermuda " },
                new (25) { Name = "Bhutan " },
                new (26) { Name = "Bolivia " },
                new (27) { Name = "Bosnia and Herzegovina " },
                new (28) { Name = "Botswana " },
                new (29) { Name = "Bouvet Island " },
                new (30) { Name = "Brazil " },
                new (31) { Name = "British Indian Ocean Territory " },
                new (32) { Name = "Brunei Darussalam " },
                new (33) { Name = "Bulgaria " },
                new (34) { Name = "Burkina Faso " },
                new (35) { Name = "Burundi " },
                new (36) { Name = "Cambodia " },
                new (37) { Name = "Cameroon " },
                new (38) { Name = "Canada " },
                new (39) { Name = "Cape Verde " },
                new (40) { Name = "Cayman Islands " },
                new (41) { Name = "Central African Republic " },
                new (42) { Name = "Chad " },
                new (43) { Name = "Chile " },
                new (44) { Name = "China " },
                new (45) { Name = "Christmas Island " },
                new (46) { Name = "Cocos (Keeling) Islands " },
                new (47) { Name = "Colombia " },
                new (48) { Name = "Comoros " },
                new (49) { Name = "Congo " },
                new (50) { Name = "Congo, The Democratic Republic of The " },
                new (51) { Name = "Cook Islands " },
                new (52) { Name = "Costa Rica " },
                new (53) { Name = "Cote D'ivoire " },
                new (54) { Name = "Croatia " },
                new (55) { Name = "Cuba " },
                new (56) { Name = "Cyprus " },
                new (57) { Name = "Czech Republic " },
                new (58) { Name = "Denmark " },
                new (59) { Name = "Djibouti " },
                new (60) { Name = "Dominica " },
                new (61) { Name = "Dominican Republic " },
                new (62) { Name = "Ecuador " },
                new (63) { Name = "Egypt " },
                new (64) { Name = "El Salvador " },
                new (65) { Name = "Equatorial Guinea " },
                new (66) { Name = "Eritrea " },
                new (67) { Name = "Estonia " },
                new (68) { Name = "Ethiopia " },
                new (69) { Name = "Falkland Islands (Malvinas) " },
                new (70) { Name = "Faroe Islands " },
                new (71) { Name = "Fiji " },
                new (72) { Name = "Finland " },
                new (73) { Name = "France " },
                new (74) { Name = "French Guiana " },
                new (75) { Name = "French Polynesia " },
                new (76) { Name = "French Southern Territories " },
                new (77) { Name = "Gabon " },
                new (78) { Name = "Gambia " },
                new (79) { Name = "Georgia " },
                new (80) { Name = "Germany " },
                new (81) { Name = "Ghana " },
                new (82) { Name = "Gibraltar " },
                new (83) { Name = "Greece " },
                new (84) { Name = "Greenland " },
                new (85) { Name = "Grenada " },
                new (86) { Name = "Guadeloupe " },
                new (87) { Name = "Guam " },
                new (88) { Name = "Guatemala " },
                new (89) { Name = "Guinea " },
                new (90) { Name = "Guinea-bissau " },
                new (91) { Name = "Guyana " },
                new (92) { Name = "Haiti " },
                new (93) { Name = "Heard Island and Mcdonald Islands " },
                new (94) { Name = "Holy See (Vatican City State) " },
                new (95) { Name = "Honduras " },
                new (96) { Name = "Hong Kong " },
                new (97) { Name = "Hungary " },
                new (98) { Name = "Iceland " },
                new (99) { Name = "India " },
                new (100) { Name = "Indonesia " },
                new (101) { Name = "Iran, Islamic Republic of " },
                new (102) { Name = "Iraq " },
                new (103) { Name = "Ireland " },
                new (104) { Name = "Israel " },
                new (105) { Name = "Italy " },
                new (106) { Name = "Jamaica " },
                new (107) { Name = "Japan " },
                new (108) { Name = "Jordan " },
                new (109) { Name = "Kazakhstan " },
                new (110) { Name = "Kenya " },
                new (111) { Name = "Kiribati " },
                new (112) { Name = "Korea, Democratic People's Republic of " },
                new (113) { Name = "Korea, Republic of " },
                new (114) { Name = "Kuwait " },
                new (115) { Name = "Kyrgyzstan " },
                new (116) { Name = "Lao People's Democratic Republic " },
                new (117) { Name = "Latvia " },
                new (118) { Name = "Lebanon " },
                new (119) { Name = "Lesotho " },
                new (120) { Name = "Liberia " },
                new (121) { Name = "Libyan Arab Jamahiriya " },
                new (122) { Name = "Liechtenstein " },
                new (123) { Name = "Lithuania " },
                new (124) { Name = "Luxembourg " },
                new (125) { Name = "Macao " },
                new (126) { Name = "North Macedonia " },
                new (127) { Name = "Madagascar " },
                new (128) { Name = "Malawi " },
                new (129) { Name = "Malaysia " },
                new (130) { Name = "Maldives " },
                new (131) { Name = "Mali " },
                new (132) { Name = "Malta " },
                new (133) { Name = "Marshall Islands " },
                new (134) { Name = "Martinique " },
                new (135) { Name = "Mauritania " },
                new (136) { Name = "Mauritius " },
                new (137) { Name = "Mayotte " },
                new (138) { Name = "Mexico " },
                new (139) { Name = "Micronesia, Federated States of " },
                new (140) { Name = "Moldova, Republic of " },
                new (141) { Name = "Monaco " },
                new (142) { Name = "Mongolia " },
                new (143) { Name = "Montserrat " },
                new (144) { Name = "Morocco " },
                new (145) { Name = "Mozambique " },
                new (146) { Name = "Myanmar " },
                new (147) { Name = "Namibia " },
                new (148) { Name = "Nauru " },
                new (149) { Name = "Nepal " },
                new (150) { Name = "Netherlands " },
                new (151) { Name = "Netherlands Antilles " },
                new (152) { Name = "New Caledonia " },
                new (153) { Name = "New Zealand " },
                new (154) { Name = "Nicaragua " },
                new (155) { Name = "Niger " },
                new (156) { Name = "Nigeria " },
                new (157) { Name = "Niue " },
                new (158) { Name = "Norfolk Island " },
                new (159) { Name = "Northern Mariana Islands " },
                new (160) { Name = "Norway " },
                new (161) { Name = "Oman " },
                new (162) { Name = "Pakistan " },
                new (163) { Name = "Palau " },
                new (164) { Name = "Palestinian Territory, Occupied " },
                new (165) { Name = "Panama " },
                new (166) { Name = "Papua New Guinea " },
                new (167) { Name = "Paraguay " },
                new (168) { Name = "Peru " },
                new (169) { Name = "Philippines " },
                new (170) { Name = "Pitcairn " },
                new (171) { Name = "Poland " },
                new (172) { Name = "Portugal " },
                new (173) { Name = "Puerto Rico " },
                new (174) { Name = "Qatar " },
                new (175) { Name = "Reunion " },
                new (176) { Name = "Romania " },
                new (177) { Name = "Russian Federation " },
                new (178) { Name = "Rwanda " },
                new (179) { Name = "Saint Helena " },
                new (180) { Name = "Saint Kitts and Nevis " },
                new (181) { Name = "Saint Lucia " },
                new (182) { Name = "Saint Pierre and Miquelon " },
                new (183) { Name = "Saint Vincent and The Grenadines " },
                new (184) { Name = "Samoa " },
                new (185) { Name = "San Marino " },
                new (186) { Name = "Sao Tome and Principe " },
                new (187) { Name = "Saudi Arabia " },
                new (188) { Name = "Senegal " },
                new (189) { Name = "Serbia and Montenegro " },
                new (190) { Name = "Seychelles " },
                new (191) { Name = "Sierra Leone " },
                new (192) { Name = "Singapore " },
                new (193) { Name = "Slovakia " },
                new (194) { Name = "Slovenia " },
                new (195) { Name = "Solomon Islands " },
                new (196) { Name = "Somalia " },
                new (197) { Name = "South Africa " },
                new (198) { Name = "South Georgia and The South Sandwich Islands " },
                new (199) { Name = "Spain " },
                new (200) { Name = "Sri Lanka " },
                new (201) { Name = "Sudan " },
                new (202) { Name = "Suriname " },
                new (203) { Name = "Svalbard and Jan Mayen " },
                new (204) { Name = "Swaziland " },
                new (205) { Name = "Sweden " },
                new (206) { Name = "Switzerland " },
                new (207) { Name = "Syrian Arab Republic " },
                new (208) { Name = "Taiwan, Province of China " },
                new (209) { Name = "Tajikistan " },
                new (210) { Name = "Tanzania, United Republic of " },
                new (211) { Name = "Thailand " },
                new (212) { Name = "Timor-leste " },
                new (213) { Name = "Togo " },
                new (214) { Name = "Tokelau " },
                new (215) { Name = "Tonga " },
                new (216) { Name = "Trinidad and Tobago " },
                new (217) { Name = "Tunisia " },
                new (218) { Name = "Turkey " },
                new (219) { Name = "Turkmenistan " },
                new (220) { Name = "Turks and Caicos Islands " },
                new (221) { Name = "Tuvalu " },
                new (222) { Name = "Uganda " },
                new (223) { Name = "Ukraine " },
                new (224) { Name = "United Arab Emirates " },
                new (225) { Name = "United Kingdom " },
                new (226) { Name = "United States " },
                new (227) { Name = "United States Minor Outlying Islands " },
                new (228) { Name = "Uruguay " },
                new (229) { Name = "Uzbekistan " },
                new (230) { Name = "Vanuatu " },
                new (231) { Name = "Venezuela " },
                new (232) { Name = "Viet Nam " },
                new (233) { Name = "Virgin Islands, British " },
                new (234) { Name = "Virgin Islands, U.S. " },
                new (235) { Name = "Wallis and Futuna " },
                new (236) { Name = "Western Sahara " },
                new (237) { Name = "Yemen " },
                new (238) { Name = "Zambia " },
                new (239) { Name = "Zimbabwe" }
            };
        }
    }
}
