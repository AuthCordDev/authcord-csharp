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
/// Structured HWID components the SDK can send alongside (or instead
/// of) an opaque HWID string. The server hashes a subset of these —
/// controlled by the app's HWID Strategy in the dashboard — into the
/// canonical HWID used for slot matching.
///
/// Typical temp HWID spoofers (used to evade FiveM-style server bans)
/// change SMBIOS UUID, disk serial, MAC, and MachineGuid — but NOT
/// the Windows User SID or CPUID. Apps using <c>STABLE</c> hash only
/// (sid + cpu_id) so users stay bound across spoofs.
///
/// On Windows, <see cref="AuthCordClient.CollectHwidComponents"/>
/// populates Sid + CpuId + MachineGuid automatically.
/// </summary>
public sealed record HwidComponents
{
    [JsonPropertyName("sid")]
    public string? Sid { get; init; }

    [JsonPropertyName("cpu_id")]
    public string? CpuId { get; init; }

    [JsonPropertyName("machine_guid")]
    public string? MachineGuid { get; init; }

    [JsonPropertyName("mac")]
    public string? Mac { get; init; }

    [JsonPropertyName("disk")]
    public string? Disk { get; init; }

    internal Dictionary<string, string> ToBody()
    {
        var d = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(Sid))          d["sid"]          = Sid!;
        if (!string.IsNullOrEmpty(CpuId))        d["cpu_id"]       = CpuId!;
        if (!string.IsNullOrEmpty(MachineGuid))  d["machine_guid"] = MachineGuid!;
        if (!string.IsNullOrEmpty(Mac))          d["mac"]          = Mac!;
        if (!string.IsNullOrEmpty(Disk))         d["disk"]         = Disk!;
        return d;
    }
}

/// <summary>
/// Result of a single heartbeat check. <c>Valid</c> is false when an admin
/// has terminated the device/session, the user has been banned/paused,
/// the product expired, or the HWID was unbound. <c>Reason</c> carries a
/// machine-readable code (e.g. <c>"terminated"</c>, <c>"banned"</c>,
/// <c>"expired"</c>) so the client can branch on it. <c>NextHeartbeatIn</c>
/// is server-controlled and the auto-heartbeat loop honours it.
/// </summary>
public sealed record HeartbeatResult
{
    [JsonPropertyName("valid")]
    public bool Valid { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("next_heartbeat_in")]
    public int NextHeartbeatIn { get; init; } = 10;
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

// ---------------------------------------------------------------------------
// Admin operation results (server-side, FULL API key only)
//
// Success is false for the expected 404 cases (carrying a machine Error code +
// human Reason) and the 409 already-paused case. Check it before reading data.
// ---------------------------------------------------------------------------

/// <summary>A product paused by <c>PauseProductAsync</c>.</summary>
public sealed record PausedProduct
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = "";

    [JsonPropertyName("paused_at")]
    public string? PausedAt { get; init; }

    [JsonPropertyName("pause_ends_at")]
    public string? PauseEndsAt { get; init; }

    [JsonPropertyName("frozen_expires_at")]
    public string? FrozenExpiresAt { get; init; }
}

/// <summary>
/// Result of a <c>PauseProductAsync</c> call. <see cref="Success"/> is false
/// for the 404 cases (machine <see cref="Error"/> + human <see cref="Reason"/>)
/// and the 409 already-paused case (Error = "already_paused" + Message).
/// </summary>
public sealed record PauseResult
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>HTTP status code of the response (not part of the JSON body).</summary>
    [JsonIgnore]
    public int Status { get; init; }

    [JsonPropertyName("paused")]
    public List<PausedProduct> Paused { get; init; } = new();

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

/// <summary>A product unpaused by <c>UnpauseProductAsync</c>.</summary>
public sealed record UnpausedProduct
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = "";

    [JsonPropertyName("new_expires_at")]
    public string? NewExpiresAt { get; init; }
}

/// <summary>Result of an <c>UnpauseProductAsync</c> call.</summary>
public sealed record UnpauseResult
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>HTTP status code of the response (not part of the JSON body).</summary>
    [JsonIgnore]
    public int Status { get; init; }

    [JsonPropertyName("unpaused")]
    public List<UnpausedProduct> Unpaused { get; init; } = new();

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

/// <summary>Per-product HWID clear count from <c>ResetHwidAsync</c>.</summary>
public sealed record HwidResetEntry
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = "";

    /// <summary>Bindings cleared. Idempotent: nothing bound reports 0.</summary>
    [JsonPropertyName("cleared_hwids")]
    public int ClearedHwids { get; init; }

    /// <summary>True when this product was skipped because it is still inside its reset cooldown.</summary>
    [JsonPropertyName("on_cooldown")]
    public bool OnCooldown { get; init; }

    /// <summary>When this product's reset cooldown ends (set when <see cref="OnCooldown"/> is true).</summary>
    [JsonPropertyName("cooldown_ends_at")]
    public DateTimeOffset? CooldownEndsAt { get; init; }
}

/// <summary>Result of a <c>ResetHwidAsync</c> call.</summary>
public sealed record ResetHwidResult
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>HTTP status code of the response (not part of the JSON body).</summary>
    [JsonIgnore]
    public int Status { get; init; }

    [JsonPropertyName("reset")]
    public List<HwidResetEntry> Reset { get; init; } = new();

    /// <summary>Soonest cooldown end among blocked products (409 <c>cooldown_active</c> only).</summary>
    [JsonPropertyName("cooldown_ends_at")]
    public DateTimeOffset? CooldownEndsAt { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}
