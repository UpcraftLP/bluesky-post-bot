using FishyFlip;
using FishyFlip.Models;

namespace Up.Bsky.PostBot.Util.FishyFlip;

public static class AtIdentifierExtensions
{
    public static async Task<ATDid> AsDid(this ATIdentifier self, ATProtocol atClient, CancellationToken cancellationToken = default)
    {
        return self switch
        {
            ATDid did => did,
            ATHandle handle => await handle.Resolve(atClient, cancellationToken),
            
            // should never happen.
            var _ => throw new ArgumentException($"Unexpected type: {self.GetType().Name}"),
        };
    }

    public static Uri ToBskyUri(this ATIdentifier self)
    {
        return new Uri($"https://bsky.app/profile/{self}");
    }
}
