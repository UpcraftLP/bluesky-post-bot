using System.ComponentModel.DataAnnotations;
using Up.Bsky.PostBot.Model.Bluesky;

namespace Up.Bsky.PostBot.Model.Discord;

public class DiscordChannel
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public ulong ChannelId { get; set; }
    
    [Required]
    public ulong ServerId { get; set; }
    
    public virtual ICollection<BskyUser> TrackedUsers { get; set; } = new List<BskyUser>();
}
