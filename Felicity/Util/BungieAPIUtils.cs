using DotNetBungieAPI.Authorization;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Requests;
using DotNetBungieAPI.Models.User;
using Felicity.Models;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Util;

public static class BungieApiUtils
{
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

        var response = userInfoCard.Response.First();
        if (response == null)
            return null;

        return await GetLatestProfile(bungieClient, response.MembershipId, response.MembershipType);
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
                var refreshedUser = await client.Authentication.RenewToken(token);

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