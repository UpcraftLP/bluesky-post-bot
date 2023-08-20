using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;

namespace Up.Bsky.PostBot.Services.Discord;

public class OnlineStatusService : DiscordClientService
{
    public OnlineStatusService(DiscordSocketClient client, ILogger<OnlineStatusService> logger) : base(client, logger)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Client.CurrentUserUpdated += (oldUser, newUser) =>
        {
            Logger.LogInformation("Setting user [{Username}#{Discriminator}]", newUser.Username, newUser.Discriminator);
            return Task.CompletedTask;
        };

        await Client.WaitForReadyAsync(cancellationToken);
        Logger.LogInformation("Client reported ready - Current user [{Name}#{Discriminator}]", Client.CurrentUser.Username, Client.CurrentUser.Discriminator);
        await Client.SetStatusAsync(UserStatus.Online);
    }
}
