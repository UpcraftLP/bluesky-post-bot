using FishyFlip;
using FishyFlip.Models;
using FishyFlip.Tools;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Util.FishyFlip;

namespace Up.Bsky.PostBot.Services.Bluesky;

public interface IFetchPostsService
{
    Task<IList<PostResponse>> FetchPostsSince(ATIdentifier userId, string? lastPostId, bool includeReplies = false, CancellationToken cancellationToken = default);
}

public class FetchPostsService : IFetchPostsService
{
    private readonly ATProtocol _atProto;
    private readonly AppDbContext _dbContext;

    public FetchPostsService(ATProtocol atProto, AppDbContext dbContext)
    {
        _atProto = atProto;
        _dbContext = dbContext;
    }

    public async Task<IList<PostResponse>> FetchPostsSince(ATIdentifier userId, string? lastPostId, bool includeReplies = false, CancellationToken cancellationToken = default)
    {
        var did = await userId.AsDid(_atProto, cancellationToken);

        var list = new List<PostResponse>();

        string? cursor = null;
        while (true)
        {
            var result = (await _atProto.Repo.ListPostAsync(did, 100, cursor, true, cancellationToken)).HandleResult()!;
            if (result.Records.Length == 0)
            {
                goto End;
            }
            foreach (
                var record in result.Records.Select(it => new {AtUri = it.Uri!, Post = (Post) it.Value!})
                    .Where(it => includeReplies || it.Post.Reply == null)
                    .OrderByDescending(it => it.Post.CreatedAt!.Value)
            )
            {
                var post = record.Post;
                var atUri = record.AtUri;

                if (atUri.Rkey == lastPostId)
                {
                    goto End;
                }

                if (await _dbContext.SeenPosts.AnyAsync(it => it.AtUri == atUri.ToString(), cancellationToken))
                {
                    continue;
                }

                Console.WriteLine("New Post!");
                Console.WriteLine($"At: {atUri}");
                Console.WriteLine($"Url: {atUri.ToBskyUri()}");
                Console.WriteLine($"Created: {post.CreatedAt}");
                Console.WriteLine($"{post.Text}");
                if (post.Facets != null)
                {
                    foreach (var postFacet in post.Facets)
                    {
                        Console.WriteLine($"Facet: {postFacet}");
                    }
                }
                if (post.Embed != null)
                {
                    Console.WriteLine($"Embed: {post.Embed}");
                }
                Console.WriteLine("----------------------------------");
                Console.WriteLine();
                Console.WriteLine();

                list.Add(new PostResponse(atUri, post.CreatedAt!.Value, post.Text, post.Embed));
            }

            cursor = result.Cursor;
        }
        End:

        return list.OrderByDescending(it => it.CreatedAt).ToList();
    }
}

public record PostResponse(ATUri AtUri, DateTime CreatedAt, string? Text, Embed? Embed)
{
    //TODO handle embeds etc
}
