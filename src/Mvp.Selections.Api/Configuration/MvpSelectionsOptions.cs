namespace Mvp.Selections.Api.Configuration;

public class MvpSelectionsOptions
{
    public const string MvpSelections = "MvpSelections";

    public int TimeFrameMonths { get; set; } = 18;

    public MentorContactOptions MentorContact { get; set; } = new();

    public ApplicationConfirmationOptions ApplicationConfirmation { get; set; } = new();

    public class MentorContactOptions
    {
        public string TemplateId { get; set; } = string.Empty;

        public string MessageSubstitutionKey { get; set; } = "message";

        public string MentorNameSubstitutionKey { get; set; } = "mentorname";

        public string MenteeNameSubstitutionKey { get; set; } = "menteename";

        public string MenteeEmailSubstitutionKey { get; set; } = "menteeemail";

        public int MaxContactPer24HourPerUser { get; set; } = 5;
    }

    public class ApplicationConfirmationOptions
    {
        public string TemplateId { get; set; } = string.Empty;

        public string ApplicationMvpType { get; set; } = "applicationmvptype";

        public string ApplicantNameSubstitutionKey { get; set; } = "applicantname";

        public string ApplicantCountrySubstitutionKey { get; set; } = "applicantcountry";

        public string ApplicationDateSubstitutionKey { get; set; } = "applicationdate";

        public string ApplicationDataSubstitutionKey { get; set; } = "applicationdata";
    }
}