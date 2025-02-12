using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvp.Selections.Api.Extensions;

// ReSharper disable once InconsistentNaming - This class extends the interface, not the type
public static class IListExtensions
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list is List<T> concreteList)
        {
            concreteList.AddRange(items);
        }
        else
        {
            foreach (T item in items)
            {
                list.Add(item);
            }
        }
    }

    public static string ToCommaSeparatedStringOrNullLiteral<T>(this IList<T>? list)
    {
        return list != null ? string.Join(',', list) : "null";
    }

    public static bool TryAdd<T, TKey>(this IList<T> list, TKey key, T addition, Func<T, TKey> keySelector)
    {
        bool result = false;
        if (list.All(t => !(keySelector(t)?.Equals(key) ?? false)))
        {
            list.Add(addition);
            result = true;
        }

        return result;
    }
}