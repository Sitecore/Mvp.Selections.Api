using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public abstract class BaseEntity<TId> : IId<TId>
        where TId : struct
    {
        protected BaseEntity(TId id)
        {
            Id = id;
        }

        public TId Id { get; private set; }
    }
}
