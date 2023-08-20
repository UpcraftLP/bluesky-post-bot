using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Model;

namespace Up.Bsky.PostBot.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<PostEntry> SeenPosts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostEntry>().HasIndex(it => it.UserDid);
    }
}
