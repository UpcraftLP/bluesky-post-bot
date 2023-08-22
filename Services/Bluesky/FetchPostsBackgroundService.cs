using FishyFlip;
using FishyFlip.Models;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Util;
using Up.Bsky.PostBot.Util.FishyFlip;

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

        var handle = ATHandle.Create("cammiescorner.dev")!;
        var did = await handle.Resolve(atProto, cancellationToken);

        var latestPost = await dbContext.SeenPosts.Where(it => it.UserDid == did.ToString()).OrderBy(it => it.CreatedAt).FirstOrDefaultAsync(cancellationToken);

        var posts = await fetchService.FetchPostsSince(did, latestPost?.AtUri, false, cancellationToken);

        Console.WriteLine($"Found {posts.Count} new posts!");
    }
}