using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Model.Bluesky;
using Up.Bsky.PostBot.Services.Discord;
using Up.Bsky.PostBot.Util;

namespace Up.Bsky.PostBot.Services.Bluesky;

public class FetchPostsBackgroundService : DelayedService<FetchPostsBackgroundService>
{
    //TODO change to 6 hours once firehose is implemented
    private static readonly TimeSpan UpdateDelay = TimeSpan.FromMinutes(10);

    public FetchPostsBackgroundService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(serviceProvider, loggerFactory, UpdateDelay)
    {
    }
    
    protected override async Task Run(IServiceScope scope, object? state, CancellationToken cancellationToken)
    {
        var fetchService = scope.ServiceProvider.GetRequiredService<IFetchPostsService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<IPostNotificationService>();

        // only fetch users that are being tracked in at least 1 channel
        var users = await dbContext.TrackedUsers.Include(it => it.Posts).Where(u => u.TrackedInChannels.Count > 0).ToListAsync(cancellationToken);

        var posts = (await Task.WhenAll(users.Select(async user =>
        {
            var latestPost = user.Posts.MinBy(it => it.CreatedAt);
            return await fetchService.FetchPostsSince(user, latestPost?.AtUri, false, cancellationToken);
        }))).SelectMany(it => it).ToList();

        if (posts.Count == 0)
        {
            Logger.LogInformation("No new posts found since last check");
            return;
        }
        
        Logger.LogInformation("Found {Posts} new posts! Sending notifications...", posts.Count);
        
        foreach (var response in posts)
        {
            await notificationService.NotifyNewPost(response, cancellationToken);
            dbContext.SeenPosts.Update(new PostEntry
            {
                AtUri = response.AtUri.ToString(),
                CreatedAt = response.CreatedAt,
                User = response.User,
            });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Done!");

        await scope.ServiceProvider.GetRequiredService<FirehoseListenerService>().UpdateCache(scope, cancellationToken);
    }
}