using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mvp.Selections.Data")]

namespace Mvp.Selections.Domain
{
    public abstract class BaseEntity<TId>
        where TId : struct
    {
        protected BaseEntity(TId id)
        {
            Id = id;
            CreatedOn = DateTime.UtcNow;
            CreatedBy = string.Empty;
        }

        public TId Id { get; private set; }

        public DateTime CreatedOn { get; internal set; }

        public string CreatedBy { get; internal set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
