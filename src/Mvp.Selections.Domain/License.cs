namespace Mvp.Selections.Domain
{
    public class License(Guid id) : BaseEntity<Guid>(id)
    {
        public string LicenseContent { get; set; } = string.Empty;

        public DateTime ExpirationDate { get; set; }

        public Guid? AssignedUserId { get; set; }
    }
}
