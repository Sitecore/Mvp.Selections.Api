using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Mvp.Selections.Data.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Includes<T>(this IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includes)
            where T : class
        {
            return includes.Aggregate(query, (current, include) => current.Include(include));
        }
    }
}
