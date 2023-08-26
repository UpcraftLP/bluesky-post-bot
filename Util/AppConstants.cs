using System.Reflection;
using Discord;

namespace Up.Bsky.PostBot.Util;

public static class AppConstants
{
    public static readonly string AppName = Assembly.GetEntryAssembly()?.GetName().Name ?? "BlueSky Post Bot";

    public static readonly ApplicationInstallParams BotPermissions = new(new[]
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
}
