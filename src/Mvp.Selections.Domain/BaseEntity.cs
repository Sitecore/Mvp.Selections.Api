using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Mvp.Selections.Data")]
[assembly: InternalsVisibleTo("Mvp.Selections.Client")]
[assembly: InternalsVisibleTo("Mvp.Selections.Client.Tests")]

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

        [JsonInclude]
        public DateTime CreatedOn { get; internal set; }

        [JsonInclude]
        public string CreatedBy { get; internal set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
