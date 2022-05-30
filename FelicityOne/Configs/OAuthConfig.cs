using BungieSharper.Entities;
using J = Newtonsoft.Json.JsonPropertyAttribute;

#pragma warning disable CS8618

// ReSharper disable UnusedMember.Global

namespace FelicityOne.Configs;

public class OAuthConfig
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
    [J("bungieName")] public string BungieName { get; set; }
    [J("membershipId")] public long MembershipId { get; set; }
    [J("membershipType")] public BungieMembershipType MembershipType { get; set; }
    [J("characterIds")] public long[] CharacterIds { get; set; }
}