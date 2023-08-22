using Discord;
using Discord.Interactions;
using FishyFlip;
using FishyFlip.Models;
using FishyFlip.Tools;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Model;

namespace Up.Bsky.PostBot.Util.Discord.Commands;

public class FollowCommand: InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ATProtocol _atClient;

    public FollowCommand(IServiceProvider serviceProvider, ATProtocol atClient)
    {
        _serviceProvider = serviceProvider;
        _atClient = atClient;
    }

    [SlashCommand("follow", "Follow a bluesky user")]
    [DefaultMemberPermissions(GuildPermission.ManageWebhooks)]
    public async Task StartTrackingAsync(string blueskyUser, ITextChannel? channel = null)
    {
        if (channel == null && Context.Channel.GetChannelType()! != ChannelType.Text)
        {
            await RespondAsync("You must execute this command in a text channel or manually specify a target channel!", ephemeral: true);
            return;
        }
        var targetChannel = channel ?? (ITextChannel) Context.Channel;
        if (targetChannel.GuildId != Context.Guild.Id)
        {
            await RespondAsync("You can only target channels in this server!", ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true);
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = ATIdentifier.Create(blueskyUser);
        if (id == null)
        {
            await FollowupAsync($"Not a valid ATProtocol identifier: {blueskyUser}", ephemeral: true);
            return;
        }

        var result = (await _atClient.Actor.GetProfileAsync(id)).HandleResult();
        if (result == null)
        {
            await FollowupAsync($"Not a valid bluesky user: {blueskyUser}", ephemeral: true);
            return;
        }

        var channelDbObject = await dbContext.DiscordChannels.Include(discordChannel => discordChannel.TrackedUsers)
            .FirstOrDefaultAsync(it => it.ChannelId == targetChannel.Id) ?? new DiscordChannel
        {
            ChannelId = targetChannel.Id,
            ServerId = targetChannel.GuildId,
        };
        if (channelDbObject.TrackedUsers.Any(u => u.Did == result.Did.ToString()))
        {
            await FollowupAsync($"Already subscribed to [@{result.Handle}](https://bsky.app/profile/{result.Did}) in {targetChannel.Mention}", ephemeral: true);
            return;
        }

        var user = await dbContext.TrackedUsers.FirstOrDefaultAsync(it => it.Did == result.Did.ToString()) ?? new BskyUser
        {
            Did = result.Did.ToString(),
        };

        channelDbObject.TrackedUsers.Add(user);

        dbContext.DiscordChannels.Update(channelDbObject);
        await dbContext.SaveChangesAsync();

        await FollowupAsync($"Successfully subscribed to [@{result.Handle}](https://bsky.app/profile/{result.Did}), events will be sent to {targetChannel.Mention}", ephemeral: true);
    }
}
