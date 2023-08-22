using FishyFlip;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Util;

namespace Up.Bsky.PostBot.Services.Bluesky;

public class FetchPostsBackgroundService : DelayedService<FetchPostsBackgroundService>
{
    private static readonly TimeSpan UpdateDelay = TimeSpan.FromMinutes(5);

    public FetchPostsBackgroundService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(serviceProvider, loggerFactory, UpdateDelay)
    {
    }
    protected override async Task Run(IServiceScope scope, object? state, CancellationToken cancellationToken)
    {
        var fetchService = scope.ServiceProvider.GetRequiredService<IFetchPostsService>();
        var atProto = scope.ServiceProvider.GetRequiredService<ATProtocol>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var users = await dbContext.TrackedUsers.Include(it => it.Posts).ToListAsync(cancellationToken);
        
        foreach (var user in users)
        {
            var latestPost = user.Posts.MinBy(it => it.CreatedAt);
            var posts = await fetchService.FetchPostsSince(user.DidObject, latestPost?.AtUri, false, cancellationToken);
            
            Console.WriteLine($"Found {posts.Count} new posts!");
        }
    }
}