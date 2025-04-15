namespace API.Helpers;

/// <summary>
/// Represents pagination metadata for API responses
/// </summary>
public class PaginationHeader(int currentPage, int itemsPerPage,
    int totalItems, int totalPages)
{
    /// <summary>
    /// Gets or sets the current page number
    /// </summary>
    public int CurrentPage { get; set; } = currentPage;
    
    /// <summary>
    /// Gets or sets the number of items per page
    /// </summary>
    public int ItemsPerPage { get; set; } = itemsPerPage;
    
    /// <summary>
    /// Gets or sets the total number of items
    /// </summary>
    public int TotalItems { get; set; } = totalItems;

    /// <summary>
    /// Gets or sets the total number of pages
    /// </summary>
    public int TotalPages { get; set; } = totalPages;
}