using System.Text;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Models.Destiny.Definitions.Records;
using DotNetBungieAPI.Models.Destiny.Responses;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;
using Felicity.Util.Enums;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireOAuth]
public class CraftingCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly ServerDb _serverDb;
    private readonly UserDb _userDb;

    public CraftingCommands(UserDb userDb, IBungieClient bungieClient, ServerDb serverDb)
    {
        _userDb = userDb;
        _bungieClient = bungieClient;
        _serverDb = serverDb;
    }

    [SlashCommand("crafted", "View all crafted weapon levels.")]
    public async Task Crafted()
    {
        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        var serverLanguage = MiscUtils.GetServer(_serverDb, Context.Guild.Id).BungieLocale;

        var request = await _bungieClient.ApiAccess.Destiny2.GetProfile(user!.DestinyMembershipType,
            user.DestinyMembershipId,
            new[]
            {
                DestinyComponentType.CharacterEquipment,
                DestinyComponentType.CharacterInventories,
                DestinyComponentType.ProfileInventories
            }, user.GetTokenData());

        var allItems = request.Response.ProfileInventory.Data.Items.Where(x =>
            x.ItemInstanceId != null && x.State.HasFlag(ItemState.Crafted)).ToList();
        allItems.AddRange(request.Response.CharacterInventories.Data.Values.SelectMany(d =>
            d.Items.Where(x => x.ItemInstanceId != null && x.State.HasFlag(ItemState.Crafted))));
        allItems.AddRange(request.Response.CharacterEquipment.Data.Values.SelectMany(d =>
            d.Items.Where(x => x.ItemInstanceId != null && x.State.HasFlag(ItemState.Crafted))));

        var embed = Embeds.MakeBuilder();
        embed.Title = "Crafted List";
        embed.Description = "List of crafted weapons and your weapon level on them.";
        embed.ThumbnailUrl =
            "https://www.bungie.net/common/destiny2_content/icons/e7e6d522d375dfa6dec055135ce6a77e.png";

        var craftedList = Craftables.CraftedList;

        var sb = new StringBuilder();

        foreach (var (source, weaponList) in craftedList)
        {
            var field = new EmbedFieldBuilder
            {
                Name = source,
                IsInline = true
            };

            foreach (var weaponId in weaponList)
            {
                if (embed.Fields.Count is 2 or 5 or 8 or 11)
                    embed.AddField("\u200b", '\u200b');

                var item = allItems.FirstOrDefault(x => x.Item.Select(y => y.Hash) == weaponId);

                if (item == null)
                    continue;

                _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(weaponId,
                    serverLanguage,
                    out var manifestRecord);

                var getItemRequest = await _bungieClient.ApiAccess.Destiny2.GetItem(user.DestinyMembershipType,
                    user.DestinyMembershipId, (long)item.ItemInstanceId!, new[]
                    {
                        DestinyComponentType.ItemPlugObjectives
                    });

                if (!Craftables.GetWeaponLevel(getItemRequest.Response, out var weaponLevel)) weaponLevel = "N/A";

                sb.Append(
                    $"\n> {EmoteHelper.StaticEmote("pattern")} lv.{weaponLevel} - [{manifestRecord.DisplayProperties.Name}]({MiscUtils.GetLightGgLink(manifestRecord.Hash)})");
            }

            if (string.IsNullOrEmpty(sb.ToString()))
                continue;

            field.Value = sb.ToString();
            sb.Clear();

            embed.AddField(field);
        }

        if (embed.Fields.Count == 0)
            embed.Description = "You do not have any crafted weapons.";

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("recipes", "View current progression towards weapon recipes.")]
    public async Task Recipes(
        [Summary("show-complete", "Show completed recipes? (default: false)")]
        bool showComplete = false)
    {
        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        var serverLanguage = MiscUtils.GetServer(_serverDb, Context.Guild.Id).BungieLocale;

        var request = await _bungieClient.ApiAccess.Destiny2.GetProfile(user!.DestinyMembershipType,
            user.DestinyMembershipId,
            new[]
            {
                DestinyComponentType.Records,
                DestinyComponentType.CharacterEquipment,
                DestinyComponentType.CharacterInventories,
                DestinyComponentType.ProfileInventories
            }, user.GetTokenData());

        var embed = Embeds.MakeBuilder();
        embed.Title = "Craftable List";
        embed.Description = "List of craftable weapons and your progress on them.";
        embed.ThumbnailUrl =
            "https://www.bungie.net/common/destiny2_content/icons/e7e6d522d375dfa6dec055135ce6a77e.png";

        var updateDescription = false;

        var craftableList = Craftables.CraftableList;

        foreach (var (source, weaponList) in craftableList)
        {
            var field = new EmbedFieldBuilder
            {
                Name = source,
                IsInline = true
            };

            foreach (var weaponId in weaponList)
            {
                if (embed.Fields.Count is 2 or 5 or 8 or 11) // there has to be a better way
                    embed.AddField("\u200b", '\u200b');

                _bungieClient.Repository.TryGetDestinyDefinition<DestinyRecordDefinition>(weaponId, serverLanguage,
                    out var manifestRecord);

                var record = request.Response.ProfileRecords.Data.Records[weaponId];
                var obj = record.Objectives.First();

                if (obj.IsComplete && !showComplete)
                    continue;

                field.Value += "\n > ";

                if (obj.IsComplete)
                {
                    field.Value += "✅";
                }
                else
                {
                    var inventoryItemCount = GetItemCount(request, manifestRecord.Hash);
                    if (inventoryItemCount > 0)
                    {
                        field.Value += $"{obj.Progress + inventoryItemCount}/{obj.CompletionValue} ⚠️ ";
                        updateDescription = true;
                    }
                    else
                    {
                        field.Value += $"{obj.Progress}/{obj.CompletionValue}";
                    }
                }

                field.Value +=
                    $" - [{manifestRecord.DisplayProperties.Name}]({MiscUtils.GetLightGgLink(Craftables.GetWeaponId(manifestRecord.Hash))})";
            }

            if (string.IsNullOrEmpty((string?)field.Value)) continue;

            embed.AddField(field);
        }

        if (updateDescription)
            embed.Description += "\n\n⚠️ = Includes incomplete deepsight weapons.";

        if (embed.Fields.Count == 0)
            embed.Description = "You have completed all available patterns.";

        await FollowupAsync(embed: embed.Build());
    }

    private static int GetItemCount(BungieResponse<DestinyProfileResponse> request, uint recordDefinitionHash)
    {
        var allItems = request.Response.ProfileInventory.Data.Items.ToList();
        allItems.AddRange(request.Response.CharacterInventories.Data.Values.SelectMany(d => d.Items));
        allItems.AddRange(request.Response.CharacterEquipment.Data.Values.SelectMany(d => d.Items));

        var goodHash = Craftables.GetWeaponId(recordDefinitionHash);

        return allItems.Where(destinyItemComponent => destinyItemComponent.Item.Hash == goodHash)
            .Count(destinyItemComponent => destinyItemComponent.State.HasFlag(ItemState.HighlightedObjective));
    }
}