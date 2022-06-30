using System.Diagnostics;
using Discord.WebSocket;

namespace Felicity.Util;

public static class BotVariables
{
    public const ulong BotOwnerId = 684854397871849482;
    public const string BungieBaseUrl = "https://www.bungie.net/";
    public const string DiscordInvite = "https://discord.gg/JBBqF6Pw2z";

    internal static string? Version;
    internal static bool IsDebug;

    public static SocketTextChannel? DiscordLogChannel { get; set; }
    
    public static async Task Initialize()
    {
        if (Debugger.IsAttached)
        {
            IsDebug = true;
            Version = "dev-env";
        }
        else
        {
            using var httpClient = new HttpClient();
            var s = await httpClient.GetStringAsync(
                "https://raw.githubusercontent.com/axsLeaf/FelicityOne/main/CHANGELOG.md");
            Version = s.Split("## [")[1].Split("]")[0];
        }
    }
}