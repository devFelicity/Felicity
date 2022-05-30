using J = Newtonsoft.Json.JsonPropertyAttribute;

#pragma warning disable CS8618

namespace FelicityOne.Configs;

public class EmoteConfig
{
    [J("Settings")] public EmoteSettings Settings { get; set; }
}

public class EmoteSettings
{
    [J("ServerIDs")] public ulong[] ServerIDs { get; set; }
    [J("Emotes")] public Dictionary<uint, Emote> Emotes { get; set; }
}

public class Emote
{
    [J("Name")] public string Name { get; set; }

    [J("Id")] public ulong Id { get; set; }
}