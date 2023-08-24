using System.Reflection;

namespace Up.Bsky.PostBot.Util;

public static class AppConstants
{
    public static readonly string AppName = Assembly.GetEntryAssembly()?.GetName().Name ?? "BlueSky Post Bot";
}
