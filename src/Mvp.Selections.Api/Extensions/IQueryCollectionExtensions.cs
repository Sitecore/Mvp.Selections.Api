using System;
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
            T result;
            if (queryCollection.TryGetValue(key, out StringValues values))
            {
                object resultValue = values.FirstOrDefault();
                result = (T)Convert.ChangeType(resultValue, typeof(T));
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }
    }
}
