using FishyFlip.Models;
using Up.Bsky.PostBot.Model.Bluesky;

namespace Up.Bsky.PostBot.Model.Discord.DTO;

public record FetchPostsResponse(BskyUser User, ATUri AtUri, DateTime CreatedAt, string? Text, Embed? Embed);
