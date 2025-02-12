using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Mvp.Selections.Data.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Includes<T>(this IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includes)
        where T : class
    {
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page = 1, short pageSize = 100)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}