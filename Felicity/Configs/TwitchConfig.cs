// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;

namespace Felicity.Configs;

public class TwitchConfig
{
    public TwitchSettings Settings { get; set; } = new();
}

public class TwitchSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public Dictionary<string, User> Users { get; set; } = new() {{"0", new User()}};
}

public class User
{
    public string Name { get; set; } = string.Empty;
    public ulong ServerId { get; set; } = 0;
    public ulong ChannelId { get; set; } = 0;
    public ulong UserId { get; set; } = 0;
    public ulong Mention { get; set; } = 0;
    public bool MentionEveryone { get; set; } = false;
}