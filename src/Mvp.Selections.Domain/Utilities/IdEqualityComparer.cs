namespace Mvp.Selections.Domain.Utilities;

public class IdEqualityComparer<T, TId> : IEqualityComparer<T>
    where T : BaseEntity<TId>
    where TId : struct
{
    public bool Equals(T? x, T? y)
    {
        bool result = ReferenceEquals(x, y);
        if (!result && x is not null && y is not null)
        {
            result = x.Id.Equals(y.Id);
        }

        return result;
    }

    public int GetHashCode(T obj)
    {
        return obj.Id.GetHashCode();
    }
}