// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Felicity.Models.Caches;

public class EmoteCache
{
    public EmoteSettings Settings { get; set; } = new();
}

public class EmoteSettings
{
    public ulong[]? ServerIDs { get; set; }
    public Dictionary<uint, Emote>? Emotes { get; set; }
}

public class Emote
{
    public string? Name { get; set; }

    public ulong Id { get; set; }
}