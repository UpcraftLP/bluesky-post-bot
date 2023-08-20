using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FishyFlip.Models;

namespace Up.Bsky.PostBot.Model;

public class PostEntry
{
    [Key]
    [Required]
    public string AtUri { get; set; } = null!;
    
    [Required]
    public string UserDid { get; set; } = null!;
    
    [NotMapped]
    public ATUri AtUriObject => ATUri.Create(AtUri);
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
