namespace Mvp.Selections.Domain.Comments
{
    public abstract class Comment : BaseEntity<Guid>
    {
        protected Comment(Guid id)
            : base(id)
        {
        }

        public string Value { get; set; } = string.Empty;
    }
}
