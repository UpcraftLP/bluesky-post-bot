using FishyFlip;
using FishyFlip.Events;
using FishyFlip.Models;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Model.Bluesky;
using Up.Bsky.PostBot.Model.Discord.DTO;
using Up.Bsky.PostBot.Services.Discord;
using Up.Bsky.PostBot.Util.FishyFlip;

namespace Up.Bsky.PostBot.Services.Bluesky;

public class FirehoseListenerService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FirehoseListenerService> _logger;

    private readonly ISet<string> _userCache = new HashSet<string>();

    public FirehoseListenerService(IServiceProvider serviceProvider, ILogger<FirehoseListenerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

    }

    public void AddUser(ATDid did)
    {
        lock (_userCache)
        {
            _userCache.Add(did.ToString());
        }
    }

    public void RemoveUser(ATDid did)
    {
        lock (_userCache)
        {
            _userCache.Remove(did.ToString());
        }
    }

    private bool TrackingUser(ATDid did)
    {
        lock (_userCache)
        {
            return _userCache.Contains(did.ToString());
        }
    }

    public async Task UpdateCache(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        lock (_userCache)
        {
            _userCache.Clear();
        }

        await dbContext.TrackedUsers.ForEachAsync(user => AddUser(user.DidObject), cancellationToken);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var atProto = scope.ServiceProvider.GetRequiredService<ATProtocol>();

        await UpdateCache(scope, cancellationToken);

        atProto.OnSubscribedRepoMessage += HandleRepoMessage;

        await atProto.StartSubscribeReposAsync(cancellationToken);
    }

    private async void HandleRepoMessage(object? sender, SubscribedRepoEventArgs args)
    {
        if (args.Message.Record?.Type != "app.bsky.feed.post")
        {
            return;
        }

        var userDid = args.Message.Commit!.Repo!;
        if (!TrackingUser(userDid))
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<IPostNotificationService>();

        var user = await dbContext.TrackedUsers.FirstOrDefaultAsync(it => it.Did == userDid.ToString());
        if (user == null)
        {
            _logger.LogWarning("User not in database: {User}, skipping...", userDid);
            RemoveUser(userDid);
            return;
        }

        foreach (var op in args.Message.Commit!.Ops!.Where(it => it.Action == "create"))
        {
            var post = args.Message.Record as Post;
            var atUri = userDid.WithPath(op.Path!);

            if (await dbContext.SeenPosts.AnyAsync(it => it.AtUri == atUri.ToString()))
            {
                continue;
            }

            var response = new FetchPostsResponse(user, atUri, post!.CreatedAt!.GetValueOrDefault(DateTime.UtcNow).ToUniversalTime(), post.Text, post.Embed);
            await notificationService.NotifyNewPost(response);

            dbContext.SeenPosts.Update(new PostEntry
            {
                AtUri = response.AtUri.ToString(),
                CreatedAt = response.CreatedAt,
                User = response.User,
            });
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var atProto = scope.ServiceProvider.GetRequiredService<ATProtocol>();
        atProto.OnSubscribedRepoMessage -= HandleRepoMessage;
        await atProto.StopSubscriptionAsync(cancellationToken);
    }
}
