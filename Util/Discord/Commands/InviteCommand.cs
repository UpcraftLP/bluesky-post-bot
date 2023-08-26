using Discord.Interactions;

namespace Up.Bsky.PostBot.Util.Discord.Commands;

public class InviteCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("invite", "Generate an invite link for this bot")]
    public async Task GenerateInviteLink()
    {
        await RespondAsync($"https://discord.com/api/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions={(ulong) AppConstants.BotPermissions.Permission}&scope={string.Join("%20", AppConstants.BotPermissions.Scopes)}", ephemeral: true);
    }
}
