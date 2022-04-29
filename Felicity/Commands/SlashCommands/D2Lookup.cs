using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIHelper;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Definitions.Collectibles;
using BungieSharper.Entities.User;
using Ceen;
using Discord;
using Discord.Interactions;
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.SlashCommands;

[Group("lookup", "Various lookup commands for Destiny 2.")]
public class D2Lookup : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("guardian", "Look up a profile of a player.")]
    public async Task Guardian(
        [Summary("bungiename", "Bungie name of the requested user (name#1234)")]
        string bungieTag = "")
    {
        await DeferAsync();

        long membershipId;
        BungieMembershipType membershipType;
        string bungieName;

        if (string.IsNullOrEmpty(bungieTag))
        {
            var linkedUser = OAuthService.GetUser(Context.User.Id).Result;
            var linkedProfile = APIService.GetApiClient().Api.Destiny2_GetLinkedProfiles(linkedUser.MembershipId,
                BungieMembershipType.BungieNext, false, linkedUser.AccessToken).Result;

            membershipId = linkedProfile.Profiles.First().MembershipId;
            membershipType = linkedProfile.Profiles.First().MembershipType;
            bungieName =
                $"{linkedProfile.BnetMembership.BungieGlobalDisplayName}#{linkedProfile.BnetMembership.BungieGlobalDisplayNameCode}";
        }
        else
        {
            if (bungieTag.StartsWith("https://www.bungie.net/7/en/User/Profile/"))
            {
                var url = bungieTag.Split("Profile/").Last();
                if (url.Contains('?')) url = url.Split("?").First();

                var urlMemId = url.Split("/").Last();
                var urlMemType = url.Split("/").First();

                var userCard = APIService.GetApiClient().Api
                    .User_GetMembershipDataById(Convert.ToInt64(urlMemId),
                        Enum.Parse<BungieMembershipType>(urlMemType)).Result.DestinyMemberships.First();

                membershipId = userCard.MembershipId;
                membershipType = userCard.MembershipType;
                bungieName = $"{userCard.BungieGlobalDisplayName}#{userCard.BungieGlobalDisplayNameCode}";
            }
            else
            {
                try
                {
                    var name = bungieTag.Split("#").First();
                    var code = Convert.ToInt16(bungieTag.Split("#").Last());

                    var userInfoCard = APIService.GetApiClient().Api.Destiny2_SearchDestinyPlayerByBungieName(
                        BungieMembershipType.All,
                        new ExactSearchRequest
                        {
                            DisplayName = name,
                            DisplayNameCode = code
                        }).Result.First();

                    membershipId = userInfoCard.MembershipId;
                    membershipType = userInfoCard.MembershipType;
                    bungieName = $"{userInfoCard.BungieGlobalDisplayName}#{userInfoCard.BungieGlobalDisplayNameCode}";
                }
                catch (Exception ex)
                {
                    var msg = $"Failed to lookup: {bungieTag}\n{ex.GetType()}: {ex.Message}";
                    await Log.ErrorAsync(msg);
                    LogHelper.LogToDiscord(msg);

                    await FollowupAsync(
                        "Failed to lookup profile, try using the full Bungie.net profile link.\n-> https://www.bungie.net/7/en/User/Profile/");
                    return;
                }
            }
        }

        var player = APIService.GetApiClient().Api.Destiny2_GetProfile(membershipId,
            membershipType, new[]
            {
                DestinyComponentType.Characters
            }).Result;

        await FollowupAsync("", new[] {player.GenerateLookupEmbed(bungieName, membershipId, membershipType)});
    }

    [SlashCommand("accountshare", "Look up account shared emblems of a player.")]
    public async Task AccountShare(
        [Summary("bungiename", "Bungie name of the requested user (name#1234)")]
        string bungieTag)
    {
        await DeferAsync();

        long membershipId;
        BungieMembershipType membershipType;
        string bungieName;

        try
        {
            var name = bungieTag.Split("#").First();
            var code = Convert.ToInt16(bungieTag.Split("#").Last());

            var userInfoCard = APIService.GetApiClient().Api.Destiny2_SearchDestinyPlayerByBungieName(
                BungieMembershipType.All,
                new ExactSearchRequest
                {
                    DisplayName = name,
                    DisplayNameCode = code
                }).Result.First();

            membershipId = userInfoCard.MembershipId;
            membershipType = userInfoCard.MembershipType;
            bungieName = $"{userInfoCard.BungieGlobalDisplayName}#{userInfoCard.BungieGlobalDisplayNameCode}";
        }
        catch (Exception ex)
        {
            var msg = $"Failed to lookup: {bungieTag}\n{ex.GetType()}: {ex.Message}";
            await Log.ErrorAsync(msg);
            LogHelper.LogToDiscord(msg);

            await FollowupAsync("Failed to lookup player, profile or collections may be private.");
            return;
        }

        var profile = APIService.GetApiClient().Api.Destiny2_GetProfile(membershipId,
            membershipType, new[]
            {
                DestinyComponentType.Characters, DestinyComponentType.Profiles, DestinyComponentType.Collectibles
            });

        var emblemCount = 0;
        var emblemList = new List<DestinyCollectibleDefinition>();

        var equippedEmblemList = profile.Result.Characters.Data.Select(destinyCharacterComponent =>
                ManifestConnection.GetInventoryItemById(unchecked((int) destinyCharacterComponent.Value.EmblemHash)))
            .ToList();

        foreach (var (key, value) in profile.Result.ProfileCollectibles.Data.Collectibles)
        {
            var manifestCollectible = ManifestConnection.GetItemCollectibleId(unchecked((int) Convert.ToInt64(key)));
            if (manifestCollectible.Redacted)
                continue;

            if (string.IsNullOrEmpty(manifestCollectible.DisplayProperties.Name))
                continue;

            var emblemCatList = new List<long>
            {
                2451657441, // Seasonal
                24961706, // Account
                1166184619, // General
                1801524334, // Competitive
                4111024827, // Gambit
                3958514834, // Strikes
                631010939, // World
                2220993106, // Trials
                329982304 // Raids
            };

            foreach (var manifestCollectibleParentNodeHash in manifestCollectible.ParentNodeHashes)
            {
                if (!emblemCatList.Contains(manifestCollectibleParentNodeHash))
                    continue;

                emblemCount++;

                foreach (var unused in from emblem in equippedEmblemList
                         where emblem.CollectibleHash == manifestCollectible.Hash
                         where value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                         where !emblemList.Contains(manifestCollectible)
                         select emblem) emblemList.Add(manifestCollectible);

                if (value.State.HasFlag(DestinyCollectibleState.Invisible) &&
                    !value.State.HasFlag(DestinyCollectibleState.NotAcquired))
                    if (!emblemList.Contains(manifestCollectible))
                        emblemList.Add(manifestCollectible);

                // ReSharper disable once InvertIf
                if (value.State.HasFlag(DestinyCollectibleState.UniquenessViolation) &&
                    value.State.HasFlag(DestinyCollectibleState.NotAcquired))
                    if (!emblemList.Contains(manifestCollectible))
                        emblemList.Add(manifestCollectible);
            }
        }

        var sortedList = emblemList.OrderBy(o => o.DisplayProperties.Name).ToList();

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = bungieName,
                Url = "https://www.bungie.net/7/en/User/Profile/254/" +
                      profile.Result.Profile.Data.UserInfo.MembershipId
            },
            Color = Color.Purple,
            Footer = new EmbedFooterBuilder
            {
                Text = $"Parsed {emblemCount} emblems."
            }
        };

        if (sortedList.Count == 0)
        {
            embed.Description = "Account has no shared emblems.";
        }
        else
        {
            embed.Description = "**Account shared emblems:**\n\n";

            foreach (var emblemDefinition in sortedList)
                embed.Description +=
                    $"[{emblemDefinition.DisplayProperties.Name}](https://destinyemblemcollector.com/emblem?id={emblemDefinition.ItemHash})\n";
        }

        await FollowupAsync(embed: embed.Build());
    }
}