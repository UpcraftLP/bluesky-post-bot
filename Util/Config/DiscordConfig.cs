namespace Up.Bsky.PostBot.Util.Config;

[Serializable]
public class DiscordConfig
{
public const string SectionName = "Discord";

    public string Token { get; set; } = null!;
}
