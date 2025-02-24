namespace Mvp.Selections.Domain.Roles;

public class SelectionRole(Guid id)
    : Role(id)
{
    public Guid? ApplicationId { get; set; }

    public Application? Application { get; set; }

    public short? CountryId { get; set; }

    public Country? Country { get; set; }

    public short? MvpTypeId { get; set; }

    public MvpType? MvpType { get; set; }

    public int? RegionId { get; set; }

    public Region? Region { get; set; }

    public Guid? SelectionId { get; set; }

    public Selection? Selection { get; set; }
}