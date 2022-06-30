﻿using System.Diagnostics;
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

    public class Images
    {
        public const string ModVendorIcon =
            "https://bungie.net/common/destiny2_content/icons/23599621d4c63076c647384028d96ca4.png";

        public const string XurVendorLogo =
            "https://bungie.net/img/destiny_content/vendor/icons/xur_large_icon.png";

        public const string SaintVendorLogo =
            "https://bungie.net/common/destiny2_content/icons/c3cb40c2b36cccd2f6cf462f14c89736.png";
    }
}