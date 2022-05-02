using System.Collections.Generic;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Configs;

public partial class EmoteConfig
{
    [J("Settings")] public EmoteSettings Settings { get; set; }
}

public class EmoteSettings
{
    [J("ServerIDs")] public ulong[] ServerIDs { get; set; }
    [J("Emotes")] public Dictionary<string, Emote> Emotes { get; set; }
}

public class Emote
{
    [J("type")] public string Type { get; set; }
    [J("name")] public string Name { get; set; }
    [J("discordID")] public ulong DiscordId { get; set; }
}

public partial class EmoteConfig
{
    public static EmoteConfig FromJson(string json)
    {
        return JsonConvert.DeserializeObject<EmoteConfig>(json, Converter.Settings);
    }
}

public static class Serialize
{
    public static string ToJson(this EmoteConfig self)
    {
        return JsonConvert.SerializeObject(self, Converter.Settings);
    }
}