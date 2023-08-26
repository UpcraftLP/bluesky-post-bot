using Discord;

namespace Up.Bsky.PostBot.Util.Discord;

public static class EmbedFooterBuilderExtensions
{
    public static EmbedFooterBuilder PoweredBy(this EmbedFooterBuilder self)
    {
        return self.WithText($"Powered by {AppConstants.AppName}");
    }
}
