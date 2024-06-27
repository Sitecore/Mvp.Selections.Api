namespace Mvp.Selections.Client.Configuration
{
    /// <summary>
    /// Configuration options for the MVP Selections API Client.
    /// </summary>
    public class MvpSelectionsApiClientOptions
    {
        /// <summary>
        /// Default key of the config section.
        /// </summary>
        public const string MvpSelectionsApiClient = "MvpSelectionsApiClient";

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> base address to use for the <see cref="HttpClient"/> used to connect to the API.
        /// </summary>
        public Uri BaseAddress { get; set; } = new ("http://localhost");
    }
}
