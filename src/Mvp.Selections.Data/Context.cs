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
                .HasData(new { Id = adminUserId, Identifier = "00uid4BxXw6I6TV4m0g3", CountryId = (short)21, ImageType = ImageType.Anonymous });
            modelBuilder.Entity<User>()
                .Navigation(u => u.Consents)
                .AutoInclude();
            modelBuilder.Entity<User>()
                .Navigation(u => u.Country)
                .AutoInclude();
            modelBuilder.Entity<User>()
                .Navigation(u => u.Roles)
                .AutoInclude();
            modelBuilder.Entity<User>()
                .Navigation(u => u.Titles)
                .AutoInclude();

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
        }

#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                        "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=C:\\Code\\Mvp.Selections\\data\\Temp.mdf");
            }
        }
#endif

        private IEnumerable<Country> SeedCountries()
        {
            return new List<Country>
            {
                new () { Id = 1, Name = "Afghanistan " },
                new () { Id = 2, Name = "Albania " },
                new () { Id = 3, Name = "Algeria " },
                new () { Id = 4, Name = "American Samoa " },
                new () { Id = 5, Name = "Andorra " },
                new () { Id = 6, Name = "Angola " },
                new () { Id = 7, Name = "Anguilla " },
                new () { Id = 8, Name = "Antarctica " },
                new () { Id = 9, Name = "Antigua and Barbuda " },
                new () { Id = 10, Name = "Argentina " },
                new () { Id = 11, Name = "Armenia " },
                new () { Id = 12, Name = "Aruba " },
                new () { Id = 13, Name = "Australia " },
                new () { Id = 14, Name = "Austria " },
                new () { Id = 15, Name = "Azerbaijan " },
                new () { Id = 16, Name = "Bahamas " },
                new () { Id = 17, Name = "Bahrain " },
                new () { Id = 18, Name = "Bangladesh " },
                new () { Id = 19, Name = "Barbados " },
                new () { Id = 20, Name = "Belarus " },
                new () { Id = 21, Name = "Belgium " },
                new () { Id = 22, Name = "Belize " },
                new () { Id = 23, Name = "Benin " },
                new () { Id = 24, Name = "Bermuda " },
                new () { Id = 25, Name = "Bhutan " },
                new () { Id = 26, Name = "Bolivia " },
                new () { Id = 27, Name = "Bosnia and Herzegovina " },
                new () { Id = 28, Name = "Botswana " },
                new () { Id = 29, Name = "Bouvet Island " },
                new () { Id = 30, Name = "Brazil " },
                new () { Id = 31, Name = "British Indian Ocean Territory " },
                new () { Id = 32, Name = "Brunei Darussalam " },
                new () { Id = 33, Name = "Bulgaria " },
                new () { Id = 34, Name = "Burkina Faso " },
                new () { Id = 35, Name = "Burundi " },
                new () { Id = 36, Name = "Cambodia " },
                new () { Id = 37, Name = "Cameroon " },
                new () { Id = 38, Name = "Canada " },
                new () { Id = 39, Name = "Cape Verde " },
                new () { Id = 40, Name = "Cayman Islands " },
                new () { Id = 41, Name = "Central African Republic " },
                new () { Id = 42, Name = "Chad " },
                new () { Id = 43, Name = "Chile " },
                new () { Id = 44, Name = "China " },
                new () { Id = 45, Name = "Christmas Island " },
                new () { Id = 46, Name = "Cocos (Keeling) Islands " },
                new () { Id = 47, Name = "Colombia " },
                new () { Id = 48, Name = "Comoros " },
                new () { Id = 49, Name = "Congo " },
                new () { Id = 50, Name = "Congo, The Democratic Republic of The " },
                new () { Id = 51, Name = "Cook Islands " },
                new () { Id = 52, Name = "Costa Rica " },
                new () { Id = 53, Name = "Cote D'ivoire " },
                new () { Id = 54, Name = "Croatia " },
                new () { Id = 55, Name = "Cuba " },
                new () { Id = 56, Name = "Cyprus " },
                new () { Id = 57, Name = "Czech Republic " },
                new () { Id = 58, Name = "Denmark " },
                new () { Id = 59, Name = "Djibouti " },
                new () { Id = 60, Name = "Dominica " },
                new () { Id = 61, Name = "Dominican Republic " },
                new () { Id = 62, Name = "Ecuador " },
                new () { Id = 63, Name = "Egypt " },
                new () { Id = 64, Name = "El Salvador " },
                new () { Id = 65, Name = "Equatorial Guinea " },
                new () { Id = 66, Name = "Eritrea " },
                new () { Id = 67, Name = "Estonia " },
                new () { Id = 68, Name = "Ethiopia " },
                new () { Id = 69, Name = "Falkland Islands (Malvinas) " },
                new () { Id = 70, Name = "Faroe Islands " },
                new () { Id = 71, Name = "Fiji " },
                new () { Id = 72, Name = "Finland " },
                new () { Id = 73, Name = "France " },
                new () { Id = 74, Name = "French Guiana " },
                new () { Id = 75, Name = "French Polynesia " },
                new () { Id = 76, Name = "French Southern Territories " },
                new () { Id = 77, Name = "Gabon " },
                new () { Id = 78, Name = "Gambia " },
                new () { Id = 79, Name = "Georgia " },
                new () { Id = 80, Name = "Germany " },
                new () { Id = 81, Name = "Ghana " },
                new () { Id = 82, Name = "Gibraltar " },
                new () { Id = 83, Name = "Greece " },
                new () { Id = 84, Name = "Greenland " },
                new () { Id = 85, Name = "Grenada " },
                new () { Id = 86, Name = "Guadeloupe " },
                new () { Id = 87, Name = "Guam " },
                new () { Id = 88, Name = "Guatemala " },
                new () { Id = 89, Name = "Guinea " },
                new () { Id = 90, Name = "Guinea-bissau " },
                new () { Id = 91, Name = "Guyana " },
                new () { Id = 92, Name = "Haiti " },
                new () { Id = 93, Name = "Heard Island and Mcdonald Islands " },
                new () { Id = 94, Name = "Holy See (Vatican City State) " },
                new () { Id = 95, Name = "Honduras " },
                new () { Id = 96, Name = "Hong Kong " },
                new () { Id = 97, Name = "Hungary " },
                new () { Id = 98, Name = "Iceland " },
                new () { Id = 99, Name = "India " },
                new () { Id = 100, Name = "Indonesia " },
                new () { Id = 101, Name = "Iran, Islamic Republic of " },
                new () { Id = 102, Name = "Iraq " },
                new () { Id = 103, Name = "Ireland " },
                new () { Id = 104, Name = "Israel " },
                new () { Id = 105, Name = "Italy " },
                new () { Id = 106, Name = "Jamaica " },
                new () { Id = 107, Name = "Japan " },
                new () { Id = 108, Name = "Jordan " },
                new () { Id = 109, Name = "Kazakhstan " },
                new () { Id = 110, Name = "Kenya " },
                new () { Id = 111, Name = "Kiribati " },
                new () { Id = 112, Name = "Korea, Democratic People's Republic of " },
                new () { Id = 113, Name = "Korea, Republic of " },
                new () { Id = 114, Name = "Kuwait " },
                new () { Id = 115, Name = "Kyrgyzstan " },
                new () { Id = 116, Name = "Lao People's Democratic Republic " },
                new () { Id = 117, Name = "Latvia " },
                new () { Id = 118, Name = "Lebanon " },
                new () { Id = 119, Name = "Lesotho " },
                new () { Id = 120, Name = "Liberia " },
                new () { Id = 121, Name = "Libyan Arab Jamahiriya " },
                new () { Id = 122, Name = "Liechtenstein " },
                new () { Id = 123, Name = "Lithuania " },
                new () { Id = 124, Name = "Luxembourg " },
                new () { Id = 125, Name = "Macao " },
                new () { Id = 126, Name = "North Macedonia " },
                new () { Id = 127, Name = "Madagascar " },
                new () { Id = 128, Name = "Malawi " },
                new () { Id = 129, Name = "Malaysia " },
                new () { Id = 130, Name = "Maldives " },
                new () { Id = 131, Name = "Mali " },
                new () { Id = 132, Name = "Malta " },
                new () { Id = 133, Name = "Marshall Islands " },
                new () { Id = 134, Name = "Martinique " },
                new () { Id = 135, Name = "Mauritania " },
                new () { Id = 136, Name = "Mauritius " },
                new () { Id = 137, Name = "Mayotte " },
                new () { Id = 138, Name = "Mexico " },
                new () { Id = 139, Name = "Micronesia, Federated States of " },
                new () { Id = 140, Name = "Moldova, Republic of " },
                new () { Id = 141, Name = "Monaco " },
                new () { Id = 142, Name = "Mongolia " },
                new () { Id = 143, Name = "Montserrat " },
                new () { Id = 144, Name = "Morocco " },
                new () { Id = 145, Name = "Mozambique " },
                new () { Id = 146, Name = "Myanmar " },
                new () { Id = 147, Name = "Namibia " },
                new () { Id = 148, Name = "Nauru " },
                new () { Id = 149, Name = "Nepal " },
                new () { Id = 150, Name = "Netherlands " },
                new () { Id = 151, Name = "Netherlands Antilles " },
                new () { Id = 152, Name = "New Caledonia " },
                new () { Id = 153, Name = "New Zealand " },
                new () { Id = 154, Name = "Nicaragua " },
                new () { Id = 155, Name = "Niger " },
                new () { Id = 156, Name = "Nigeria " },
                new () { Id = 157, Name = "Niue " },
                new () { Id = 158, Name = "Norfolk Island " },
                new () { Id = 159, Name = "Northern Mariana Islands " },
                new () { Id = 160, Name = "Norway " },
                new () { Id = 161, Name = "Oman " },
                new () { Id = 162, Name = "Pakistan " },
                new () { Id = 163, Name = "Palau " },
                new () { Id = 164, Name = "Palestinian Territory, Occupied " },
                new () { Id = 165, Name = "Panama " },
                new () { Id = 166, Name = "Papua New Guinea " },
                new () { Id = 167, Name = "Paraguay " },
                new () { Id = 168, Name = "Peru " },
                new () { Id = 169, Name = "Philippines " },
                new () { Id = 170, Name = "Pitcairn " },
                new () { Id = 171, Name = "Poland " },
                new () { Id = 172, Name = "Portugal " },
                new () { Id = 173, Name = "Puerto Rico " },
                new () { Id = 174, Name = "Qatar " },
                new () { Id = 175, Name = "Reunion " },
                new () { Id = 176, Name = "Romania " },
                new () { Id = 177, Name = "Russian Federation " },
                new () { Id = 178, Name = "Rwanda " },
                new () { Id = 179, Name = "Saint Helena " },
                new () { Id = 180, Name = "Saint Kitts and Nevis " },
                new () { Id = 181, Name = "Saint Lucia " },
                new () { Id = 182, Name = "Saint Pierre and Miquelon " },
                new () { Id = 183, Name = "Saint Vincent and The Grenadines " },
                new () { Id = 184, Name = "Samoa " },
                new () { Id = 185, Name = "San Marino " },
                new () { Id = 186, Name = "Sao Tome and Principe " },
                new () { Id = 187, Name = "Saudi Arabia " },
                new () { Id = 188, Name = "Senegal " },
                new () { Id = 189, Name = "Serbia and Montenegro " },
                new () { Id = 190, Name = "Seychelles " },
                new () { Id = 191, Name = "Sierra Leone " },
                new () { Id = 192, Name = "Singapore " },
                new () { Id = 193, Name = "Slovakia " },
                new () { Id = 194, Name = "Slovenia " },
                new () { Id = 195, Name = "Solomon Islands " },
                new () { Id = 196, Name = "Somalia " },
                new () { Id = 197, Name = "South Africa " },
                new () { Id = 198, Name = "South Georgia and The South Sandwich Islands " },
                new () { Id = 199, Name = "Spain " },
                new () { Id = 200, Name = "Sri Lanka " },
                new () { Id = 201, Name = "Sudan " },
                new () { Id = 202, Name = "Suriname " },
                new () { Id = 203, Name = "Svalbard and Jan Mayen " },
                new () { Id = 204, Name = "Swaziland " },
                new () { Id = 205, Name = "Sweden " },
                new () { Id = 206, Name = "Switzerland " },
                new () { Id = 207, Name = "Syrian Arab Republic " },
                new () { Id = 208, Name = "Taiwan, Province of China " },
                new () { Id = 209, Name = "Tajikistan " },
                new () { Id = 210, Name = "Tanzania, United Republic of " },
                new () { Id = 211, Name = "Thailand " },
                new () { Id = 212, Name = "Timor-leste " },
                new () { Id = 213, Name = "Togo " },
                new () { Id = 214, Name = "Tokelau " },
                new () { Id = 215, Name = "Tonga " },
                new () { Id = 216, Name = "Trinidad and Tobago " },
                new () { Id = 217, Name = "Tunisia " },
                new () { Id = 218, Name = "Turkey " },
                new () { Id = 219, Name = "Turkmenistan " },
                new () { Id = 220, Name = "Turks and Caicos Islands " },
                new () { Id = 221, Name = "Tuvalu " },
                new () { Id = 222, Name = "Uganda " },
                new () { Id = 223, Name = "Ukraine " },
                new () { Id = 224, Name = "United Arab Emirates " },
                new () { Id = 225, Name = "United Kingdom " },
                new () { Id = 226, Name = "United States " },
                new () { Id = 227, Name = "United States Minor Outlying Islands " },
                new () { Id = 228, Name = "Uruguay " },
                new () { Id = 229, Name = "Uzbekistan " },
                new () { Id = 230, Name = "Vanuatu " },
                new () { Id = 231, Name = "Venezuela " },
                new () { Id = 232, Name = "Viet Nam " },
                new () { Id = 233, Name = "Virgin Islands, British " },
                new () { Id = 234, Name = "Virgin Islands, U.S. " },
                new () { Id = 235, Name = "Wallis and Futuna " },
                new () { Id = 236, Name = "Western Sahara " },
                new () { Id = 237, Name = "Yemen " },
                new () { Id = 238, Name = "Zambia " },
                new () { Id = 239, Name = "Zimbabwe" }
            };
        }
    }
}
