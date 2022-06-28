using DotNetBungieAPI.Clients;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.User;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Util;

public class BungieApiUtils
{
    public async Task<DestinyProfileUserInfoCard> GetLatestProfile(IBungieClient client, long membershipId, BungieMembershipType membershipType)
    {
        var result = new DestinyProfileUserInfoCard();

        var linkedProfiles = await client.ApiAccess.Destiny2.GetLinkedProfiles(membershipType, membershipId);
        
        foreach (var potentialProfile in linkedProfiles.Response.Profiles)
            if (potentialProfile.DateLastPlayed > result.DateLastPlayed)
                result = potentialProfile;

        return result;
    }
}