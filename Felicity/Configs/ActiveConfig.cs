using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Felicity.Configs;

internal class ActiveConfig
{
    [JsonProperty("MaximumLoggingUsers")]
    public static int MaximumLoggingUsers = 20;

    [JsonProperty("RefreshesBeforeKick")]
    public static int RefreshesBeforeKick = 2;

    [JsonProperty("ActiveAFKUsers")]
    public static List<ActiveAFKUser> ActiveAFKUsers { get; set; } = new();

    public class ActiveAFKUser
    {
        [JsonProperty("DiscordID")]
        public ulong DiscordID { get; set; }

        [JsonProperty("BungieMembershipID")]
        public ulong BungieMembershipID { get; set; }

        [JsonProperty("UniqueBungieName")]
        public string UniqueBungieName { get; set; } = "Guardian#0000";

        [JsonProperty("DiscordChannelID")]
        public ulong DiscordChannelID { get; set; }

        [JsonProperty("StartLevel")]
        public int StartLevel { get; set; }

        [JsonProperty("StartLevelProgress")]
        public int StartLevelProgress { get; set; }

        [JsonProperty("TimeStarted")]
        public DateTime TimeStarted { get; set; } = DateTime.Now;

        [JsonProperty("LastLoggedLevel")]
        public int LastLoggedLevel { get; set; }

        [JsonProperty("LastLoggedLevelProgress")]
        public int LastLevelProgress { get; set; }

        [JsonProperty("NoXPGainRefreshes")]
        public int NoXPGainRefreshes { get; set; }
    }
}