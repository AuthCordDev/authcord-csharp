# AuthCord .NET SDK

> [AuthCord](https://authcord.dev) - Sell, authenticate, and manage your software. All in one place. Replace your auth system, payment platform, and Discord bots with a single dashboard.

Official AuthCord SDK for .NET 6+.

## Installation

```bash
dotnet add package AuthCord
```

Or add a project reference if building from source:

```bash
dotnet add reference path/to/AuthCord/AuthCord.csproj
```

## Usage

```csharp
using AuthCord;

using var client = new AuthCordClient("dax_your_api_key");

// Validate a user by Discord ID
var result = await client.ValidateAsync("your_app_id", discordId: "123456789", hwid: "HWID-ABC");
if (result.Valid)
{
    Console.WriteLine($"Welcome {result.User?.Username}!");
    foreach (var product in result.Products ?? new())
        Console.WriteLine($"  Product: {product.Name} (lifetime: {product.IsLifetime})");
}
else
{
    Console.WriteLine($"Access denied: {result.Reason}");
}

// Session-based validation
var session = await client.CreateSessionAsync("your_app_id", "HWID-ABC", discordId: "123456789", deviceName: "Work PC");
Console.WriteLine($"Session token: {session.SessionToken}");

var sessionResult = await client.ValidateSessionAsync(session.SessionToken, "HWID-ABC");
if (sessionResult.Valid)
    Console.WriteLine("Session valid!");

// Offline tokens
var offlineToken = await client.GetOfflineTokenAsync("your_app_id", discordId: "123456789");
Console.WriteLine($"Offline token expires: {offlineToken.ExpiresAt}");
```

## Real-time Session Kick (Heartbeat)

After `ValidateAsync()` succeeds, start a background heartbeat so an admin clicking **Terminate** in the dashboard takes effect within ~10 seconds instead of waiting for the user's next manual validate.

```csharp
using var cts = new CancellationTokenSource();

var heartbeatTask = client.StartHeartbeatAsync(
    appId: "your_app_id",
    discordId: "123456789",
    hwid: "HWID-ABC",
    onTerminated: hb =>
    {
        Console.Error.WriteLine($"Session ended: {hb.Reason}"); // "terminated", "banned", "expired", ...
        // Tear down: close windows, clear in-memory secrets, redirect to login, etc.
        Environment.Exit(0);
        return Task.CompletedTask;
    },
    // onError: ex => ...                  // optional; loop keeps running on transient errors
    // intervalSeconds: 10,                // optional; otherwise the server controls cadence
    cancellationToken: cts.Token);

// ... your app does its thing ...
cts.Cancel();              // clean shutdown on normal sign-out
await heartbeatTask;
```

For a DeviceSession-based flow, pass `sessionToken:` instead of `discordId:` + `hwid:`. Full runnable example in `examples/HeartbeatExample.cs`.

## Email-Based Validation

AuthCord supports validating users by Discord ID, user ID, or email:

```csharp
// Validate by email
var result = await client.ValidateAsync("your_app_id", email: "user@example.com", hwid: "HWID-ABC");

// Validate by custom user ID
var result2 = await client.ValidateAsync("your_app_id", userId: "user123");

// Create a session with email
var session = await client.CreateSessionAsync("your_app_id", "HWID-ABC", email: "user@example.com");

// Get offline token with email
var token = await client.GetOfflineTokenAsync("your_app_id", email: "user@example.com");
```

## Error Handling

```csharp
try
{
    var result = await client.ValidateAsync("your_app_id", discordId: "123456789");
}
catch (AuthenticationException)
{
    Console.WriteLine("Invalid API key");
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (ApiException ex)
{
    Console.WriteLine($"API error ({ex.StatusCode}): {ex.Message}");
}
```
