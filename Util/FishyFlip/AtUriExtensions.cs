using FishyFlip.Models;

namespace Up.Bsky.PostBot.Util.FishyFlip;

public static class AtUriExtensions
{
    public static Uri ToBskyUri(this ATUri self)
    {
        return new Uri($"https://bsky.app/profile/{self.Did}/post/{self.Rkey}");
    }
    
    public static string GetBskyTid(this ATUri self) => self.Rkey;
}
