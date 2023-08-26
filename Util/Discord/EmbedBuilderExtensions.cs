using Discord;

namespace Up.Bsky.PostBot.Util.Discord;

public static class EmbedBuilderExtensions
{
    public static EmbedBuilder PoweredBy(this EmbedBuilder self)
    {
        return self.WithFooter(new EmbedFooterBuilder().PoweredBy());
    }
}
