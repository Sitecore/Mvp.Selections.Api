namespace Mvp.Selections.Domain.Comments
{
    public class ApplicationComment : Comment
    {
        public ApplicationComment(Guid id)
            : base(id)
        {
        }

        public Application Application { get; set; } = null!;
    }
}
