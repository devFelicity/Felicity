// ReSharper disable UnusedMember.Global

namespace Felicity.Configs
{
    public class BotConfig
    {
        public BotSettings Settings { get; set; } = new BotSettings();
    }

    public class BotSettings
    {
        public string DiscordToken { get; set; } = "[YOUR TOKEN HERE]";

        public string BungieApiKey { get; set; } = "[YOUR API KEY HERE]";

        public int TimeBetweenRefresh { get; set; } = 5;

        public double Version { get; set; } = 4.0;

        public string CommandPrefix { get; set; } = "f!";

        public string Note { get; set; } = "Hello World";

        public int DurationToWaitForNextMessage { get; set; } = 20;

        public ulong[] BotStaff { get; set; }

        public ulong[] BotSupporters { get; set; }

        public EmbedColorGroup EmbedColor { get; set; } = new EmbedColorGroup();
    }

    public class EmbedColorGroup
    {
        public int R { get; set; } = 255;

        public int G { get; set; } = 105;

        public int B { get; set; } = 180;
    }
}
