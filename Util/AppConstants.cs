using System.Reflection;

namespace Up.Bsky.PostBot.Util;

public static class AppConstants
{
    public static readonly string AppName = Assembly.GetEntryAssembly()?.GetName().FullName ?? "BlueSky Post Bot";
}
