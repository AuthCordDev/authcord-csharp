using System.Text.Json.Serialization;

namespace AuthCord;

/// <summary>
/// Result of a user validation request.
/// </summary>
public sealed record ValidationResult
{
    [JsonPropertyName("valid")]
    public bool Valid { get; init; }

    [JsonPropertyName("mode")]
    public string? Mode { get; init; }

    [JsonPropertyName("user")]
    public UserInfo? User { get; init; }

    [JsonPropertyName("products")]
    public List<ProductInfo>? Products { get; init; }

    [JsonPropertyName("hwid_results")]
    public List<HwidResult>? HwidResults { get; init; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; init; }

    [JsonPropertyName("config")]
    public Dictionary<string, object>? Config { get; init; }

    [JsonPropertyName("entitlements")]
    public Dictionary<string, object>? Entitlements { get; init; }

    [JsonPropertyName("files")]
    public List<FileInfo>? Files { get; init; }

    [JsonPropertyName("session")]
    public SessionInfo? SessionInfo { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("banned")]
    public bool Banned { get; init; }

    [JsonPropertyName("hwid_mismatch")]
    public bool HwidMismatch { get; init; }
}

/// <summary>
/// Basic user information returned from validation.
/// </summary>
public sealed record UserInfo
{
    [JsonPropertyName("discord_id")]
    public string DiscordId { get; init; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; init; } = string.Empty;
}

/// <summary>
/// Product access information.
/// </summary>
public sealed record ProductInfo
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public string? ExpiresAt { get; init; }

    [JsonPropertyName("is_lifetime")]
    public bool IsLifetime { get; init; }

    [JsonPropertyName("hwid_status")]
    public string? HwidStatus { get; init; }
}

/// <summary>
/// Per-product HWID status.
/// </summary>
public sealed record HwidResult
{
    [JsonPropertyName("productId")]
    public string ProductId { get; init; } = string.Empty;

    [JsonPropertyName("productName")]
    public string ProductName { get; init; } = string.Empty;

    [JsonPropertyName("hwidStatus")]
    public string HwidStatus { get; init; } = string.Empty;
}

/// <summary>
/// Downloadable file information.
/// </summary>
public sealed record FileInfo
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("filename")]
    public string Filename { get; init; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("version")]
    public string? Version { get; init; }

    [JsonPropertyName("checksum")]
    public string? Checksum { get; init; }

    [JsonPropertyName("stream_only")]
    public bool StreamOnly { get; init; }
}

/// <summary>
/// Device session context returned from session validation.
/// </summary>
public sealed record SessionInfo
{
    [JsonPropertyName("device_name")]
    public string? DeviceName { get; init; }

    [JsonPropertyName("first_seen")]
    public string? FirstSeen { get; init; }

    [JsonPropertyName("last_seen")]
    public string? LastSeen { get; init; }

    [JsonPropertyName("ip")]
    public string? Ip { get; init; }

    [JsonPropertyName("user_agent")]
    public string? UserAgent { get; init; }
}

/// <summary>
/// Result of creating a new session.
/// </summary>
public sealed record SessionCreateResult
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("session_token")]
    public string SessionToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public string ExpiresAt { get; init; } = string.Empty;

    [JsonPropertyName("device_name")]
    public string? DeviceName { get; init; }
}

/// <summary>
/// Signed offline token result.
/// </summary>
public sealed record OfflineTokenResult
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

    [JsonPropertyName("payload")]
    public Dictionary<string, object>? Payload { get; init; }

    [JsonPropertyName("expires_at")]
    public string ExpiresAt { get; init; } = string.Empty;
}

/// <summary>
/// Public key for offline token verification.
/// </summary>
public sealed record PublicKeyResult
{
    [JsonPropertyName("public_key")]
    public string PublicKey { get; init; } = string.Empty;

    [JsonPropertyName("algorithm")]
    public string Algorithm { get; init; } = string.Empty;
}

/// <summary>
/// A device session entry.
/// </summary>
public sealed record Session
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("hwid")]
    public string Hwid { get; init; } = string.Empty;

    [JsonPropertyName("device_name")]
    public string? DeviceName { get; init; }

    [JsonPropertyName("ip")]
    public string? Ip { get; init; }

    [JsonPropertyName("last_used_at")]
    public string LastUsedAt { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public string ExpiresAt { get; init; } = string.Empty;

    [JsonPropertyName("revoked_at")]
    public string? RevokedAt { get; init; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; init; }
}

/// <summary>
/// Internal wrapper for session list responses.
/// </summary>
internal sealed record SessionListResponse
{
    [JsonPropertyName("sessions")]
    public List<Session> Sessions { get; init; } = new();
}

/// <summary>
/// Internal wrapper for revoke responses.
/// </summary>
internal sealed record RevokeResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("count")]
    public int Count { get; init; }
}
