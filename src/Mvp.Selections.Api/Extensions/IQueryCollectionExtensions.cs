using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mvp.Selections.Api.Extensions
{
    // ReSharper disable once InconsistentNaming - This class extends the interface, not the type
    public static class IQueryCollectionExtensions
    {
        public static T GetFirstValueOrDefault<T>(
            this IQueryCollection queryCollection,
            string key,
            T defaultValue = default)
        {
            T result = queryCollection.TryGetValue(key, out StringValues values)
                ? ConvertOrDefault(values.FirstOrDefault(), defaultValue)
                : defaultValue;

            return result;
        }

        public static IList<T> GetValuesOrEmpty<T>(this IQueryCollection queryCollection, string key)
        {
            List<T> result = new ();
            if (queryCollection.TryGetValue(key, out StringValues values))
            {
                result.AddRange(values.Select(value => ConvertOrDefault<T>(value)));
            }

            return result;
        }

        private static T ConvertOrDefault<T>(string value, T defaultValue = default)
        {
            T result;
            if (!string.IsNullOrEmpty(value))
            {
                Type t = typeof(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    t = Nullable.GetUnderlyingType(t) ?? t;
                }

                if (t.IsAssignableTo(typeof(Enum)) && Enum.TryParse(t, value, out object enumResult))
                {
                    result = (T)enumResult;
                }
                else if (t == typeof(Guid) && Guid.TryParse(value, out Guid guidResult))
                {
                    result = (T)Convert.ChangeType(guidResult, t);
                }
                else
                {
                    result = (T)Convert.ChangeType(value, t);
                }
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }
    }
}
