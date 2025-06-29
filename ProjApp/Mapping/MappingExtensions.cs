using System.Linq.Expressions;

namespace ProjApp.Mapping;

public static class MappingExtensions
{
    public static IQueryable<TOut> Map<TIn, TOut>(this IQueryable<TIn> query, Expression<Func<TIn, TOut>> map)
    {
        return query.Select(map);
    }
}