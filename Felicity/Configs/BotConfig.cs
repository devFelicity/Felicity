// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Felicity.Configs;

public class BotConfig
{
    public BotSettings Settings { get; set; } = new();
}

public class BotSettings
{
    public string DiscordToken { get; set; } = "";
    public string BungieApiKey { get; set; } = "";
    public string BungieClientId { get; set; } = "";
    public string BungieClientSecret { get; set; } = "";
    public string TwitchAccessToken { get; set; } = "";
    public string TwitchClientId { get; set; } = "";
    public string PfxSecret { get; set; } = "";
    public int TimeBetweenRefresh { get; set; } = 5;
    public double Version { get; set; } = 4.0;
    public string CommandPrefix { get; set; } = "f!";
    public string Note { get; set; } = "Hello World";
    public int DurationToWaitForNextMessage { get; set; } = 20;
    public ulong[] BotStaff { get; set; }
    public ulong[] BotSupporters { get; set; }
    public ulong[] BannedUsers { get; set; }
    public ulong ManagementChannel { get; set; }
    public EmbedColorGroup EmbedColor { get; set; } = new();
}

public class EmbedColorGroup
{
    public int R { get; set; } = 255;
    public int G { get; set; } = 105;
    public int B { get; set; } = 180;
}