using System;
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

public partial class OAuthResponse
{
    [J("access_token")] public string AccessToken { get; set; }
    [J("token_type")] public string TokenType { get; set; }
    [J("expires_in")] public long ExpiresIn { get; set; }
    [J("refresh_token")] public string RefreshToken { get; set; }
    [J("refresh_expires_in")] public long RefreshExpiresIn { get; set; }
    [J("membership_id")] public string MembershipId { get; set; }
}

public partial class OAuthResponse
{
    public static OAuthResponse FromJson(string json)
    {
        return JsonConvert.DeserializeObject<OAuthResponse>(json, Converter.Settings);
    }
}

