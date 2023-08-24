using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;

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

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var trackedChannels = await dbContext.DiscordChannels.Where(it => it.ServerId == guild.Id).ToListAsync(stoppingToken);
            dbContext.DiscordChannels.RemoveRange(trackedChannels);

            var users = await dbContext.TrackedUsers.Where(it => it.TrackedInChannels.Count == 0).Include(bskyUser => bskyUser.Posts).ToListAsync(stoppingToken);
            var posts = users.SelectMany(it => it.Posts).ToList();
            dbContext.RemoveRange(posts);
            dbContext.TrackedUsers.RemoveRange(users);
            
            Logger.LogInformation("Removing {UserCount} users with {PostCount} posts from database", users.Count, posts.Count);
            
            await dbContext.SaveChangesAsync(stoppingToken);
        };
        await Task.Delay(-1, stoppingToken);
    }
}
