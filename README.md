# AuthCord .NET SDK

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
