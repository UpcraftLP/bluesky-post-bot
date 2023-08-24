﻿using Discord;
using Discord.WebSocket;
using FishyFlip;
using FishyFlip.Tools;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Model.Discord.DTO;
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
        var channels = await _dbContext.DiscordChannels.Where(it => it.TrackedUsers.Contains(post.User)).ToListAsync(cancellationToken);
        
        var userProfile = (await _atProto.Actor.GetProfileAsync(post.User.DidObject, cancellationToken)).HandleResult();
        if (userProfile == null)
        {
            throw new ApplicationException($"User profile for {post.User.DidObject.ToBskyUri()} does not exist!");
        }
        
        foreach (var channel in channels)
        {
            var embed = new EmbedBuilder()
                .WithAuthor(userProfile.DisplayName, userProfile.Avatar, post.AtUri.ToBskyUri().ToString())
                .WithDescription(post.Text)
                .WithTimestamp(post.CreatedAt)
                .Build();
            if (await _client.GetChannelAsync(channel.ChannelId) is not ITextChannel discordChannel)
            {
                _logger.LogWarning("Channel {ChannelId} for guild {GuildId} not found or is not a text channel!", channel.ChannelId, channel.ServerId);
                continue;
            }
            
            //TODO send as webhook
            await discordChannel.SendMessageAsync(embed: embed);
        }
    }
}