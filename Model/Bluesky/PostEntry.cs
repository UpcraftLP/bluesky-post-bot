using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FishyFlip.Models;

namespace Up.Bsky.PostBot.Model.Bluesky;

public class PostEntry
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string AtUri { get; set; } = null!;
    
    [Required]
    public string UserDid { get; set; } = null!;
    
    public virtual BskyUser User { get; set; } = null!;
    
    [NotMapped]
    public ATUri AtUriObject => ATUri.Create(AtUri);
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
