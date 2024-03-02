using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Mvp.Selections.Data")]
[assembly: InternalsVisibleTo("Mvp.Selections.Client")]
[assembly: InternalsVisibleTo("Mvp.Selections.Client.Tests")]

namespace Mvp.Selections.Domain
{
    public abstract class BaseEntity<TId>(TId id)
        where TId : struct
    {
        public TId Id { get; private set; } = id;

        [JsonInclude]
        public DateTime CreatedOn { get; internal set; } = DateTime.UtcNow;

        [JsonInclude]
        public string CreatedBy { get; internal set; } = string.Empty;

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
