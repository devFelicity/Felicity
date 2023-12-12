using System.Text;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;
using Humanizer;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

public class RollFinderCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;

    public RollFinderCommands(IBungieClient bungieClient)
    {
        _bungieClient = bungieClient;
    }

    [SlashCommand("roll-finder", "Find a recommended roll for a specific weapon from a curated list.")]
    public async Task RollFinder(
        [Summary("game-mode", "Game mode to suggest rolls for:")] [Choice("PvE", 0)] [Choice("PvP", 1)]
        int gameMode,
        [Autocomplete(typeof(RollFinderAutocomplete))] [Summary("weapon-name", "Name of the weapon to search for:")]
        uint weaponId)
    {
        await DeferAsync();

        var weaponRollList = await ProcessRollData.FromJsonAsync();

        if (weaponRollList == null)
        {
            await FollowupAsync("An error occurred fetching curated weapon rolls.");
            return;
        }

        var rollList = gameMode switch
        {
            0 => weaponRollList.PvE,
            1 => weaponRollList.PvP,
            _ => null
        };

        if (rollList == null)
        {
            await FollowupAsync("An error has occurred.");
            return;
        }

        var requestedRoll = rollList.Where(x => x.WeaponId == weaponId).ToList();
        if (!requestedRoll.Any())
        {
            await FollowupAsync("An error has occurred while fetching requested roll.");
            return;
        }

        var embed = Embeds.MakeBuilder();

        if (weaponRollList.Authors != null)
        {
            var rollAuthor = weaponRollList.Authors.FirstOrDefault(x => x.Id == requestedRoll.First().AuthorId);
            embed.Author = new EmbedAuthorBuilder
            {
                Name = rollAuthor?.Name,
                IconUrl = rollAuthor?.Image,
                Url = rollAuthor?.Url
            };
        }

        if (!_bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(
                requestedRoll.First().WeaponId,
                out var weaponDefinition))
        {
            await FollowupAsync("Failed to fetch weapon from manifest.");
            return;
        }

        embed.ThumbnailUrl = weaponDefinition.DisplayProperties.Icon.AbsolutePath;

        var perks = string.Empty;

        for (var i = 0; i < requestedRoll.First().WeaponPerks.Count - 1; i++)
            perks += $"{requestedRoll.First().WeaponPerks[i]},";

        var foundryLink =
            $"https://d2foundry.gg/w/{requestedRoll.First().WeaponId}?p={perks.TrimEnd(',')}&m=0&mw={requestedRoll.First().WeaponPerks.Last()}";

        embed.Description =
            $"This is the recommended {Format.Bold(gameMode == 0 ? "PvE" : "PvP")} roll for {Format.Bold(weaponDefinition.DisplayProperties.Name)}.\n" +
            $"[Click here]({MiscUtils.GetLightGgLink(requestedRoll.First().WeaponId)}) to view the weapon on Light.GG.";

        embed.AddField("Type", EmoteHelper.StaticEmote(weaponDefinition.EquippingBlock.AmmoType.ToString()) +
                               EmoteHelper.StaticEmote(weaponDefinition.DefaultDamageTypeEnumValue.ToString()) +
                               EmoteHelper.GetItemType(weaponDefinition.ItemSubType), true);
        embed.AddField("Acquirable", requestedRoll.First().CanDrop, true);
        embed.AddField("Source", ReadSource(requestedRoll.First().Source), true);

        for (var i = 0; i < requestedRoll.Count; i++)
        {
            embed.AddField(requestedRoll.Count != 1 ? $"Recommended roll {i + 1}" : "Why should you pick this roll?",
                $"> {requestedRoll[i].Reason}", true);
            embed.AddField("Perks", GetPerkList(requestedRoll[i]), true);
            embed.AddField("Foundry Link", $"[Click Here]({foundryLink})", true);
        }

        await FollowupAsync(embed: embed.Build());
    }

    private static string ReadSource(WeaponSource source)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (source)
        {
            case WeaponSource.XurEternity:
                return "Dares Of Eternity";
            case WeaponSource.SeasonalHunt:
            case WeaponSource.SeasonalChosen:
            case WeaponSource.SeasonalSplicer:
            case WeaponSource.SeasonalLost:
            case WeaponSource.SeasonalRisen:
            case WeaponSource.SeasonalHaunted:
            case WeaponSource.SeasonalPlunder:
                var split = source.Humanize(LetterCasing.Title).Split(' ');
                return $"{split[0]} ({split[1]})";
            default:
                return source.Humanize(LetterCasing.Title);
        }
    }

    private string GetPerkList(Roll requestedRoll)
    {
        var perkList = new StringBuilder();

        foreach (var weaponPerk in requestedRoll.WeaponPerks)
        {
            if (weaponPerk == 0)
            {
                perkList.Append("*no data*\n");
                continue;
            }

            _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(weaponPerk,
                out var weaponPerkDefinition);

            if (weaponPerkDefinition.Plug.PlugStyle == PlugUiStyles.Masterwork)
                perkList.Append(
                    EmoteHelper.StaticEmote(weaponPerkDefinition.Plug.PlugCategoryIdentifier.Split('.').Last()));
            else
                perkList.Append(EmoteHelper.GetEmote(Context.Client,
                    weaponPerkDefinition.DisplayProperties.Icon.RelativePath,
                    weaponPerkDefinition.DisplayProperties.Name, weaponPerkDefinition.Hash));

            perkList.Append(weaponPerkDefinition.DisplayProperties.Name);

            perkList.Append('\n');
        }

        return perkList.ToString();
    }
}
