namespace Mvp.Selections.Api.Extensions;

// ReSharper disable once InconsistentNaming - This class extends the interface, not the type
public static class ICollectionExtensions
{
    public static T? Median<T>(this ICollection<T> collection)
    {
        return collection.OrderBy(i => i).Skip(collection.Count / 2).Take(1).SingleOrDefault();
    }
}