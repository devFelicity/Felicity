using System.Collections.Generic;
using Newtonsoft.Json;

namespace Felicity.Configs;

internal class DataConfig
{
    [JsonProperty("DiscordIDLinks")]
    public static List<DiscordIDLink> DiscordIDLinks { get; set; } = new();

    [JsonProperty("AnnounceDailyLinks")]
    public static List<ulong> AnnounceDailyLinks { get; set; } = new();

    [JsonProperty("AnnounceWeeklyLinks")]
    public static List<ulong> AnnounceWeeklyLinks { get; set; } = new();

    public class DiscordIDLink
    {
        [JsonProperty("DiscordID")]
        public ulong DiscordID { get; set; }

        [JsonProperty("BungieMembershipID")]
        public string BungieMembershipID { get; set; } = "-1";

        [JsonProperty("BungieMembershipType")]
        public string BungieMembershipType { get; set; } = "-1";

        [JsonProperty("UniqueBungieName")]
        public string UniqueBungieName { get; set; } = "Guardian#0000";
    }
}