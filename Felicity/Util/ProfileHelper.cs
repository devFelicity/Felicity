﻿using DotNetBungieAPI.Models;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;

namespace Felicity.Util;

public abstract class ProfileHelper
{
    public static async Task<ProfileResponse> GetRequestedProfile(
        string bungieTag,
        ulong discordId,
        UserDb userDb,
        IBungieClient bungieClient)
    {
        var profile = new ProfileResponse();

        if (string.IsNullOrEmpty(bungieTag))
        {
            var currentUser = userDb.Users.FirstOrDefault(x => x.DiscordId == discordId);

            if (currentUser == null)
            {
                profile.Error =
                    "You haven't specified a Bungie name to look up. If you want to use your own account, please register with '/user register' first."
                    + "Alternatively, you can provide a specific name to search for.";
                return profile;
            }

            profile.MembershipId = currentUser.DestinyMembershipId;
            profile.MembershipType = currentUser.DestinyMembershipType;
            profile.BungieName = currentUser.BungieName;

            return profile;
        }

        var name = bungieTag.Split("#").First();
        var code = Convert.ToInt16(bungieTag.Split("#").Last());

        var goodProfile = await BungieApiUtils.GetLatestProfileAsync(bungieClient, name, code);
        if (goodProfile == null || goodProfile.MembershipType == BungieMembershipType.None)
        {
            profile.Error =
                $"No profiles found matching `{bungieTag}`.\nThis can happen if no characters are currently on the Bungie account.";
            return profile;
        }

        profile.MembershipId = goodProfile.MembershipId;
        profile.MembershipType = goodProfile.MembershipType;
        profile.BungieName = $"{goodProfile.BungieGlobalDisplayName}#{goodProfile.BungieGlobalDisplayNameCode}";

        return profile;
    }

    public class ProfileResponse
    {
        public string? Error { get; set; }
        public long MembershipId { get; set; }
        public BungieMembershipType MembershipType { get; set; }
        public string? BungieName { get; set; }
    }
}
