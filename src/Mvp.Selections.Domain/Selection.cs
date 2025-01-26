namespace Mvp.Selections.Domain;

public class Selection(Guid id)
    : BaseEntity<Guid>(id)
{
    public short Year { get; set; }

    public bool? ApplicationsActive { get; set; }

    public DateTime ApplicationsStart { get; set; }

    public DateTime ApplicationsEnd { get; set; }

    public bool? ReviewsActive { get; set; }

    public DateTime ReviewsStart { get; set; }

    public DateTime ReviewsEnd { get; set; }

    public ICollection<MvpType> MvpTypes { get; init; } = new List<MvpType>();

    public bool Finalized { get; set; }

    public bool AreApplicationsOpen()
    {
        return ApplicationsActive ?? (ApplicationsStart < DateTime.UtcNow && ApplicationsEnd > DateTime.UtcNow);
    }

    public bool AreReviewsOpen()
    {
        return ReviewsActive ?? (ReviewsStart < DateTime.UtcNow && ReviewsEnd > DateTime.UtcNow);
    }
}