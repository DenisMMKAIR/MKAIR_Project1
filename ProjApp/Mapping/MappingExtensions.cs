using System.Linq.Expressions;

namespace ProjApp.Mapping;

public static class MappingExtensions
{
    /// <summary>
    /// Custom mapping extension. Maps a query to a new type
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="query"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static IQueryable<TOut> Map<TIn, TOut>(this IQueryable<TIn> query, Expression<Func<TIn, TOut>> map)
    {
        return query.Select(map);
    }
}