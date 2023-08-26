using Discord;
using Discord.Addons.Hosting.Util;
using Discord.Webhook;
using Discord.WebSocket;
using FishyFlip;
using FishyFlip.Tools;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Model.Discord.DTO;
using Up.Bsky.PostBot.Util;
using Up.Bsky.PostBot.Util.Discord;
using Up.Bsky.PostBot.Util.FishyFlip;

namespace Up.Bsky.PostBot.Services.Discord;

public interface IPostNotificationService
{
    public Task NotifyNewPost(FetchPostsResponse post, CancellationToken cancellationToken = default);
}

public class PostNotificationService : IPostNotificationService
{
    private readonly DiscordSocketClient _client;
    private readonly AppDbContext _dbContext;
    private readonly ATProtocol _atProto;
    private readonly ILogger<PostNotificationService> _logger;
    
    public PostNotificationService(DiscordSocketClient client, AppDbContext dbContext, ATProtocol atProto, ILogger<PostNotificationService> logger)
    {
        _client = client;
        _dbContext = dbContext;
        _atProto = atProto;
        _logger = logger;
    }

    public async Task NotifyNewPost(FetchPostsResponse post, CancellationToken cancellationToken = default)
    {
        await _client.WaitForReadyAsync(cancellationToken);
        var channels = await _dbContext.DiscordChannels.Where(it => it.TrackedUsers.Contains(post.User)).ToListAsync(cancellationToken);
        
        var userProfile = (await _atProto.Actor.GetProfileAsync(post.User.DidObject, cancellationToken)).HandleResult();
        if (userProfile == null)
        {
            throw new ApplicationException($"User profile for {post.User.DidObject.ToBskyUri()} does not exist!");
        }

        var authorName = userProfile.DisplayName;
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(authorName, userProfile.Avatar, post.AtUri.ToBskyUri().ToString())
            .WithTimestamp(post.CreatedAt)
            .PoweredBy();
        
        if (post.Text != null)
        {
            embedBuilder.WithDescription(post.Text);
        }

        if (post.Embed != null)
        {
            //TODO handle embeds
            Console.WriteLine($"Embed: {post.Embed}");
        }

        var embed = embedBuilder.Build();
        
        foreach (var channel in channels)
        {
            if (await _client.GetChannelAsync(channel.ChannelId) is not ITextChannel discordChannel)
            {
                _logger.LogWarning("Channel {ChannelId} for guild {GuildId} not found or is not a text channel!", channel.ChannelId, channel.ServerId);
                continue;
            }
            
            // make sure webhook exists
            var webhook = channel.WebhookId == 0 ? null : await discordChannel.GetWebhookAsync(channel.WebhookId);
            if (webhook == null)
            {
                _logger.LogDebug("Webhook for channel {ChannelId} not found, creating new webhook...", channel.ChannelId);
                webhook = await discordChannel.CreateWebhookAsync(AppConstants.AppName);
                channel.WebhookId = webhook.Id;
                _dbContext.Update(channel);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            using var webhookClient = new DiscordWebhookClient(webhook);
            await webhookClient.SendMessageAsync(username: authorName, embeds: new[] {embed}, avatarUrl: userProfile.Avatar);
        }
    }
}
