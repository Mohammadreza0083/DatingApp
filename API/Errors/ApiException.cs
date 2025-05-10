namespace API.Errors;

/// <summary>
/// Custom exception class for API errors with status code and details
/// </summary>
public class ApiException(int statusCode, string message, string? details)
{
    /// <summary>
    /// Gets or sets the HTTP status code for the error
    /// </summary>
    public int StatusCode { get; set; } = statusCode;

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string Message { get; set; } = message;

    /// <summary>
    /// Gets or sets additional error details
    /// </summary>
    public string? Details { get; set; } = details;
}
