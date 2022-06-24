using DotNetBungieAPI.Clients;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.User;

namespace Felicity.Util;

public class BungieAPIUtils
{
    private readonly IBungieClient? _client;

    public BungieAPIUtils(IBungieClient client)
    {
        _client = client;
    }

    public async Task<DestinyProfileUserInfoCard> GetLatestProfile(long membershipId, BungieMembershipType membershipType)
    {
        var result = new DestinyProfileUserInfoCard();

        if (_client == null)
            return result;

        var linkedProfiles = await _client.ApiAccess.Destiny2.GetLinkedProfiles(membershipType, membershipId);
        
        foreach (var potentialProfile in linkedProfiles.Response.Profiles)
            if (potentialProfile.DateLastPlayed > result.DateLastPlayed)
                result = potentialProfile;

        return result;
    }
}