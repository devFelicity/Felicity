using Newtonsoft.Json;

namespace FelicityOne.Configs;

using J = JsonPropertyAttribute;

public class BotConfig
{
    [J("Settings")] public BotSettings Settings { get; set; } = new();
}

public class BotSettings
{
    [J("DiscordToken")] public string DiscordToken { get; set; } = "";
    [J("BungieApiKey")] public string BungieApiKey { get; set; } = "";
    [J("BungieClientId")] public string BungieClientId { get; set; } = "";
    [J("BungieClientSecret")] public string BungieClientSecret { get; set; } = "";
    [J("TwitchAccessToken")] public string TwitchAccessToken { get; set; } = "";
    [J("TwitchClientId")] public string TwitchClientId { get; set; } = "";

    [J("SentryDsn")]
    public string SentryDsn { get; set; } =
        "https://0d87dc97aaab4d4e867a93831677b90e@o1264277.ingest.sentry.io/6446823";

    [J("PfxSecret")] public string PfxSecret { get; set; } = "";
    [J("Version")] public string Version { get; set; } = "5.0.0";
    [J("CommandPrefix")] public string CommandPrefix { get; set; } = "f!";

    [J("BotStaff")]
    public List<User> BotStaff { get; set; } = new() {new User {Id = 684854397871849482, Name = "Leaf"}};

    [J("BotSupporters")] public List<User> BotSupporters { get; set; } = new();
    [J("BannedUsers")] public List<User> BannedUsers { get; set; } = new();
    [J("ManagementChannel")] public ulong ManagementChannel { get; set; } = 960484928393465885;
    [J("CheckpointChannel")] public ulong CheckpointChannel { get; set; } = 973173481162285106;
    [J("EmbedColor")] public EmbedColor EmbedColor { get; set; } = new();
}

public class User
{
    [J("name")] public string Name { get; set; } = "";
    [J("id")] public ulong Id { get; set; }

    [J("reason", NullValueHandling = NullValueHandling.Ignore)]
    public string Reason { get; set; } = "";
}

public class EmbedColor
{
    [J("R")] public int R { get; set; } = 255;
    [J("G")] public int G { get; set; } = 105;
    [J("B")] public int B { get; set; } = 180;
}