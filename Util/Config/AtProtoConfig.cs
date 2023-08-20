namespace Up.Bsky.PostBot.Util.Config;

[Serializable]
public class AtProtoConfig
{
    public const string SectionName = "AtProto";

    public string ServiceUrl { get; set; } = "https://bsky.social";
    
    public string LoginIdentifier { get; set; } = null!;
    
    public string LoginToken { get; set; } = null!;
}
