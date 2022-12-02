using Mvp.Selections.Domain;

namespace Mvp.Selections.Client.Models
{
    public class Applicant
    {
        public string? Name { get; set; }

        public Uri? ImageUri { get; set; }

        public Guid ApplicationId { get; set; }

        public Country? Country { get; set; }

        public MvpType? MvpType { get; set; }
    }
}
