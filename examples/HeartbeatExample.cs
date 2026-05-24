// Real-time session kick via the heartbeat loop.
//
// When an admin clicks Terminate in the AuthCord dashboard, the user's
// client app should disconnect within ~10 seconds. This is done by
// running a background heartbeat after the initial validate call.

using AuthCord;

const string ApiKey    = "dax_your_api_key_here";
const string AppId     = "your_app_id";
const string DiscordId = "123456789012345678";
const string Hwid      = "PC-12345";

using var client = new AuthCordClient(ApiKey);

// Step 1: standard validate at startup.
var result = await client.ValidateAsync(AppId, discordId: DiscordId, hwid: Hwid);
if (!result.Valid)
{
    Console.Error.WriteLine($"Access denied: {result.Reason}");
    Environment.Exit(1);
}

Console.WriteLine($"Access granted for {result.User?.Username}");

// Step 2: start the heartbeat loop. The CancellationTokenSource lets you
// stop the loop on normal sign-out. onTerminated fires exactly once when
// the server returns Valid=false (admin clicked Terminate, user banned,
// product expired, ...) and then the task completes on its own.
using var cts = new CancellationTokenSource();

var heartbeatTask = client.StartHeartbeatAsync(
    appId: AppId,
    discordId: DiscordId,
    hwid: Hwid,
    onTerminated: hb =>
    {
        Console.Error.WriteLine($"\nSession ended by AuthCord: {hb.Reason}");
        // Tear down whatever your app is doing — close windows, clear
        // secrets in memory, redirect to login, etc.
        Environment.Exit(0);
        return Task.CompletedTask;
    },
    onError: ex =>
    {
        // Network errors are non-fatal — the loop keeps polling.
        Console.Error.WriteLine($"[heartbeat] transient error: {ex.Message}");
        return Task.CompletedTask;
    },
    cancellationToken: cts.Token);

Console.WriteLine("App running. The heartbeat will kick us off if an admin terminates the session.");

// ... your actual app does its thing here ...
// On normal sign-out: cts.Cancel(); await heartbeatTask;
await heartbeatTask;
