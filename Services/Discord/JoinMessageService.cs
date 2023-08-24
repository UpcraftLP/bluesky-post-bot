using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;

namespace Up.Bsky.PostBot.Services.Discord;

public class JoinMessageService : DiscordClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _env;

    public JoinMessageService(DiscordSocketClient client, ILogger<JoinMessageService> logger, IHostEnvironment env, IServiceProvider serviceProvider) : base(client, logger)
    {
        _env = env;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.JoinedGuild += async guild =>
        {
            using var scope = _serviceProvider.CreateScope();
            Logger.LogInformation("Bot joined guild ({GuildId}): {GuildName}", guild.Id, guild.Name);
            var updateStatusService = scope.ServiceProvider.GetRequiredService<IUpdateStatusService>();
            await updateStatusService.UpdateStatus(stoppingToken);

            if (_env.IsDevelopment())
            {
                var interactionService = scope.ServiceProvider.GetRequiredService<InteractionService>();
                await interactionService.RegisterCommandsToGuildAsync(guild.Id);
            }
        };
        Client.LeftGuild += async guild =>
        {
            using var scope = _serviceProvider.CreateScope();
            Logger.LogInformation("Bot left guild ({GuildId}): {GuildName}", guild.Id, guild.Name);
            var updateStatusService = scope.ServiceProvider.GetRequiredService<IUpdateStatusService>();
            await updateStatusService.UpdateStatus(stoppingToken);
            
            //TODO remove all channels in guild from DB, remove all unused users from DB, remove all orphan posts from DB
        };
        await Task.Delay(-1, stoppingToken);
    }
}
