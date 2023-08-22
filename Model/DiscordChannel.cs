using System.ComponentModel.DataAnnotations;

namespace Up.Bsky.PostBot.Model;

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
