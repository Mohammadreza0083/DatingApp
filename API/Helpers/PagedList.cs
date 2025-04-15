using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

/// <summary>
/// Generic class for handling paginated data
/// </summary>
/// <typeparam name="T">The type of items in the list</typeparam>
public class PagedList<T> : List<T>
{
    /// <summary>
    /// Initializes a new instance of the PagedList class
    /// </summary>
    /// <param name="items">The items to include in the current page</param>
    /// <param name="pageNumber">The current page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="count">The total number of items</param>
    public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int count)
    {
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    /// <summary>
    /// Gets or sets the current page number
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of items
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Creates a new PagedList asynchronously from an IQueryable source
    /// </summary>
    /// <param name="source">The source queryable</param>
    /// <param name="pageNumber">The current page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A new PagedList containing the paginated data</returns>
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, pageNumber, pageSize, count);
    }
}