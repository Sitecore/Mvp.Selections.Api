using System;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Api.Model.Comments
{
    public class PatchCommentBody : Comment
    {
        public PatchCommentBody(Guid id)
            : base(id)
        {
        }
    }
}
