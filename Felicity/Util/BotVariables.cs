using System.Diagnostics;
using Discord.WebSocket;

namespace Felicity.Util;

public static class BotVariables
{
    // TODO: move these to settings
    public const ulong BotOwnerId = 684854397871849482;
    public const ulong CpChannelId = 973173481162285106;
    public const string DiscordInvite = "https://discord.gg/JBBqF6Pw2z";
    public const string BungieBaseUrl = "https://www.bungie.net/";

    internal const string ErrorMessage = $"You can report this error either in our [Support Server]({DiscordInvite}) " +
                                         "or by creating a new [Issue](https://github.com/axsLeaf/FelicityOne/issues/new?assignees=axsLeaf&labels=bug&template=bug-report.md&title=) on GitHub.";

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
            using var httpClient = new HttpClient();
            var s = await httpClient.GetStringAsync(
                "https://raw.githubusercontent.com/axsLeaf/FelicityOne/main/CHANGELOG.md");
            Version = s.Split("## [")[1].Split("]")[0];
        }
    }

    public static class Images
    {
        public const string ModVendorIcon =
            "https://bungie.net/common/destiny2_content/icons/23599621d4c63076c647384028d96ca4.png";

        public const string XurVendorLogo =
            "https://bungie.net/img/destiny_content/vendor/icons/xur_large_icon.png";

        public const string SaintVendorLogo =
            "https://bungie.net/common/destiny2_content/icons/c3cb40c2b36cccd2f6cf462f14c89736.png";

        public const string SadFace = "https://cdn.tryfelicity.one/images/peepoSad.png";

        public const string FelicityCircle = "https://cdn.tryfelicity.one/images/felicity_circle.jpg";

        public const string FelicitySquare = "https://cdn.tryfelicity.one/images/felicity.jpg";

        public const string DungeonIcon =
            "https://bungie.net/common/destiny2_content/icons/4456b756d5b28e38a7c905fd68e557b7.png";

        public const string RaidIcon =
            "https://bungie.net/common/destiny2_content/icons/8b1bfd1c1ce1cab51d23c78235a6e067.png";

        public const string JoaquinAvatar = "https://cdn.tryfelicity.one/images/joaquin-avatar.png";

        public const string GunsmithVendorLogo =
            "https://www.bungie.net/common/destiny2_content/icons/5fb7fa47a8f1dd04538017d289f4f910.png";
    }
}