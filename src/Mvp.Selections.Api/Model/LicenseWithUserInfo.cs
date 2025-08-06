namespace Mvp.Selections.Api.Model
{
    public class LicenseWithUserInfo
    {
        public Guid Id { get; set; }

        public DateTime ExpirationDate { get; set; }

        public Guid? AssignedUserId { get; set; }

        public string? AssignedUserName { get; set; }

        public static LicenseWithUserInfo MapFromLicense(Domain.License license, string? userName = null)
        {
            return new LicenseWithUserInfo
            {
                Id = license.Id,
                ExpirationDate = license.ExpirationDate,
                AssignedUserId = license.AssignedUserId,
                AssignedUserName = userName
            };
        }
    }
}