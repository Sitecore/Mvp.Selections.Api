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
                string resultValue = values.FirstOrDefault();
                if (resultValue != null)
                {
                    Type t = typeof(T);
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        t = Nullable.GetUnderlyingType(t) ?? t;
                    }

                    if (t.IsAssignableTo(typeof(Enum)) && Enum.TryParse(t, resultValue, out object enumResult))
                    {
                        result = (T)enumResult;
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(resultValue, t);
                    }
                }
                else
                {
                    result = defaultValue;
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
