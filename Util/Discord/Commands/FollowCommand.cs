using Discord;
using Discord.Interactions;
using FishyFlip;
using FishyFlip.Models;
using FishyFlip.Tools;

namespace Up.Bsky.PostBot.Util.Discord.Commands;

public class FollowCommand: InteractionModuleBase<SocketInteractionContext>
{
    private readonly ATProtocol _atClient;
    
    public FollowCommand(ATProtocol atClient)
    {
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
        await DeferAsync(ephemeral: true);

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
        var handle = result.Handle;
        var targetChannel = channel ?? (ITextChannel) Context.Channel;
        
        
        
        await FollowupAsync($"Successfully subscribed to [@{handle}](https://bsky.app/profile/{result.Did}), events will be sent to {targetChannel.Mention}", ephemeral: true);
    }
}
