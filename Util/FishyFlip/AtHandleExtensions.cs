using FishyFlip;
using FishyFlip.Models;
using FishyFlip.Tools;

namespace Up.Bsky.PostBot.Util.FishyFlip;

public static class AtHandleExtensions
{
    public static async Task<ATDid> Resolve(this ATHandle self, ATProtocol atClient, CancellationToken cancellationToken = default)
    {
        return (await atClient.Identity.ResolveHandleAsync(self, cancellationToken)).HandleResult()!.Did!;
    }
}
