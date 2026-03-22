namespace AuthCord;

/// <summary>
/// Base exception for all AuthCord SDK errors.
/// </summary>
public class AuthCordException : Exception
{
    /// <summary>
    /// HTTP status code that triggered the error, or 0 if not applicable.
    /// </summary>
    public int StatusCode { get; }

    public AuthCordException(string message, int statusCode = 0)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public AuthCordException(string message, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}

/// <summary>
/// Raised when API key authentication fails (HTTP 401).
/// </summary>
public class AuthenticationException : AuthCordException
{
    public AuthenticationException(string message = "Invalid API key")
        : base(message, 401)
    {
    }
}

/// <summary>
/// Raised when the API rate limit is exceeded (HTTP 429).
/// </summary>
public class RateLimitException : AuthCordException
{
    /// <summary>
    /// How long to wait before retrying, if the server provided a Retry-After header.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    public RateLimitException(string message = "Rate limit exceeded", TimeSpan? retryAfter = null)
        : base(message, 429)
    {
        RetryAfter = retryAfter;
    }
}

/// <summary>
/// Raised when the API returns a non-success status code.
/// </summary>
public class ApiException : AuthCordException
{
    public ApiException(string message, int statusCode)
        : base(message, statusCode)
    {
    }
}

/// <summary>
/// Raised when offline token operations fail (invalid format, expired, bad signature).
/// </summary>
public class OfflineTokenException : AuthCordException
{
    public OfflineTokenException(string message)
        : base(message)
    {
    }
}
