using Discord;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;

namespace Up.Bsky.PostBot.Services.Discord;

public interface IUpdateStatusService
{
    public Task UpdateStatus(CancellationToken cancellationToken = default);
}

public class UpdateStatusService : IUpdateStatusService
{
    private readonly DiscordSocketClient _client;

    public UpdateStatusService(DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task UpdateStatus(CancellationToken cancellationToken = default)
    {
        await _client.WaitForReadyAsync(cancellationToken);
        await _client.SetStatusAsync(UserStatus.Online);
        var servers = _client.Guilds.Count == 1 ? "server" : "servers";
        if (_client.Guilds.Count > 0)
        {
            await _client.SetActivityAsync(new Game($"{_client.Guilds.Count} {servers}", ActivityType.Watching));
        }
        else
        {
            await _client.SetActivityAsync(null);
        }
    }
}
