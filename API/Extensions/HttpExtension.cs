using System.Text.Json;
using API.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace API.Extensions;

/// <summary>
/// Extension methods for HTTP response handling
/// </summary>
public static class HttpExtension
{
    /// <summary>
    /// Adds pagination headers to the HTTP response
    /// </summary>
    /// <typeparam name="T">The type of data being paginated</typeparam>
    /// <param name="response">The HTTP response to add headers to</param>
    /// <param name="data">The paginated data containing pagination information</param>
    public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
    {
        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize,
            data.TotalCount, data.TotalPages);
        var jsonOption = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        
        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOption));
        
        response.Headers.Append("Access-Control-Expose-Header", "Pagination");
    }
}