﻿using BungieSharper.Entities;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Definitions;
using BungieSharper.Entities.Destiny.Definitions.Collectibles;
using BungieSharper.Entities.Destiny.Responses;
using Discord;
using Discord.Interactions;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using FelicityOne.Services;
using Serilog;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands.SlashCommands;

[Group("lookup", "Various lookup commands for Destiny 2.")]
public class Lookup : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("wish", "Look up patterns for wishes in the Last Wish raid.")]
    public async Task LookupWish(
        [Summary("wish", "Which wish do you need?")] [Autocomplete(typeof(WishAutocomplete))]
        int wishNumber)
    {
        await DeferAsync();

        var wish = Wishes.KnownWishes[wishNumber-1];

        var embed = new EmbedBuilder
        {
            Color = ConfigService.GetEmbedColor(),
            Description = wish.Description,
            Footer = Extensions.GenerateEmbedFooter(),
            ImageUrl = $"https://cdn.tryfelicity.one/images/wishes/wish-{wishNumber}.png",
            ThumbnailUrl = "https://bungie.net/common/destiny2_content/icons/fc5791eb2406bf5e6b361f3d16596693.png",
            Title = $"Wish {wish.Number}: {wish.Name}"
        };

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("guardian", "Look up a profile of a player.")]
    public async Task LookupGuardian(
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

            if (linkedUser == null!)
                await FollowupAsync("You aren't registered and didn't provide a bungie name.");

            var linkedProfile = BungieAPI.GetApiClient().Api.Destiny2_GetLinkedProfiles(linkedUser.MembershipId,
                BungieMembershipType.BungieNext, authToken: linkedUser.AccessToken).Result;

            var latestProfile = new DestinyProfileUserInfoCard();
            var lastPlayed = new DateTime(1970, 1, 1);

            foreach (var profile in linkedProfile.Profiles)
                if (profile.DateLastPlayed > lastPlayed)
                    latestProfile = profile;

            membershipId = latestProfile.MembershipId;
            membershipType = latestProfile.MembershipType;
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

                var userCard = BungieAPI.GetApiClient().Api
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

                    var goodProfile = Extensions.GetLatestProfile(name, code);

                    membershipId = goodProfile.MembershipId;
                    membershipType = goodProfile.MembershipType;
                    bungieName = $"{goodProfile.BungieGlobalDisplayName}#{goodProfile.BungieGlobalDisplayNameCode}";
                }
                catch (Exception ex)
                {
                    var msg = $"Failed to lookup: {bungieTag}\n{ex.GetType()}: {ex.Message}";
                    Log.Error(ex, msg);

                    await FollowupAsync(
                        "Failed to lookup profile, try using the full Bungie.net profile link.\n-> https://www.bungie.net/7/en/User/Profile/");
                    return;
                }
            }
        }

        var player = BungieAPI.GetApiClient().Api.Destiny2_GetProfile(membershipId,
            membershipType, new[]
            {
                DestinyComponentType.Characters
            }).Result;

        await FollowupAsync(embed: player.GenerateLookupEmbed(bungieName, membershipId, membershipType));
    }

    [SlashCommand("accountshare", "Look up account shared emblems of a player.")]
    public async Task LookupAccountShare(
        [Summary("bungiename",
            "Bungie name of the requested user (name#1234).")]
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

            var goodProfile = Extensions.GetLatestProfile(name, code);

            membershipId = goodProfile.MembershipId;
            membershipType = goodProfile.MembershipType;
            bungieName = $"{goodProfile.BungieGlobalDisplayName}#{goodProfile.BungieGlobalDisplayNameCode}";
        }
        catch (Exception ex)
        {
            var msg = $"Failed to lookup: {bungieTag}\n{ex.GetType()}: {ex.Message}";

            if (ex.InnerException is not FormatException)
            {
                ex.Data.Add("command", "lookup accountshare");
                ex.Data.Add("parameter", bungieTag);
                Log.Error(msg, ex);
            }

            await FollowupAsync(msg);

            return;
        }

        var profile = BungieAPI.GetApiClient().Api.Destiny2_GetProfile(membershipId,
            membershipType, new[]
            {
                DestinyComponentType.Characters, DestinyComponentType.Profiles, DestinyComponentType.Collectibles
            }).Result;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable HeuristicUnreachableCode
        if (profile.ProfileCollectibles.Data == null!)
        {
            var privateEmbed = Extensions.GenerateMessageEmbed(bungieName, "",
                "User has their collections set to private, unable to parse emblems.",
                "https://www.bungie.net/7/en/User/Profile/254/" + profile.Profile.Data.UserInfo.MembershipId);

            await FollowupAsync(embed: privateEmbed.Build());
            return;
        }
        // ReSharper restore HeuristicUnreachableCode

        var emblemCount = 0;
        var emblemList = new List<DestinyCollectibleDefinition>();

        var manifestInventoryItemIDs = profile.Characters.Data
            .Select(destinyCharacterComponent => destinyCharacterComponent.Value.EmblemHash).ToList();
        var manifestCollectibleIDs =
            profile.ProfileCollectibles.Data.Collectibles.Select(collectible => collectible.Key).ToList();

        var lang = Context.Language();

        var manifestInventoryItems =
            BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lang, manifestInventoryItemIDs);

        var manifestCollectibles =
            BungieAPI.GetManifestDefinition<DestinyCollectibleDefinition>(lang, manifestCollectibleIDs);

        foreach (var collectible in from collectible in manifestCollectibles
                 where !collectible.Redacted
                 where !string.IsNullOrEmpty(collectible.DisplayProperties.Name)
                 from manifestCollectibleParentNodeHash in collectible.ParentNodeHashes
                 where EmblemCats.EmblemCatList.Contains((EmblemCat) manifestCollectibleParentNodeHash)
                 select collectible)
        {
            emblemCount++;

            var value = profile.ProfileCollectibles.Data.Collectibles[collectible.Hash];

            foreach (var unused in from emblem in manifestInventoryItems
                     where emblem.CollectibleHash == collectible.Hash
                     where value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                     where !emblemList.Contains(collectible)
                     select emblem) emblemList.Add(collectible);

            if (value.State.HasFlag(DestinyCollectibleState.Invisible) &&
                !value.State.HasFlag(DestinyCollectibleState.NotAcquired))
                if (!emblemList.Contains(collectible))
                    emblemList.Add(collectible);

            // ReSharper disable once InvertIf
            if (value.State.HasFlag(DestinyCollectibleState.UniquenessViolation) &&
                value.State.HasFlag(DestinyCollectibleState.NotAcquired))
                if (!emblemList.Contains(collectible))
                    emblemList.Add(collectible);
        }

        var sortedList = emblemList.OrderBy(o => o.DisplayProperties.Name).ToList();

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = bungieName,
                Url = "https://www.bungie.net/7/en/User/Profile/254/" +
                      profile.Profile.Data.UserInfo.MembershipId
            },
            Color = Color.Purple,
            ThumbnailUrl = BungieAPI.BaseUrl + profile.Characters.Data.First().Value.EmblemPath,
            Footer = new EmbedFooterBuilder
            {
                Text = $"{Strings.FelicityVersion} | Parsed {emblemCount} emblems.",
                IconUrl = Images.FelicityLogo
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

public class WishAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        await Task.Delay(0);

        var resultList = new List<Wish>();

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        resultList.AddRange(currentSearch != null
            ? Wishes.KnownWishes.Where(autocompleteResult =>
                autocompleteResult.Description.ToLower().Contains(currentSearch.ToLower()))
            : Wishes.KnownWishes);

        var autocompleteList = resultList
            .Select(wish => new AutocompleteResult($"Wish {wish.Number}: {wish.Description}", wish.Number)).ToList();

        return AutocompletionResult.FromSuccess(autocompleteList);
    }
}