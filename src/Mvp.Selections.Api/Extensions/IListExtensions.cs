using System.Collections.Generic;

namespace Mvp.Selections.Api.Extensions
{
    // ReSharper disable once InconsistentNaming - This class extends the interface, not the type
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Add(item);
            }
        }
    }
}
