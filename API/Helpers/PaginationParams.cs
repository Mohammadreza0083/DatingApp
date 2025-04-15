namespace API.Helpers;

/// <summary>
/// Base class for pagination parameters with size limits
/// </summary>
public class PaginationParams
{
    /// <summary>
    /// Maximum allowed page size
    /// </summary>
    private const int MaxPageSize = 50;

    /// <summary>
    /// Gets or sets the current page number
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// Private field for page size with validation
    /// </summary>
    private int _pageSize = 10;

    /// <summary>
    /// Gets or sets the number of items per page
    /// </summary>
    /// <remarks>
    /// The value is capped at MaxPageSize if a larger value is provided
    /// </remarks>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}