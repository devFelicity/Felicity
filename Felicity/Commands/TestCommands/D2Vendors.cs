using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIHelper;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Entities.Items;
using BungieSharper.Entities.Destiny.Entities.Vendors;
using Discord;
using Discord.Commands;
using Felicity.Enums;
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.TestCommands;

public class D2Vendors : ModuleBase<SocketCommandContext>
{
    [RequireOAuthPrecondition]
    [Command("mods")]
    public async Task Mods()
    {
        var oauth = Context.User.OAuth();

        var destinyMembership = oauth.DestinyMembership;
        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendors(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, new[]
                {
                    DestinyComponentType.VendorSales
                }, authToken: oauth.AccessToken)
            .Result;
        
        var adaItems = vendorData.Sales.Data[(uint) Vendors.Ada1].SaleItems;
        var bansheeItems = vendorData.Sales.Data[(uint) Vendors.Banshee44].SaleItems;

        var adaMods = "";
        var bansheeMods = "";

        foreach (var (_, value) in adaItems)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) value.ItemHash));

            if (item.ItemType != DestinyItemType.Mod) continue;

            if (value.SaleStatus == VendorItemStatus.Success)
                adaMods += "*";

            adaMods += $"{item.DisplayProperties.Name}\n";
        }

        foreach (var (_, value) in bansheeItems)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) value.ItemHash));

            if (item.ItemType != DestinyItemType.Mod) continue;

            if (value.SaleStatus == VendorItemStatus.Success)
                bansheeMods += "*";

            bansheeMods += $"{item.DisplayProperties.Name}\n";
        }

        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl =
                    "https://www.bungie.net/common/destiny2_content/icons/DestinyPresentationNodeDefinition_1b273a0d5a6aba677747b2e7412ea6fd.png",
                Name = "Mod Vendors"
            },
            Description = "* = not owned",
            Footer = new EmbedFooterBuilder
            {
                Text = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0}",
                IconUrl = "https://whaskell.pw/images/felicity_circle.jpg"
            }
        };

        embed.AddField("Ada-1", adaMods, true);
        embed.AddField("Banshee-44", bansheeMods, true);

        await Context.Message.ReplyAsync(embed: embed.Build());
    }

    [RequireOAuthPrecondition]
    [Command("xur")]
    public async Task Xur()
    {
        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendor(2305843009842534029, 4611686018471516071,
                BungieMembershipType.TigerSteam, (uint) Vendors.Xur, new[]
                {
                    DestinyComponentType.ItemPerks, DestinyComponentType.ItemStats, DestinyComponentType.ItemSockets,
                    DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                    DestinyComponentType.VendorSales
                },
                Context.User.OAuth().AccessToken)
            .Result;

        var xurSales = vendorData.Sales.Data;
        var xurPerks = vendorData.ItemComponents.Perks.Data;
        var xurSockets = vendorData.ItemComponents.Sockets.Data;
        var xurStats = vendorData.ItemComponents.Stats.Data;

        var xurExoticsCat = vendorData.Categories.Data.Categories.ElementAt(0);
        var xurSeasonalWepsCat = vendorData.Categories.Data.Categories.ElementAt(1);
        var xurLegendaryWeaponsCat = vendorData.Categories.Data.Categories.ElementAt(2);
        var xurArmorCat = vendorData.Categories.Data.Categories.ElementAt(3);

        var xurExotics = new List<DestinyVendorSaleItemComponent>();
        //var xurSeasonalWeps = new List<DestinyVendorSaleItemComponent>();
        var xurLegendaryWeapons = new List<DestinyVendorSaleItemComponent>();
        var xurArmor = new List<DestinyVendorSaleItemComponent>();

        foreach (var (key, value) in xurSales)
        {
            if (key == 1)
                continue;
            if (xurExoticsCat.ItemIndexes.Contains(key))
                xurExotics.Add(value);
            if (xurSeasonalWepsCat.ItemIndexes.Contains(key))
                xurExotics.Add(value);
            if (xurLegendaryWeaponsCat.ItemIndexes.Contains(key))
                xurLegendaryWeapons.Add(value);
            if (xurArmorCat.ItemIndexes.Contains(key))
                xurArmor.Add(value);
        }

        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = "https://www.bungie.net/img/destiny_content/vendor/icons/xur_large_icon.png",
                Name = "Xûr, Agent of the Nine"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0}",
                IconUrl = "https://whaskell.pw/images/felicity_circle.jpg"
            }
        };

        var exoticArmor = "";
        var exoticWeapons = "";
        var legendaryWeapons = "";

        foreach (var destinyVendorSaleItemComponent in xurExotics)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) destinyVendorSaleItemComponent.ItemHash));

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (item.ItemType)
            {
                case DestinyItemType.Armor:
                    exoticArmor +=
                        $"{item.DisplayProperties.Name} ({GetTotalStats(xurStats, destinyVendorSaleItemComponent.VendorItemIndex)})\n";
                    break;
                case DestinyItemType.Weapon:
                    exoticWeapons +=
                        $"{item.DisplayProperties.Name} ({xurSockets[destinyVendorSaleItemComponent.VendorItemIndex].Sockets.ElementAt(3).PlugHash} / {xurSockets[destinyVendorSaleItemComponent.VendorItemIndex].Sockets.ElementAt(4).PlugHash})\n";
                    break;
            }
        }

        embed.AddField("Exotic Armor", exoticArmor, true);
        embed.AddField("Exotic Weapons", exoticWeapons, true);

        // too cursed.
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var destinyVendorSaleItemComponent in xurLegendaryWeapons)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) destinyVendorSaleItemComponent.ItemHash));

            if (item.ItemType != DestinyItemType.Weapon)
                continue;

            legendaryWeapons += $"{item.DisplayProperties.Name}\n";
        }

        embed.AddField("Weapons", legendaryWeapons, true);

        await ReplyAsync(embed: embed.Build());
    }

    private static int GetTotalStats(IReadOnlyDictionary<int, DestinyItemStatsComponent> xurStats, int vendorItemIndex)
    {
        var item = xurStats[vendorItemIndex];
        var total = item.Stats.Sum(destinyStat => destinyStat.Value.Value);
        return total;
    }
}