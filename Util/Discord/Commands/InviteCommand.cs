using Discord;
using Discord.Interactions;

namespace Up.Bsky.PostBot.Util.Discord.Commands;

public class InviteCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("invite", "Generate an invite link for this bot")]
    public async Task GenerateInviteLink()
    {
        var requiredPermissions = new ApplicationInstallParams(
            new[]
            {
                "bot",
                "applications.commands",
            },
            GuildPermission.ManageWebhooks
            | GuildPermission.SendMessages
            | GuildPermission.EmbedLinks
            | GuildPermission.AttachFiles
            | GuildPermission.MentionEveryone
            | GuildPermission.UseExternalEmojis
            | GuildPermission.AddReactions
        );

        await RespondAsync($"https://discord.com/api/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions={requiredPermissions.Permission}&scope={string.Join("%20", requiredPermissions.Scopes)}", ephemeral: true);
    }
}
