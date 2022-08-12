namespace Mvp.Selections.Client.Configuration
{
    public class MvpSelectionsApiClientOptions
    {
        public const string MvpSelectionsApiClient = "MvpSelectionsApiClient";

        public Uri BaseAddress { get; set; } = new ("http://localhost");
    }
}
