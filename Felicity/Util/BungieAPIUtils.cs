using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Authorization;
using DotNetBungieAPI.Models.Requests;
using DotNetBungieAPI.Models.User;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Serilog;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Util;

public static class BungieApiUtils
{
    public static async Task<bool> CheckApi(IBungieClient client)
    {
        var apiInfo = await client.ApiAccess.Misc.GetCommonSettings();

        try
        {
            if (apiInfo.Response.Systems.TryGetValue("Destiny2", out var d2Value))
                if (d2Value.IsEnabled)
                    return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "API check failure.");
        }

        return false;
    }

    public static async Task<DestinyProfileUserInfoCard> GetLatestProfile(IBungieClient client, long membershipId,
        BungieMembershipType membershipType)
    {
        var result = new DestinyProfileUserInfoCard();

        var linkedProfiles = await client.ApiAccess.Destiny2.GetLinkedProfiles(membershipType, membershipId);

        foreach (var potentialProfile in linkedProfiles.Response.Profiles)
            if (potentialProfile.DateLastPlayed > result.DateLastPlayed)
                result = potentialProfile;

        return result;
    }

    public static async Task<DestinyProfileUserInfoCard?> GetLatestProfile(IBungieClient bungieClient,
        string bungieName, short bungieCode)
    {
        var userInfoCard = await bungieClient.ApiAccess.Destiny2.SearchDestinyPlayerByBungieName(
            BungieMembershipType.All,
            new ExactSearchRequest
            {
                DisplayName = bungieName,
                DisplayNameCode = bungieCode
            });

        var response = userInfoCard.Response;
        if (response == null || response.Count == 0)
            return null;

        return await GetLatestProfile(bungieClient, response.First().MembershipId, response.First().MembershipType);
    }

    public static async Task ForceRefresh(IBungieClient client, UserDb userDb)
    {
        var nowTime = DateTime.UtcNow;

        foreach (var user in userDb.Users)
            try
            {
                var token = new AuthorizationTokenData
                {
                    AccessToken = user.OAuthToken,
                    RefreshToken = user.OAuthRefreshToken,
                    RefreshExpiresIn = (int)(user.OAuthRefreshExpires - nowTime).TotalSeconds,
                    MembershipId = user.BungieMembershipId,
                    TokenType = "Bearer"
                };
                var refreshedUser = await client.Authorization.RenewToken(token);

                user.OAuthToken = refreshedUser.AccessToken;
                user.OAuthTokenExpires = nowTime.AddSeconds(refreshedUser.ExpiresIn);
                user.OAuthRefreshToken = refreshedUser.RefreshToken;
                user.OAuthRefreshExpires = nowTime.AddSeconds(refreshedUser.RefreshExpiresIn);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Caught {e.GetType()}: {e.Message}\n{user.BungieName}");
            }

        await userDb.SaveChangesAsync();
    }
}