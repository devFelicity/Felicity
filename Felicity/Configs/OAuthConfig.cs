using System;
using BungieSharper.Entities;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Felicity.Configs;

using J = JsonPropertyAttribute;

public partial class OAuthConfig
{
    [J("discordId")] public ulong DiscordId { get; set; }
    [J("state")] public string State { get; set; }
    [J("accessToken")] public string AccessToken { get; set; }
    [J("tokenType")] public string TokenType { get; set; }
    [J("expiresAt")] public DateTime ExpiresAt { get; set; }
    [J("refreshToken")] public string RefreshToken { get; set; }
    [J("refreshExpiresAt")] public DateTime RefreshExpiresAt { get; set; }
    [J("membershipId")] public long MembershipId { get; set; }
    [J("destinyMembership")] public DestinyMembership DestinyMembership { get; set; }
}

public class DestinyMembership
{
    [J("membershipId")] public long MembershipId { get; set; }
    [J("membershipType")] public BungieMembershipType MembershipType { get; set; }
    [J("characterIds")] public long[] CharacterIds { get; set; }
}

public partial class OAuthConfig
{
    public static OAuthConfig FromJson(string json)
    {
        return JsonConvert.DeserializeObject<OAuthConfig>(json, Converter.Settings);
    }

    public static string ToJson(OAuthConfig self)
    {
        return JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
