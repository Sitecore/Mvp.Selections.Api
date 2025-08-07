namespace Mvp.Selections.Api.Model
{
    public class LicenseWithUserInfo(Guid id) : Domain.License(id)
    {
        public string? AssignedUserName { get; set; }
    }
}