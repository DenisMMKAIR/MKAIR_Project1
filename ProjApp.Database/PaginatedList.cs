using Microsoft.EntityFrameworkCore;

namespace ProjApp.Database;

public class PaginatedList<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    public IReadOnlyList<T> Items { get; private set; }
    public int TotalCount { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        Items = items;
        TotalCount = count;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
    {
        return await PaginatedList<T>.CreateAsync(source, pageIndex, pageSize);
    }
}
