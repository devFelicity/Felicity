using System.Diagnostics;
using Discord.WebSocket;

namespace Felicity.Util;

public static class BotVariables
{
    // TODO: move these to settings
    public const ulong BotOwnerId = 684854397871849482;
    public const string DiscordInvite = "https://discord.gg/JBBqF6Pw2z";
    public const string BungieBaseUrl = "https://www.bungie.net/";

    internal const string ErrorMessage =
        $"You can report this error either in our [Support Server]({DiscordInvite}) " +
        "or by creating a new [Issue](https://github.com/devFelicity/Bot-Frontend/issues/new?assignees=MoonieGZ&labels=bug&template=bug-report.md&title=) on GitHub.";

    internal static bool IsDebug;
    internal static string? Version;

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
            var s = await HttpClientInstance.Instance.GetStringAsync(
                "https://raw.githubusercontent.com/devFelicity/Bot-Frontend/main/CHANGELOG.md");
            Version = s.Split("# Version: v")[1].Split(" (")[0];
        }
    }

    public static class Images
    {
        public const string AdaVendorLogo = "https://cdn.tryfelicity.one/bungie_assets/VendorAda.png";
        public const string DungeonIcon = "https://cdn.tryfelicity.one/bungie_assets/Dungeon.png";
        public const string FelicityCircle = "https://cdn.tryfelicity.one/images/profile/candle-circle.png";
        public const string FelicitySquare = "https://cdn.tryfelicity.one/images/profile/candle.png";
        public const string GunsmithVendorLogo = "https://cdn.tryfelicity.one/bungie_assets/VendorGunsmith.png";
        public const string JoaquinAvatar = "https://cdn.tryfelicity.one/images/joaquin-avatar.png";
        public const string ModVendorIcon = "https://cdn.tryfelicity.one/bungie_assets/ModVendor.png";
        public const string RaidIcon = "https://cdn.tryfelicity.one/bungie_assets/Raid.png";
        public const string SadFace = "https://cdn.tryfelicity.one/images/peepoSad.png";
        public const string SaintVendorLogo = "https://cdn.tryfelicity.one/bungie_assets/VendorSaint.png";
        public const string XurVendorLogo = "https://cdn.tryfelicity.one/bungie_assets/VendorXur.png";
    }
}
