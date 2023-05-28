using System.Text;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.Collectibles;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;
using Felicity.Util.Enums;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

[Group("emblem", "Various lookup commands for Destiny 2.")]
public class EmblemCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly ServerDb _serverDb;
    private readonly UserDb _userDb;

    public EmblemCommands(IBungieClient bungieClient, ServerDb serverDb, UserDb userDb)
    {
        _bungieClient = bungieClient;
        _serverDb = serverDb;
        _userDb = userDb;
    }

    [SlashCommand("shares", "Look up account shared emblems of a player.")]
    public async Task EmblemShares(
        [Summary("bungie-name",
            "Bungie name of the requested user (name#1234).")]
        string bungieTag = "")
    {
        await DeferAsync();

        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        var joke = bungieTag.ToLower() == "moonie#6881";

        if (bungieTag.ToLower() == "~moonie#6881")
            bungieTag = "Moonie#6881";

        if (!string.IsNullOrEmpty(bungieTag) && !bungieTag.Contains('#'))
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description =
                $"`{bungieTag}` is not a correct format for a Bungie name.\nTry again with the `<name>#<number>` format.";
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var requestedProfile =
            await ProfileHelper.GetRequestedProfile(bungieTag, Context.User.Id, _userDb, _bungieClient);

        if (!string.IsNullOrEmpty(requestedProfile.Error))
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = requestedProfile.Error;
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var profile = await _bungieClient.ApiAccess.Destiny2.GetProfile(requestedProfile.MembershipType,
            requestedProfile.MembershipId, new[]
            {
                DestinyComponentType.Characters, DestinyComponentType.Profiles, DestinyComponentType.Collectibles
            });

        if (joke)
        {
            var jokeEmbed = Embeds.MakeBuilder();

            jokeEmbed.Title = requestedProfile.BungieName;
            jokeEmbed.Url =
                $"https://www.bungie.net/7/en/User/Profile/{(int)profile.Response.Profile.Data.UserInfo.MembershipType}/" +
                profile.Response.Profile.Data.UserInfo.MembershipId;
            jokeEmbed.ThumbnailUrl =
                BotVariables.BungieBaseUrl + profile.Response.Characters.Data.First().Value.EmblemPath;

            jokeEmbed.Description += "> [Wish Ascended](https://emblem.report/2419113769)\n";
            jokeEmbed.Description += "> [Heavy Is The Crown](https://emblem.report/1661191198)\n";
            jokeEmbed.Description += "> [Creator's Cachet](https://emblem.report/2526736320)\n";
            jokeEmbed.Description += "> [Parallel Program](https://emblem.report/3936625542)\n";

            jokeEmbed.Footer.Text += " | Try ~Moonie#6881.";

            jokeEmbed.AddField("Parsed", "> 4", true);
            jokeEmbed.AddField("Shared", "> 713", true);

            await FollowupAsync(embed: jokeEmbed.Build());
            return;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        // Note to self, bungie privacy sucks ass and always reports privacy as private.
        if (profile.Response.ProfileCollectibles.Data == null)
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = $"`{bungieTag}` has their collections set to private, unable to parse emblems.";

            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var emblemCount = 0;
        var emblemList = new List<DestinyCollectibleDefinition>();

        var manifestInventoryItemIDs = profile.Response.Characters.Data
            .Select(destinyCharacterComponent => destinyCharacterComponent.Value.Emblem.Hash).ToList();
        var manifestCollectibleIDs =
            profile.Response.ProfileCollectibles.Data.Collectibles.Select(collectible => collectible.Key).ToList();

        var lg = MiscUtils.GetLanguage(Context.Guild, _serverDb);

        var manifestInventoryItems = new List<DestinyInventoryItemDefinition>();
        foreach (var destinyInventoryItemDefinition in manifestInventoryItemIDs)
        {
            _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(
                (uint)destinyInventoryItemDefinition!, lg, out var result);

            manifestInventoryItems.Add(result);
        }

        var manifestCollectibles = new List<DestinyCollectibleDefinition>();
        foreach (var definitionHashPointer in manifestCollectibleIDs)
        {
            _bungieClient.Repository.TryGetDestinyDefinition<DestinyCollectibleDefinition>(
                (uint)definitionHashPointer.Hash!, lg, out var result);

            manifestCollectibles.Add(result);
        }

        foreach (var collectible in from collectible in manifestCollectibles
                 where !collectible.Redacted
                 where !string.IsNullOrEmpty(collectible.DisplayProperties.Name)
                 from manifestCollectibleParentNodeHash in collectible.ParentNodes
                 where EmblemCats.EmblemCatList.Contains((EmblemCat)manifestCollectibleParentNodeHash.Hash!)
                 select collectible)
        {
            emblemCount++;

            var value = profile.Response.ProfileCollectibles.Data.Collectibles[collectible.Hash];

            foreach (var unused in from emblem in manifestInventoryItems
                     where emblem.Collectible.Hash == collectible.Hash
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

        var embed = Embeds.MakeBuilder();

        embed.Title = requestedProfile.BungieName;
        embed.Url =
            $"https://www.bungie.net/7/en/User/Profile/{(int)profile.Response.Profile.Data.UserInfo.MembershipType}/" +
            profile.Response.Profile.Data.UserInfo.MembershipId;
        embed.ThumbnailUrl = BotVariables.BungieBaseUrl + profile.Response.Characters.Data.First().Value.EmblemPath;

        if (sortedList.Count == 0)
        {
            embed.Description = "Account has no shared emblems.";
        }
        else
        {
            embed.Description = "**Account shared emblems:**\n";

            foreach (var emblemDefinition in sortedList)
                embed.Description +=
                    $"> [{emblemDefinition.DisplayProperties.Name}](https://emblem.report/{emblemDefinition.Item.Hash})\n";
        }

        embed.AddField("Parsed", $"> {emblemCount}", true);
        embed.AddField("Shared", $"> {sortedList.Count}", true);

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("rarest", "Gets the top 5 rarest emblems in collections.")]
    public async Task EmblemRarest(
        [Summary("bungie-name",
            "Bungie name of the requested user (name#1234).")]
        string bungieTag = "")
    {
        await DeferAsync();

        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        if (!string.IsNullOrEmpty(bungieTag) && !bungieTag.Contains('#'))
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description =
                $"`{bungieTag}` is not a correct format for a Bungie name.\nTry again with the `<name>#<number>` format.";
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var requestedProfile =
            await ProfileHelper.GetRequestedProfile(bungieTag, Context.User.Id, _userDb, _bungieClient);

        if (!string.IsNullOrEmpty(requestedProfile.Error))
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = requestedProfile.Error;
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var profile = await _bungieClient.ApiAccess.Destiny2.GetProfile(requestedProfile.MembershipType,
            requestedProfile.MembershipId, new[]
            {
                DestinyComponentType.Collectibles, DestinyComponentType.Profiles
            });

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        // Note to self, bungie privacy sucks ass and always reports privacy as private.
        if (profile.Response.ProfileCollectibles.Data == null)
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = $"`{bungieTag}` has their collections set to private, unable to parse emblems.";
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var manifestCollectibleIDs =
            (from destinyCollectibleComponent in profile.Response.ProfileCollectibles.Data.Collectibles
                where !destinyCollectibleComponent.Value.State.HasFlag(DestinyCollectibleState.UniquenessViolation) ||
                      !destinyCollectibleComponent.Value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                where !destinyCollectibleComponent.Value.State.HasFlag(DestinyCollectibleState.Invisible) ||
                      destinyCollectibleComponent.Value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                where !destinyCollectibleComponent.Value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                select destinyCollectibleComponent.Key).ToList();

        foreach (var destinyCollectibleComponent in profile.Response.CharacterCollectibles.Data)
            manifestCollectibleIDs.AddRange(from collectibleComponent in destinyCollectibleComponent.Value.Collectibles
                where !collectibleComponent.Value.State.HasFlag(DestinyCollectibleState.UniquenessViolation) ||
                      !collectibleComponent.Value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                where !collectibleComponent.Value.State.HasFlag(DestinyCollectibleState.Invisible) ||
                      collectibleComponent.Value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                where !collectibleComponent.Value.State.HasFlag(DestinyCollectibleState.NotAcquired)
                select collectibleComponent.Key);

        var lg = MiscUtils.GetLanguage(Context.Guild, _serverDb);

        var manifestCollectibles = new List<DestinyCollectibleDefinition>();
        foreach (var definitionHashPointer in manifestCollectibleIDs)
        {
            _bungieClient.Repository.TryGetDestinyDefinition<DestinyCollectibleDefinition>(
                (uint)definitionHashPointer.Hash!, lg, out var result);

            if (!result.Redacted && !string.IsNullOrEmpty(result.DisplayProperties.Name))
                manifestCollectibles.AddRange(from definitionParentNode in result.ParentNodes
                    where EmblemCats.EmblemCatList.Contains((EmblemCat)definitionParentNode.Hash!)
                    select result);
        }

        string jsonString;

        try
        {
            var collectiblesData = new
            {
                collectibles = manifestCollectibles.Select(x => x.Hash).ToList()
            };

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(collectiblesData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://emblem.report/api/getRarestEmblems", content);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            jsonString = responseData;
        }
        catch (Exception error)
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = error.Message;
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var emblemResponse = EmblemReport.EmblemResponse.FromJson(jsonString);
        if (emblemResponse?.Data == null)
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = "Failed to parse response from server.";
            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var embed = new EmbedBuilder
        {
            Title = requestedProfile.BungieName,
            Url =
                $"https://www.bungie.net/7/en/User/Profile/{(int)profile.Response.Profile.Data.UserInfo.MembershipType}/" +
                profile.Response.Profile.Data.UserInfo.MembershipId,
            Color = Color.Purple,
            Footer = Embeds.MakeFooter(),
            Description =
                "Here are the 5 rarest emblems in collections for this user.\nData provided by [emblem.report](https://emblem.report).\n\n"
        };

        foreach (var emblem in emblemResponse.Data)
            if (_bungieClient.Repository.TryGetDestinyDefinition<DestinyCollectibleDefinition>(emblem.CollectibleHash,
                    lg, out var emblemDef))
            {
                var sb = new StringBuilder();

                var acquired = emblem.Acquisition.ToString("N0");

                switch (acquired.Length)
                {
                    case 1:
                        sb.Append("     ");
                        break;
                    case 2:
                        sb.Append("    ");
                        break;
                    case 3:
                        sb.Append("   ");
                        break;
                    case 4:
                        sb.Append("  ");
                        break;
                    case 5:
                        sb.Append(' ');
                        break;
                }

                sb.Append(acquired);

                embed.Description +=
                    $"> `{sb}` - " +
                    $"[{emblemDef.DisplayProperties.Name}](https://emblem.report/{emblemDef.Item.Select(x => x.Hash)}) " +
                    $"({emblem.Percentage}%)\n";
            }

        await FollowupAsync(embed: embed.Build());
    }
}