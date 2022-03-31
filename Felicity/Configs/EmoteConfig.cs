using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace Felicity.Configs;

internal class EmoteConfig
{
    public List<ulong> EmoteServers { get; internal set; } = new List<ulong>();

    public Dictionary<string, string> Emotes { get; internal set; } = new Dictionary<string, string>();
}