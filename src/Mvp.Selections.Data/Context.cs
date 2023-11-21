using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data
{
    public class Context : DbContext
    {
        public static readonly Guid DefaultAdminRoleId = new ("00000000-0000-0000-0000-000000000001");

        public static readonly Guid DefaultCandidateRoleId = new ("00000000-0000-0000-0000-000000000002");

        public static readonly Guid DefaultReviewerRoleId = new ("00000000-0000-0000-0000-000000000003");

        public static readonly Guid DefaultScorerRoleId = new ("00000000-0000-0000-0000-000000000004");

        public static readonly Guid DefaultCommenterRoleId = new ("00000000-0000-0000-0000-000000000005");

        public static readonly Guid DefaultAwarderRoleId = new ("00000000-0000-0000-0000-000000000006");

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        protected Context()
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        // ReSharper disable once UnusedMember.Global - Used Generically
        public DbSet<ProfileLink> ProfileLinks => Set<ProfileLink>();

        public DbSet<Consent> Consents => Set<Consent>();

        public DbSet<Selection> Selections => Set<Selection>();

        public DbSet<Application> Applications => Set<Application>();

        // ReSharper disable once UnusedMember.Global - Used Generically
        public DbSet<Contribution> Contributions => Set<Contribution>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<ReviewCategoryScore> ReviewCategoryScores => Set<ReviewCategoryScore>();

        // ReSharper disable once UnusedMember.Global - Used Generically
        public DbSet<Region> Regions => Set<Region>();

        public DbSet<Country> Countries => Set<Country>();

        public DbSet<ScoreCategory> ScoreCategories => Set<ScoreCategory>();

        // ReSharper disable once UnusedMember.Global - Used Generically
        public DbSet<Score> Scores => Set<Score>();

        // ReSharper disable once UnusedMember.Global - Used Generically
        public DbSet<Product> Products => Set<Product>();

        public DbSet<Comment> Comments => Set<Comment>();

        public DbSet<Title> Titles => Set<Title>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Guid adminUserId = new ("00000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Country>()
                .HasData(SeedCountries());

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Identifier)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasData(new { Id = adminUserId, Identifier = "00uqyu5bxcffmH3xP0h7", Name = "Ivan Lieckens", Email = "ivan.lieckens@sitecore.com", CountryId = (short)21, ImageType = ImageType.Anonymous, CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" });

            modelBuilder.Entity("RoleUser")
                .HasData(new { UsersId = adminUserId, RolesId = DefaultAdminRoleId });

            modelBuilder.Entity<SelectionRole>();

            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = DefaultAdminRoleId, Name = "Admin", Rights = Right.Admin | Right.Any, CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" });
            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = DefaultCandidateRoleId, Name = "Candidate", Rights = Right.Apply | Right.Any, CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" });
            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = DefaultReviewerRoleId, Name = "Reviewer", Rights = Right.Review | Right.Any, CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" });
            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = DefaultScorerRoleId, Name = "Scorer", Rights = Right.Score | Right.Any, CreatedOn = new DateTime(2023, 1, 1), CreatedBy = "System" });
            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = DefaultCommenterRoleId, Name = "Commenter", Rights = Right.Comment | Right.Any, CreatedOn = new DateTime(2023, 1, 1), CreatedBy = "System" });
            modelBuilder.Entity<SystemRole>()
                .HasData(new { Id = DefaultAwarderRoleId, Name = "Awarder", Rights = Right.Award | Right.Any, CreatedOn = new DateTime(2023, 1, 1), CreatedBy = "System" });

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
            modelBuilder.Entity<Application>()
                .HasIndex("ApplicantId", "SelectionId")
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.Reviews)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Title>()
                .HasOne(t => t.MvpType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Title>()
                .HasIndex("ApplicationId")
                .IsUnique();

            modelBuilder.Entity<Selection>()
                .HasAlternateKey(s => s.Year);

            modelBuilder.Entity<Contribution>()
                .Navigation(al => al.RelatedProducts)
                .AutoInclude();

            modelBuilder.Entity<ScoreCategory>()
                .HasMany(sc => sc.ScoreOptions)
                .WithMany(s => s.ScoreCategories);
            modelBuilder.Entity<ScoreCategory>()
                .HasOne(sc => sc.CalculationScore)
                .WithMany();

            modelBuilder.Entity<Comment>()
                .Navigation(c => c.User)
                .AutoInclude();

            modelBuilder.Entity<ApplicationComment>();
        }

        private static IEnumerable<Country> SeedCountries()
        {
            return new List<Country>
            {
                new (1) { Name = "Afghanistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (2) { Name = "Albania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (3) { Name = "Algeria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (4) { Name = "American Samoa", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (5) { Name = "Andorra", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (6) { Name = "Angola", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (7) { Name = "Anguilla", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (8) { Name = "Antarctica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (9) { Name = "Antigua and Barbuda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (10) { Name = "Argentina", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (11) { Name = "Armenia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (12) { Name = "Aruba", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (13) { Name = "Australia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (14) { Name = "Austria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (15) { Name = "Azerbaijan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (16) { Name = "Bahamas", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (17) { Name = "Bahrain", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (18) { Name = "Bangladesh", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (19) { Name = "Barbados", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (20) { Name = "Belarus", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (21) { Name = "Belgium", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (22) { Name = "Belize", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (23) { Name = "Benin", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (24) { Name = "Bermuda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (25) { Name = "Bhutan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (26) { Name = "Bolivia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (27) { Name = "Bosnia and Herzegovina", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (28) { Name = "Botswana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (29) { Name = "Bouvet Island", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (30) { Name = "Brazil", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (31) { Name = "British Indian Ocean Territory", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (32) { Name = "Brunei Darussalam", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (33) { Name = "Bulgaria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (34) { Name = "Burkina Faso", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (35) { Name = "Burundi", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (36) { Name = "Cambodia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (37) { Name = "Cameroon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (38) { Name = "Canada", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (39) { Name = "Cape Verde", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (40) { Name = "Cayman Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (41) { Name = "Central African Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (42) { Name = "Chad", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (43) { Name = "Chile", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (44) { Name = "China", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (45) { Name = "Christmas Island", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (46) { Name = "Cocos (Keeling) Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (47) { Name = "Colombia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (48) { Name = "Comoros", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (49) { Name = "Congo", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (50) { Name = "Congo, The Democratic Republic of The", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (51) { Name = "Cook Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (52) { Name = "Costa Rica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (53) { Name = "Cote D'ivoire", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (54) { Name = "Croatia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (55) { Name = "Cuba", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (56) { Name = "Cyprus", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (57) { Name = "Czech Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (58) { Name = "Denmark", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (59) { Name = "Djibouti", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (60) { Name = "Dominica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (61) { Name = "Dominican Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (62) { Name = "Ecuador", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (63) { Name = "Egypt", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (64) { Name = "El Salvador", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (65) { Name = "Equatorial Guinea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (66) { Name = "Eritrea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (67) { Name = "Estonia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (68) { Name = "Ethiopia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (69) { Name = "Falkland Islands (Malvinas)", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (70) { Name = "Faroe Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (71) { Name = "Fiji", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (72) { Name = "Finland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (73) { Name = "France", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (74) { Name = "French Guiana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (75) { Name = "French Polynesia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (76) { Name = "French Southern Territories", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (77) { Name = "Gabon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (78) { Name = "Gambia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (79) { Name = "Georgia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (80) { Name = "Germany", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (81) { Name = "Ghana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (82) { Name = "Gibraltar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (83) { Name = "Greece", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (84) { Name = "Greenland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (85) { Name = "Grenada", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (86) { Name = "Guadeloupe", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (87) { Name = "Guam", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (88) { Name = "Guatemala", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (89) { Name = "Guinea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (90) { Name = "Guinea-bissau", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (91) { Name = "Guyana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (92) { Name = "Haiti", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (93) { Name = "Heard Island and Mcdonald Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (94) { Name = "Holy See (Vatican City State)", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (95) { Name = "Honduras", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (96) { Name = "Hong Kong", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (97) { Name = "Hungary", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (98) { Name = "Iceland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (99) { Name = "India", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (100) { Name = "Indonesia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (101) { Name = "Iran, Islamic Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (102) { Name = "Iraq", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (103) { Name = "Ireland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (104) { Name = "Israel", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (105) { Name = "Italy", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (106) { Name = "Jamaica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (107) { Name = "Japan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (108) { Name = "Jordan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (109) { Name = "Kazakhstan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (110) { Name = "Kenya", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (111) { Name = "Kiribati", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (112) { Name = "Korea, Democratic People's Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (113) { Name = "Korea, Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (114) { Name = "Kuwait", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (115) { Name = "Kyrgyzstan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (116) { Name = "Lao People's Democratic Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (117) { Name = "Latvia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (118) { Name = "Lebanon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (119) { Name = "Lesotho", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (120) { Name = "Liberia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (121) { Name = "Libyan Arab Jamahiriya", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (122) { Name = "Liechtenstein", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (123) { Name = "Lithuania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (124) { Name = "Luxembourg", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (125) { Name = "Macao", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (126) { Name = "North Macedonia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (127) { Name = "Madagascar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (128) { Name = "Malawi", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (129) { Name = "Malaysia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (130) { Name = "Maldives", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (131) { Name = "Mali", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (132) { Name = "Malta", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (133) { Name = "Marshall Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (134) { Name = "Martinique", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (135) { Name = "Mauritania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (136) { Name = "Mauritius", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (137) { Name = "Mayotte", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (138) { Name = "Mexico", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (139) { Name = "Micronesia, Federated States of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (140) { Name = "Moldova, Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (141) { Name = "Monaco", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (142) { Name = "Mongolia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (143) { Name = "Montserrat", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (144) { Name = "Morocco", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (145) { Name = "Mozambique", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (146) { Name = "Myanmar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (147) { Name = "Namibia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (148) { Name = "Nauru", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (149) { Name = "Nepal", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (150) { Name = "Netherlands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (151) { Name = "Netherlands Antilles", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (152) { Name = "New Caledonia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (153) { Name = "New Zealand", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (154) { Name = "Nicaragua", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (155) { Name = "Niger", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (156) { Name = "Nigeria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (157) { Name = "Niue", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (158) { Name = "Norfolk Island", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (159) { Name = "Northern Mariana Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (160) { Name = "Norway", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (161) { Name = "Oman", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (162) { Name = "Pakistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (163) { Name = "Palau", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (164) { Name = "Palestinian Territory, Occupied", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (165) { Name = "Panama", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (166) { Name = "Papua New Guinea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (167) { Name = "Paraguay", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (168) { Name = "Peru", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (169) { Name = "Philippines", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (170) { Name = "Pitcairn", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (171) { Name = "Poland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (172) { Name = "Portugal", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (173) { Name = "Puerto Rico", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (174) { Name = "Qatar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (175) { Name = "Reunion", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (176) { Name = "Romania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (177) { Name = "Russian Federation", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (178) { Name = "Rwanda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (179) { Name = "Saint Helena", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (180) { Name = "Saint Kitts and Nevis", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (181) { Name = "Saint Lucia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (182) { Name = "Saint Pierre and Miquelon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (183) { Name = "Saint Vincent and The Grenadines", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (184) { Name = "Samoa", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (185) { Name = "San Marino", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (186) { Name = "Sao Tome and Principe", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (187) { Name = "Saudi Arabia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (188) { Name = "Senegal", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (189) { Name = "Serbia and Montenegro", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (190) { Name = "Seychelles", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (191) { Name = "Sierra Leone", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (192) { Name = "Singapore", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (193) { Name = "Slovakia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (194) { Name = "Slovenia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (195) { Name = "Solomon Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (196) { Name = "Somalia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (197) { Name = "South Africa", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (198) { Name = "South Georgia and The South Sandwich Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (199) { Name = "Spain", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (200) { Name = "Sri Lanka", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (201) { Name = "Sudan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (202) { Name = "Suriname", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (203) { Name = "Svalbard and Jan Mayen", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (204) { Name = "Swaziland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (205) { Name = "Sweden", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (206) { Name = "Switzerland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (207) { Name = "Syrian Arab Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (208) { Name = "Taiwan, Province of China", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (209) { Name = "Tajikistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (210) { Name = "Tanzania, United Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (211) { Name = "Thailand", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (212) { Name = "Timor-leste", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (213) { Name = "Togo", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (214) { Name = "Tokelau", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (215) { Name = "Tonga", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (216) { Name = "Trinidad and Tobago", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (217) { Name = "Tunisia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (218) { Name = "Turkey", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (219) { Name = "Turkmenistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (220) { Name = "Turks and Caicos Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (221) { Name = "Tuvalu", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (222) { Name = "Uganda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (223) { Name = "Ukraine", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (224) { Name = "United Arab Emirates", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (225) { Name = "United Kingdom", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (226) { Name = "United States", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (227) { Name = "United States Minor Outlying Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (228) { Name = "Uruguay", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (229) { Name = "Uzbekistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (230) { Name = "Vanuatu", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (231) { Name = "Venezuela", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (232) { Name = "Viet Nam", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (233) { Name = "Virgin Islands, British", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (234) { Name = "Virgin Islands, U.S.", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (235) { Name = "Wallis and Futuna", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (236) { Name = "Western Sahara", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (237) { Name = "Yemen", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (238) { Name = "Zambia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
                new (239) { Name = "Zimbabwe", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" }
            };
        }
    }
}
