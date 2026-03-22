using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthCord;

/// <summary>
/// Official AuthCord .NET SDK client.
/// </summary>
/// <example>
/// <code>
/// using var client = new AuthCordClient("dax_your_api_key");
/// var result = await client.ValidateAsync("your_app_id", discordId: "123456789");
/// if (result.Valid)
///     Console.WriteLine($"Welcome {result.User?.Username}!");
/// </code>
/// </example>
public sealed class AuthCordClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    /// <summary>
    /// Create a new AuthCord client.
    /// </summary>
    /// <param name="apiKey">Your API key (starts with dax_).</param>
    /// <param name="baseUrl">Base URL for the AuthCord API.</param>
    /// <param name="timeout">Request timeout.</param>
    public AuthCordClient(
        string apiKey,
        string baseUrl = "https://authcord.dev",
        TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required.", nameof(apiKey));

        _baseUrl = baseUrl.TrimEnd('/');

        _httpClient = new HttpClient
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(30)
        };
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "AuthCord-CSharp-SDK/1.0.0");
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Validate a user's access to your application.
    /// At least one of discordId, userId, or email must be provided.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(
        string appId,
        string? discordId = null,
        string? userId = null,
        string? email = null,
        string? productId = null,
        string? hwid = null,
        string? ip = null,
        string? userAgent = null,
        Dictionary<string, object>? deviceMeta = null,
        string? binaryHash = null,
        string? appVersion = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(discordId) && string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("At least one of discordId, userId, or email is required.");

        var body = new Dictionary<string, object>
        {
            ["app_id"] = appId
        };
        if (discordId != null) body["discord_id"] = discordId;
        if (userId != null) body["user_id"] = userId;
        if (email != null) body["email"] = email;
        if (productId != null) body["product_id"] = productId;
        if (hwid != null) body["hwid"] = hwid;
        if (ip != null) body["ip"] = ip;
        if (userAgent != null) body["user_agent"] = userAgent;
        if (deviceMeta != null) body["device_meta"] = deviceMeta;
        if (binaryHash != null) body["binary_hash"] = binaryHash;
        if (appVersion != null) body["app_version"] = appVersion;

        return await RequestAsync<ValidationResult>(
            HttpMethod.Post, "/api/v1/auth/validate", body, cancellationToken);
    }

    /// <summary>
    /// Create a persistent device session.
    /// At least one of discordId, userId, or email must be provided.
    /// </summary>
    public async Task<SessionCreateResult> CreateSessionAsync(
        string appId,
        string hwid,
        string? discordId = null,
        string? userId = null,
        string? email = null,
        string? deviceName = null,
        Dictionary<string, object>? deviceMeta = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(discordId) && string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("At least one of discordId, userId, or email is required.");

        var body = new Dictionary<string, object>
        {
            ["app_id"] = appId,
            ["hwid"] = hwid
        };
        if (discordId != null) body["discord_id"] = discordId;
        if (userId != null) body["user_id"] = userId;
        if (email != null) body["email"] = email;
        if (deviceName != null) body["device_name"] = deviceName;
        if (deviceMeta != null) body["device_meta"] = deviceMeta;

        return await RequestAsync<SessionCreateResult>(
            HttpMethod.Post, "/api/v1/auth/sessions/create", body, cancellationToken);
    }

    /// <summary>
    /// Validate using a session token.
    /// </summary>
    public async Task<ValidationResult> ValidateSessionAsync(
        string sessionToken,
        string hwid,
        string? productId = null,
        Dictionary<string, object>? deviceMeta = null,
        string? binaryHash = null,
        string? appVersion = null,
        CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object>
        {
            ["session_token"] = sessionToken,
            ["hwid"] = hwid
        };
        if (productId != null) body["product_id"] = productId;
        if (deviceMeta != null) body["device_meta"] = deviceMeta;
        if (binaryHash != null) body["binary_hash"] = binaryHash;
        if (appVersion != null) body["app_version"] = appVersion;

        return await RequestAsync<ValidationResult>(
            HttpMethod.Post, "/api/v1/auth/sessions/validate", body, cancellationToken);
    }

    /// <summary>
    /// Revoke a specific session by token.
    /// </summary>
    public async Task<bool> RevokeSessionAsync(
        string sessionToken,
        CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object>
        {
            ["session_token"] = sessionToken
        };

        var result = await RequestAsync<RevokeResponse>(
            HttpMethod.Post, "/api/v1/auth/sessions/revoke", body, cancellationToken);
        return result.Success;
    }

    /// <summary>
    /// Revoke all sessions for a user in an app. Returns the count of sessions revoked.
    /// </summary>
    public async Task<int> RevokeAllSessionsAsync(
        string discordId,
        string appId,
        CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object>
        {
            ["discord_id"] = discordId,
            ["app_id"] = appId
        };

        var result = await RequestAsync<RevokeResponse>(
            HttpMethod.Post, "/api/v1/auth/sessions/revoke", body, cancellationToken);
        return result.Count;
    }

    /// <summary>
    /// List all sessions for a user in an app.
    /// </summary>
    public async Task<List<Session>> ListSessionsAsync(
        string discordId,
        string appId,
        CancellationToken cancellationToken = default)
    {
        var path = $"/api/v1/auth/sessions/list?discord_id={Uri.EscapeDataString(discordId)}&app_id={Uri.EscapeDataString(appId)}";
        var result = await RequestAsync<SessionListResponse>(
            HttpMethod.Get, path, null, cancellationToken);
        return result.Sessions;
    }

    /// <summary>
    /// Generate a signed offline token.
    /// At least one of discordId, userId, or email must be provided.
    /// </summary>
    public async Task<OfflineTokenResult> GetOfflineTokenAsync(
        string appId,
        string? discordId = null,
        string? userId = null,
        string? email = null,
        string? productId = null,
        string? hwid = null,
        int? ttl = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(discordId) && string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("At least one of discordId, userId, or email is required.");

        var body = new Dictionary<string, object>
        {
            ["app_id"] = appId
        };
        if (discordId != null) body["discord_id"] = discordId;
        if (userId != null) body["user_id"] = userId;
        if (email != null) body["email"] = email;
        if (productId != null) body["product_id"] = productId;
        if (hwid != null) body["hwid"] = hwid;
        if (ttl != null) body["ttl"] = ttl;

        return await RequestAsync<OfflineTokenResult>(
            HttpMethod.Post, "/api/v1/auth/offline-token", body, cancellationToken);
    }

    /// <summary>
    /// Get the public key for offline token verification.
    /// </summary>
    public async Task<PublicKeyResult> GetPublicKeyAsync(
        string appId,
        CancellationToken cancellationToken = default)
    {
        var path = $"/api/v1/auth/offline-token/public-key?app_id={Uri.EscapeDataString(appId)}";
        return await RequestAsync<PublicKeyResult>(
            HttpMethod.Get, path, null, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request, deserializes the response, and handles errors.
    /// </summary>
    private async Task<T> RequestAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        CancellationToken cancellationToken)
    {
        var url = $"{_baseUrl}{path}";

        using var request = new HttpRequestMessage(method, url);

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new AuthCordException("Request timed out.", 0, ex);
        }
        catch (HttpRequestException ex)
        {
            throw new AuthCordException($"Network error: {ex.Message}", 0, ex);
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            var errorMessage = $"HTTP {statusCode}";

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    errorMessage = msgProp.GetString() ?? errorMessage;
                else if (doc.RootElement.TryGetProperty("error", out var errProp))
                    errorMessage = errProp.GetString() ?? errorMessage;
            }
            catch (JsonException)
            {
                // Use default error message
            }

            throw statusCode switch
            {
                401 => new AuthenticationException(errorMessage),
                429 => new RateLimitException(
                    errorMessage,
                    response.Headers.TryGetValues("Retry-After", out var values)
                        ? TimeSpan.FromSeconds(int.TryParse(values.FirstOrDefault(), out var s) ? s : 60)
                        : null),
                _ => new ApiException(errorMessage, statusCode)
            };
        }

        try
        {
            return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions)
                ?? throw new AuthCordException("Failed to deserialize response.");
        }
        catch (JsonException ex)
        {
            throw new AuthCordException($"Failed to parse response: {ex.Message}", 0, ex);
        }
    }

    /// <summary>
    /// Disposes the underlying HttpClient.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
