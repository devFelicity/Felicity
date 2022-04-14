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
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.TestCommands;

public class D2Vendors : ModuleBase<SocketCommandContext>
{
    [Command("xur")]
    public async Task Xur()
    {
        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendor(2305843009842534029, 4611686018471516071,
                BungieMembershipType.TigerSteam, 2190858386, new[]
                {
                    DestinyComponentType.ItemPerks, DestinyComponentType.ItemStats, DestinyComponentType.ItemSockets,
                    DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                    DestinyComponentType.VendorSales
                },
                "CLKFBBKGAgAgaPDdgIJ43R42xovC703blgH8fXl6U+JD+hwsEgqLKxrgAAAAvk8lLIK3zTugGzR9PNStikywNEqMxbYoEaWVRJ7DrWlywUNwoJ2HDRS1WeMKMSYQZH51GwVmEK6UV3sKYWYTUR62RAhI5d/Q1ghBtEPml8sJAZWS4kqHGftWZp+Idr4q0VSCTcwODjrShF7gcIxGx5GDWpL4SJR4+9Ltbk9FIrrbEBHYI6wYxHvavJQ9e5zMbSq3lGH/OzqOFksEDDOBl+BshkjbRZQ3cBg/ui5KJctK6rmM3Ws/nvdfwPVW/SoShfaNRm+p0OwnHBxVfTvsK1V67lHX2ipaVIZx2agdqR0=")
            .Result;

        var xurSales = vendorData.Sales.Data;
        var xurPerks = vendorData.ItemComponents.Perks.Data;
        var xurStats = vendorData.ItemComponents.Stats.Data;

        var xurExoticsCat = vendorData.Categories.Data.Categories.ElementAt(0);
        var xurSeasonalWepsCat = vendorData.Categories.Data.Categories.ElementAt(1);
        var xurLegendaryWeaponsCat = vendorData.Categories.Data.Categories.ElementAt(2);
        var xurArmorCat = vendorData.Categories.Data.Categories.ElementAt(3);

        var xurExotics = new List<DestinyVendorSaleItemComponent>();
        var xurSeasonalWeps = new List<DestinyVendorSaleItemComponent>();
        var xurLegendaryWeapons = new List<DestinyVendorSaleItemComponent>();
        var xurArmor = new List<DestinyVendorSaleItemComponent>();

        foreach (var (key, value) in xurSales)
        {
            if (key == 1)
                continue;
            if (xurExoticsCat.ItemIndexes.Contains(key))
                xurExotics.Add(value);
            if (xurSeasonalWepsCat.ItemIndexes.Contains(key))
                xurSeasonalWeps.Add(value);
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

        var exotics = "";
        var legendaryWeapons = "";

        foreach (var destinyVendorSaleItemComponent in xurExotics)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) destinyVendorSaleItemComponent.ItemHash));

            if (item.ItemType != DestinyItemType.Armor)
                continue;

            exotics += $"{item.DisplayProperties.Name} ({GetTotalStats(xurStats, destinyVendorSaleItemComponent.VendorItemIndex)})\n";
        }
        embed.AddField("Exotics", exotics, true);

        // too cursed.
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var destinyVendorSaleItemComponent in xurLegendaryWeapons)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int)destinyVendorSaleItemComponent.ItemHash));

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