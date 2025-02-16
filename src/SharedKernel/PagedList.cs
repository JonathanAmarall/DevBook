namespace SharedKernel;

public sealed record PagedList<T>
{
    public PagedList(IEnumerable<T> items, short pageNumber, short pageSize, long totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items.ToList();
    }

    /// <summary>
    /// Gets the current page.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the page size. The maximum page size is 100.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public long TotalCount { get; }

    /// <summary>
    /// Gets the flag indicating whether the next page exists.
    /// </summary>
    public bool HasNextPage => PageNumber * PageSize < TotalCount;

    /// <summary>
    /// Gets the flag indicating whether the previous page exists.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }
}

public static class PagedListExtensions
{
    public static PagedList<T> ToPagedList<T>(
        this IQueryable<T> query,
        short pageNumber,
        short pageSize)
    {
        int totalCount = query.Count();
        var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }
}
