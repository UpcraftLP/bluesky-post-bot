using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Model.Bluesky;
using Up.Bsky.PostBot.Model.Discord;

namespace Up.Bsky.PostBot.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<PostEntry> SeenPosts { get; set; } = null!;
    public DbSet<BskyUser> TrackedUsers { get; set; } = null!;
    public DbSet<DiscordChannel> DiscordChannels { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostEntry>().HasIndex(it => it.AtUri).IsUnique();
        modelBuilder.Entity<PostEntry>().HasIndex(it => it.UserDid);
        modelBuilder.Entity<PostEntry>().HasOne(it => it.User).WithMany(it => it.Posts).HasForeignKey(it => it.UserDid).HasPrincipalKey(it => it.Did);
        
        modelBuilder.Entity<BskyUser>().HasIndex(it => it.Did).IsUnique();
        
        modelBuilder.Entity<DiscordChannel>().HasIndex(it => it.ChannelId).IsUnique();
        modelBuilder.Entity<DiscordChannel>().HasMany(it => it.TrackedUsers).WithMany();
    }
}
