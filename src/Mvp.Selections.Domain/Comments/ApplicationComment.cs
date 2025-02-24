namespace Mvp.Selections.Domain.Comments;

public class ApplicationComment(Guid id)
    : Comment(id)
{
    public Application Application { get; set; } = null!;
}