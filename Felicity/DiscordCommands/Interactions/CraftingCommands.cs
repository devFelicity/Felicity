using System.Text;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Authorization;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Models.Destiny.Definitions.Records;
using DotNetBungieAPI.Models.Destiny.Responses;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;
using Felicity.Util.Enums;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireOAuth]
public class CraftingCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly InteractiveService _interactiveService;
    private readonly UserDb _userDb;

    public CraftingCommands(UserDb userDb, IBungieClient bungieClient, InteractiveService interactiveService)
    {
        _userDb = userDb;
        _bungieClient = bungieClient;
        _interactiveService = interactiveService;
    }

    // TODO: paginate this
    [SlashCommand("crafted", "View all crafted weapon levels.")]
    public async Task Crafted()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);

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
                if (embed.Fields.Count % 3 == 2)
                    embed.AddField("\u200b", '\u200b');

                var itemList = allItems.Where(x => x.Item.Select(y => y.Hash) == weaponId).ToList();

                if (!itemList.Any())
                    continue;

                var highestWeaponLevel = 0;

                foreach (var destinyItemComponent in itemList)
                {
                    var getItemRequest = await _bungieClient.ApiAccess.Destiny2.GetItem(user.DestinyMembershipType,
                        user.DestinyMembershipId, (long)destinyItemComponent.ItemInstanceId!, new[]
                        {
                            DestinyComponentType.ItemPlugObjectives
                        });

                    // ReSharper disable once InvertIf
                    if (Craftables.GetWeaponLevel(getItemRequest.Response, out var weaponLevel))
                    {
                        if (!int.TryParse(weaponLevel, out var currentWeaponLevel))
                            continue;

                        if (highestWeaponLevel < currentWeaponLevel)
                            highestWeaponLevel = currentWeaponLevel;
                    }
                }

                _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(weaponId,
                    out var manifestRecord);

                sb.Append(
                    //$"\n> {FormattedWeaponLevel(highestWeaponLevel, itemList.Count > 1)} [{manifestRecord.DisplayProperties.Name}]({MiscUtils.GetLightGgLink(manifestRecord.Hash)})");
                    $"\n> {FormattedWeaponLevel(highestWeaponLevel, itemList.Count > 1)} - {manifestRecord.DisplayProperties.Name}");

                if (itemList.Count > 1 && !embed.Description.Contains("`*` = "))
                    embed.Description +=
                        "\n\n`*` = Multiple crafted weapons are in your inventory, only the highest level is returned.";
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
        [Summary("showAll", "Show complete patterns (default: false)")]
        bool showAll = false)
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);

        var request = await _bungieClient.ApiAccess.Destiny2.GetProfile(user!.DestinyMembershipType,
            user.DestinyMembershipId,
            new[]
            {
                DestinyComponentType.Characters,
                DestinyComponentType.Records,
                DestinyComponentType.CharacterEquipment,
                DestinyComponentType.CharacterInventories,
                DestinyComponentType.ProfileInventories
            }, user.GetTokenData());

        var deepDeepsight = await IsDeepsightAvailable(8721509,
            user.DestinyMembershipType, user.DestinyMembershipId, user.GetTokenData(),
            request.Response.Characters.Data.Keys.First());

        var invDescription = false;
        var buyDescription = false;

        var craftableList = Craftables.CraftableList;
        var pageList = new List<PageBuilder>();

        var done = false;
        var i = 0;

        while (!done)
        {
            var page = new PageBuilder
            {
                Title = "Craftable List",
                Description = "List of craftable weapons and your progress on them.",
                //ThumbnailUrl =
                //    "https://www.bungie.net/common/destiny2_content/icons/cf05991b4a82c4faec17755105bda88f.png",
                Color = Embeds.DefaultColor
            };

            while (page.Fields.Count < 5)
            {
                var keyValuePair = craftableList.ElementAt(i);

                var field = new EmbedFieldBuilder
                {
                    Name = keyValuePair.Key,
                    IsInline = true
                };

                foreach (var weaponId in keyValuePair.Value)
                {
                    if (page.Fields.Count % 3 == 2)
                        page.AddField("\u200b", '\u200b');

                    _bungieClient.Repository.TryGetDestinyDefinition<DestinyRecordDefinition>(weaponId,
                        out var manifestRecord);

                    var record = request.Response.ProfileRecords.Data.Records[weaponId];
                    var obj = record.Objectives.First();

                    if (obj.IsComplete)
                    {
                        if (showAll)
                            field.Value += "\n > `✅`";
                        else
                            continue;
                    }
                    else
                    {
                        var inventoryItemCount = GetItemCount(request, manifestRecord.Hash);
                        if (inventoryItemCount > 0)
                        {
                            field.Value += $"\n > `{obj.Progress + inventoryItemCount}/{obj.CompletionValue}` ⚠️ ";
                            invDescription = true;
                        }
                        else
                        {
                            field.Value += $"\n > `{obj.Progress}/{obj.CompletionValue}`";
                        }

                        if (keyValuePair.Key is "Deep" && deepDeepsight)
                        {
                            if (field.Value.ToString()!.Contains("⚠️"))
                                field.Value += "💰 ";
                            else
                                field.Value += " 💰 ";

                            buyDescription = true;
                        }
                    }

                    if (!obj.IsComplete || showAll)
                        field.Value +=
                            $" - [{manifestRecord.DisplayProperties.Name}]({MiscUtils.GetLightGgLink(Craftables.GetWeaponId(manifestRecord.Hash))})";
                }

                i++;

                if (invDescription)
                    page.Description += "\n\n⚠ = Includes incomplete deepsight weapons.";

                if (buyDescription)
                    page.Description +=
                        "\n\n💰 = A pattern for this weapon can be purchased from the appropriate vendor.";

                if (!string.IsNullOrEmpty((string?)field.Value))
                    page.Fields.Add(field);

                // ReSharper disable once InvertIf
                if (craftableList.Keys.Count == i)
                {
                    done = true;
                    break;
                }
            }

            if (page.Fields.Count == 0)
                page.Description += "\n\nYou have completed all available patterns.";

            pageList.Add(page);
        }

        var paginatorBuilder = new StaticPaginatorBuilder()
            .AddUser(Context.User)
            .WithPages(pageList)
            .AddOption(new Emoji("◀"), PaginatorAction.Backward)
            .AddOption(new Emoji("🔢"), PaginatorAction.Jump)
            .AddOption(new Emoji("▶"), PaginatorAction.Forward)
            .WithActionOnCancellation(ActionOnStop.DisableInput)
            .WithActionOnTimeout(ActionOnStop.DisableInput)
            .Build();

        await _interactiveService.SendPaginatorAsync(paginatorBuilder, Context.Interaction,
            TimeSpan.FromMinutes(10), InteractionResponseType.DeferredChannelMessageWithSource);
    }

    private async Task<bool> IsDeepsightAvailable(uint vendorId, BungieMembershipType destinyMembershipType,
        long destinyMembershipId, AuthorizationTokenData tokenData, long characterId)
    {
        var request = await _bungieClient.ApiAccess.Destiny2.GetVendor(destinyMembershipType, destinyMembershipId,
            characterId, vendorId, new[]
            {
                DestinyComponentType.VendorCategories,
                DestinyComponentType.ItemSockets
            }, tokenData);

        var categoryIndex = vendorId switch
        {
            8721509 => 1,
            _ => 0
        };

        var vendorItemIndex = request.Response.Categories.Data.Categories.ElementAt(categoryIndex).ItemIndexes
            .ElementAt(1);

        try
        {
            return request.Response.ItemComponents.Sockets.Data[vendorItemIndex].Sockets.Last().Plug
                .Select(x => x.DisplayProperties.Name).Contains("Deepsight");
        }
        catch
        {
            return false;
        }
    }

    private static string FormattedWeaponLevel(int weaponLevel, bool isMultiple)
    {
        var sb = new StringBuilder();
        sb.Append("`lv.");

        switch (weaponLevel.ToString().Length)
        {
            case 1:
                sb.Append("  ");
                break;
            case 2:
                sb.Append(' ');
                break;
        }

        sb.Append(weaponLevel);

        sb.Append(isMultiple ? '*' : ' ');

        sb.Append('`');

        return sb.ToString();
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