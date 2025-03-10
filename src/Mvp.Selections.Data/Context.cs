using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data;

public class Context : DbContext
{
    public static readonly Guid DefaultAdminRoleId = new("00000000-0000-0000-0000-000000000001");

    public static readonly Guid DefaultCandidateRoleId = new("00000000-0000-0000-0000-000000000002");

    public static readonly Guid DefaultReviewerRoleId = new("00000000-0000-0000-0000-000000000003");

    public static readonly Guid DefaultScorerRoleId = new("00000000-0000-0000-0000-000000000004");

    public static readonly Guid DefaultCommenterRoleId = new("00000000-0000-0000-0000-000000000005");

    public static readonly Guid DefaultAwarderRoleId = new("00000000-0000-0000-0000-000000000006");

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

    public DbSet<Dispatch> Dispatches => Set<Dispatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guid adminUserId = new("00000000-0000-0000-0000-000000000001");

        modelBuilder.Entity<Country>()
            .HasData(SeedCountries());

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Identifier)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasData(new { Id = adminUserId, Identifier = "00uqyu5bxcffmH3xP0h7", Name = "Ivan Lieckens", Email = "ivan.lieckens@sitecore.com", CountryId = (short)21, ImageType = ImageType.Anonymous, CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System", IsMentor = false, IsOpenToNewMentees = false });
        modelBuilder.Entity<User>()
            .Property(u => u.IsMentor);
        modelBuilder.Entity<User>()
            .Property(u => u.IsOpenToNewMentees);
        modelBuilder.Entity<User>()
            .Property(u => u.MentorDescription);

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
        modelBuilder.Entity<ScoreCategory>()
            .Property(sc => sc.Weight)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Comment>()
            .Navigation(c => c.User)
            .AutoInclude();

        modelBuilder.Entity<ApplicationComment>();
    }

    private static IEnumerable<Country> SeedCountries()
    {
        // ReSharper disable StringLiteralTypo
        return
        [
            new Country(1) { Name = "Afghanistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(2) { Name = "Albania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(3) { Name = "Algeria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(4) { Name = "American Samoa", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(5) { Name = "Andorra", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(6) { Name = "Angola", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(7) { Name = "Anguilla", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(8) { Name = "Antarctica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(9) { Name = "Antigua and Barbuda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(10) { Name = "Argentina", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(11) { Name = "Armenia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(12) { Name = "Aruba", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(13) { Name = "Australia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(14) { Name = "Austria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(15) { Name = "Azerbaijan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(16) { Name = "Bahamas", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(17) { Name = "Bahrain", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(18) { Name = "Bangladesh", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(19) { Name = "Barbados", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(20) { Name = "Belarus", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(21) { Name = "Belgium", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(22) { Name = "Belize", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(23) { Name = "Benin", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(24) { Name = "Bermuda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(25) { Name = "Bhutan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(26) { Name = "Bolivia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(27) { Name = "Bosnia and Herzegovina", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(28) { Name = "Botswana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(29) { Name = "Bouvet Island", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(30) { Name = "Brazil", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(31) { Name = "British Indian Ocean Territory", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(32) { Name = "Brunei Darussalam", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(33) { Name = "Bulgaria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(34) { Name = "Burkina Faso", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(35) { Name = "Burundi", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(36) { Name = "Cambodia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(37) { Name = "Cameroon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(38) { Name = "Canada", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(39) { Name = "Cape Verde", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(40) { Name = "Cayman Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(41) { Name = "Central African Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(42) { Name = "Chad", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(43) { Name = "Chile", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(44) { Name = "China", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(45) { Name = "Christmas Island", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(46) { Name = "Cocos (Keeling) Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(47) { Name = "Colombia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(48) { Name = "Comoros", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(49) { Name = "Congo", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(50) { Name = "Congo, The Democratic Republic of The", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(51) { Name = "Cook Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(52) { Name = "Costa Rica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(53) { Name = "Cote D'ivoire", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(54) { Name = "Croatia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(55) { Name = "Cuba", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(56) { Name = "Cyprus", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(57) { Name = "Czech Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(58) { Name = "Denmark", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(59) { Name = "Djibouti", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(60) { Name = "Dominica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(61) { Name = "Dominican Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(62) { Name = "Ecuador", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(63) { Name = "Egypt", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(64) { Name = "El Salvador", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(65) { Name = "Equatorial Guinea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(66) { Name = "Eritrea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(67) { Name = "Estonia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(68) { Name = "Ethiopia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(69) { Name = "Falkland Islands (Malvinas)", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(70) { Name = "Faroe Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(71) { Name = "Fiji", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(72) { Name = "Finland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(73) { Name = "France", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(74) { Name = "French Guiana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(75) { Name = "French Polynesia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(76) { Name = "French Southern Territories", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(77) { Name = "Gabon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(78) { Name = "Gambia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(79) { Name = "Georgia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(80) { Name = "Germany", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(81) { Name = "Ghana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(82) { Name = "Gibraltar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(83) { Name = "Greece", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(84) { Name = "Greenland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(85) { Name = "Grenada", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(86) { Name = "Guadeloupe", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(87) { Name = "Guam", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(88) { Name = "Guatemala", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(89) { Name = "Guinea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(90) { Name = "Guinea-bissau", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(91) { Name = "Guyana", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(92) { Name = "Haiti", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(93) { Name = "Heard Island and Mcdonald Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(94) { Name = "Holy See (Vatican City State)", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(95) { Name = "Honduras", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(96) { Name = "Hong Kong", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(97) { Name = "Hungary", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(98) { Name = "Iceland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(99) { Name = "India", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(100) { Name = "Indonesia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(101) { Name = "Iran, Islamic Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(102) { Name = "Iraq", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(103) { Name = "Ireland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(104) { Name = "Israel", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(105) { Name = "Italy", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(106) { Name = "Jamaica", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(107) { Name = "Japan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(108) { Name = "Jordan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(109) { Name = "Kazakhstan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(110) { Name = "Kenya", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(111) { Name = "Kiribati", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(112) { Name = "Korea, Democratic People's Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(113) { Name = "Korea, Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(114) { Name = "Kuwait", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(115) { Name = "Kyrgyzstan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(116) { Name = "Lao People's Democratic Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(117) { Name = "Latvia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(118) { Name = "Lebanon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(119) { Name = "Lesotho", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(120) { Name = "Liberia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(121) { Name = "Libyan Arab Jamahiriya", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(122) { Name = "Liechtenstein", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(123) { Name = "Lithuania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(124) { Name = "Luxembourg", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(125) { Name = "Macao", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(126) { Name = "North Macedonia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(127) { Name = "Madagascar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(128) { Name = "Malawi", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(129) { Name = "Malaysia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(130) { Name = "Maldives", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(131) { Name = "Mali", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(132) { Name = "Malta", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(133) { Name = "Marshall Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(134) { Name = "Martinique", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(135) { Name = "Mauritania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(136) { Name = "Mauritius", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(137) { Name = "Mayotte", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(138) { Name = "Mexico", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(139) { Name = "Micronesia, Federated States of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(140) { Name = "Moldova, Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(141) { Name = "Monaco", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(142) { Name = "Mongolia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(143) { Name = "Montserrat", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(144) { Name = "Morocco", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(145) { Name = "Mozambique", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(146) { Name = "Myanmar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(147) { Name = "Namibia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(148) { Name = "Nauru", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(149) { Name = "Nepal", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(150) { Name = "Netherlands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(151) { Name = "Netherlands Antilles", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(152) { Name = "New Caledonia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(153) { Name = "New Zealand", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(154) { Name = "Nicaragua", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(155) { Name = "Niger", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(156) { Name = "Nigeria", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(157) { Name = "Niue", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(158) { Name = "Norfolk Island", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(159) { Name = "Northern Mariana Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(160) { Name = "Norway", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(161) { Name = "Oman", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(162) { Name = "Pakistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(163) { Name = "Palau", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(164) { Name = "Palestinian Territory, Occupied", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(165) { Name = "Panama", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(166) { Name = "Papua New Guinea", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(167) { Name = "Paraguay", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(168) { Name = "Peru", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(169) { Name = "Philippines", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(170) { Name = "Pitcairn", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(171) { Name = "Poland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(172) { Name = "Portugal", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(173) { Name = "Puerto Rico", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(174) { Name = "Qatar", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(175) { Name = "Reunion", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(176) { Name = "Romania", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(177) { Name = "Russian Federation", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(178) { Name = "Rwanda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(179) { Name = "Saint Helena", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(180) { Name = "Saint Kitts and Nevis", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(181) { Name = "Saint Lucia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(182) { Name = "Saint Pierre and Miquelon", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(183) { Name = "Saint Vincent and The Grenadines", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(184) { Name = "Samoa", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(185) { Name = "San Marino", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(186) { Name = "Sao Tome and Principe", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(187) { Name = "Saudi Arabia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(188) { Name = "Senegal", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(189) { Name = "Serbia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(190) { Name = "Seychelles", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(191) { Name = "Sierra Leone", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(192) { Name = "Singapore", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(193) { Name = "Slovakia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(194) { Name = "Slovenia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(195) { Name = "Solomon Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(196) { Name = "Somalia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(197) { Name = "South Africa", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(198) { Name = "South Georgia and The South Sandwich Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(199) { Name = "Spain", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(200) { Name = "Sri Lanka", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(201) { Name = "Sudan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(202) { Name = "Suriname", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(203) { Name = "Svalbard and Jan Mayen", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(204) { Name = "Swaziland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(205) { Name = "Sweden", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(206) { Name = "Switzerland", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(207) { Name = "Syrian Arab Republic", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(208) { Name = "Taiwan, Province of China", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(209) { Name = "Tajikistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(210) { Name = "Tanzania, United Republic of", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(211) { Name = "Thailand", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(212) { Name = "Timor-leste", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(213) { Name = "Togo", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(214) { Name = "Tokelau", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(215) { Name = "Tonga", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(216) { Name = "Trinidad and Tobago", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(217) { Name = "Tunisia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(218) { Name = "Turkey", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(219) { Name = "Turkmenistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(220) { Name = "Turks and Caicos Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(221) { Name = "Tuvalu", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(222) { Name = "Uganda", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(223) { Name = "Ukraine", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(224) { Name = "United Arab Emirates", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(225) { Name = "United Kingdom", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(226) { Name = "United States", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(227) { Name = "United States Minor Outlying Islands", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(228) { Name = "Uruguay", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(229) { Name = "Uzbekistan", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(230) { Name = "Vanuatu", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(231) { Name = "Venezuela", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(232) { Name = "Viet Nam", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(233) { Name = "Virgin Islands, British", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(234) { Name = "Virgin Islands, U.S.", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(235) { Name = "Wallis and Futuna", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(236) { Name = "Western Sahara", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(237) { Name = "Yemen", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(238) { Name = "Zambia", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(239) { Name = "Zimbabwe", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" },
            new Country(240) { Name = "Montenegro", CreatedOn = new DateTime(2022, 9, 1), CreatedBy = "System" }
        ];

        // ReSharper restore StringLiteralTypo
    }
}