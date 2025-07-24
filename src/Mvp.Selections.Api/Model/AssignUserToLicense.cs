using System.ComponentModel.DataAnnotations;

namespace Mvp.Selections.Api.Model
{
    public class AssignUserToLicense
    {
        public Guid LicenceId { get; set; }

        public string Email { get; set; }
    }
}
