namespace Mvp.Selections.Domain.Comments
{
    public abstract class Comment(Guid id)
        : BaseEntity<Guid>(id)
    {
        public string Value { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}
