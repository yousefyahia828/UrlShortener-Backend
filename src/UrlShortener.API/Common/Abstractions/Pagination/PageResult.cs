using Microsoft.EntityFrameworkCore;

namespace UrlShortener.API.Common.Abstractions.Pagination;

public sealed class PageResult<T>
{
    private PageResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasNext => Page * PageSize < TotalCount;
    public bool HasPrevious => Page > 1;

    public static async Task<PageResult<T>> CreateAsync(
        IQueryable<T> query,
        int? page,
        int? pageSize)
    {
        ArgumentNullException.ThrowIfNull(query);

        int internalPage = Math.Max(1, page ?? 1);
        int internalPageSize = Math.Clamp(pageSize ?? 10, 1, 100);

        var pagedQuery = query
            .Select(e => new WindowedItem<T>
            {
                Item = e,
                TotalCount = query.Count()
            })
            .Skip((internalPage - 1) * internalPageSize)
            .Take(internalPageSize);


        var result = await pagedQuery.ToListAsync(); // database hit

        int totalCount = result.Count > 0 ? result[0].TotalCount : 0;

        int totalPages = totalCount > 0
            ? (int)Math.Ceiling((double)totalCount / internalPageSize)
            : 1;

        internalPage = Math.Clamp(internalPage, 1, totalPages);

        var items = result.Select(w => w.Item).ToList();

        return new(items, internalPage, internalPageSize, totalCount);
    }
}

internal sealed class WindowedItem<TItem>
{
    public TItem Item { get; init; } = default!;
    public int TotalCount { get; init; }
}