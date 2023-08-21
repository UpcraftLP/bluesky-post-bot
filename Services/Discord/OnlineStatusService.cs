using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;

namespace Up.Bsky.PostBot.Services.Discord;

public class OnlineStatusService : DiscordClientService
{
    private readonly IServiceProvider _serviceProvider;
    
    public OnlineStatusService(DiscordSocketClient client, ILogger<OnlineStatusService> logger, IServiceProvider serviceProvider) : base(client, logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Client.CurrentUserUpdated += async (oldUser, newUser) =>
        {
            await UpdateClient(newUser);
        };

        await Client.WaitForReadyAsync(cancellationToken);
        Logger.LogInformation("Client reported ready");
        await UpdateClient(Client.CurrentUser);
        
        await Task.Delay(-1, cancellationToken);
    }

    private async Task UpdateClient(IUser user)
    {
        using var scope = _serviceProvider.CreateScope();
        Logger.LogInformation("Setting user [{Username}#{Discriminator}]", user.Username, user.Discriminator);
        var updateStatusService = scope.ServiceProvider.GetRequiredService<IUpdateStatusService>();
        await updateStatusService.UpdateStatus();
    }
}
