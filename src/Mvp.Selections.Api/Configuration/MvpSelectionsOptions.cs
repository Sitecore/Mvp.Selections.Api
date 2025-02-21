namespace Mvp.Selections.Api.Configuration;

public class MvpSelectionsOptions
{
    public const string MvpSelections = "MvpSelections";

    public int TimeFrameMonths { get; set; } = 18;

    public MentorContactOptions MentorContact { get; set; } = new();

    public class MentorContactOptions
    {
        public string TemplateId { get; set; } = string.Empty;

        public string MessageSubstitutionKey { get; set; } = "message";

        public string MentorNameSubstitutionKey { get; set; } = "mentorname";

        public string MenteeNameSubstitutionKey { get; set; } = "menteename";

        public int MaxContactPer24HourPerUser { get; set; } = 5;
    }
}