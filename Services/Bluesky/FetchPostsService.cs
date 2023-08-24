using FishyFlip;
using FishyFlip.Models;
using FishyFlip.Tools;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Model.Bluesky;
using Up.Bsky.PostBot.Model.Discord.DTO;
using Up.Bsky.PostBot.Util.FishyFlip;

namespace Up.Bsky.PostBot.Services.Bluesky;

public interface IFetchPostsService
{
    Task<IList<FetchPostsResponse>> FetchPostsSince(BskyUser user, string? lastPostId, bool includeReplies = false, CancellationToken cancellationToken = default);
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

    public async Task<IList<FetchPostsResponse>> FetchPostsSince(BskyUser user, string? lastPostId, bool includeReplies = false, CancellationToken cancellationToken = default)
    {
        var list = new List<FetchPostsResponse>();

        string? cursor = null;
        while (true)
        {
            var result = (await _atProto.Repo.ListPostAsync(user.DidObject, 100, cursor, true, cancellationToken)).HandleResult()!;
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
                list.Add(new FetchPostsResponse(user, atUri, post.CreatedAt!.Value, post.Text, post.Embed));
            }

            cursor = result.Cursor;
        }
        End:

        return list.OrderByDescending(it => it.CreatedAt).ToList();
    }
}
