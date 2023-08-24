using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FishyFlip.Models;

namespace Up.Bsky.PostBot.Model.Bluesky;

public class BskyUser
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Did { get; set; } = null!;
    
    public virtual ICollection<PostEntry> Posts { get; set; } = new List<PostEntry>();
    
    [NotMapped]
    public ATDid DidObject => ATDid.Create(Did)!;
}
