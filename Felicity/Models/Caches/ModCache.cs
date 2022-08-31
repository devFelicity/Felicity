using System.Text;
using System.Text.Json;
using Discord;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Components;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Models.Destiny.Definitions.SandboxPerks;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Util;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Models.Caches;

public class ModCache
{
    public Dictionary<string, List<Mod>>? ModInventory { get; set; }
    public DateTime InventoryExpires { get; set; }
}

public class Mod
{
    public uint Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public static class ProcessModData
{
    public static async Task<Embed> BuildEmbed(IBungieClient bungieClient, ModCache self, User user)
    {
        var embed = Embeds.MakeBuilder();

        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Mod Vendors:",
            IconUrl = BotVariables.Images.ModVendorIcon
        };

        embed.Description = "Ada-1 and Banshee-44 can both be found in the Tower.";

        var adaMods = await GetMods(bungieClient, self, DefinitionHashes.Vendors.Ada1_350061650, user);
        var bansheeMods = await GetMods(bungieClient, self, DefinitionHashes.Vendors.Banshee44_672118013, user);

        embed.AddField("Ada-1", adaMods, true);
        embed.AddField("Banshee-44", bansheeMods, true);

        if (adaMods.Contains("⚠️") || bansheeMods.Contains("⚠️"))
            embed.Description += "\n\n⚠️ - not owned in collections.";

        return embed.Build();
    }

    private static async Task<string> GetMods(IBungieClient bungieClient, ModCache self, uint vendor, User user)
    {
        var clarityDb = await ClarityParser.Fetch();

        var result = new StringBuilder();

        var characterIdTask = await bungieClient.ApiAccess.Destiny2.GetProfile(user.DestinyMembershipType,
            user.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            });

        var vendorData = await bungieClient.ApiAccess.Destiny2.GetVendors(user.DestinyMembershipType,
            user.DestinyMembershipId, characterIdTask.Response.Characters.Data.Keys.First(), new[]
            {
                DestinyComponentType.VendorSales
            }, user.GetTokenData());

        foreach (var mod in self.ModInventory![vendor.ToString()])
        {
            var missing = "";

            foreach (var personalDestinyVendorSaleItemSetComponent in vendorData.Response.Sales.Data)
            foreach (var destinyVendorSaleItemComponent in personalDestinyVendorSaleItemSetComponent.Value.SaleItems)
                if (destinyVendorSaleItemComponent.Value.Item.Hash == mod.Id)
                    if (destinyVendorSaleItemComponent.Value.SaleStatus == VendorItemStatus.Success)
                        missing = "⚠️ ";

            Clarity? clarityValue = null;

            if (clarityDb != null && clarityDb.ContainsKey(mod.Id.ToString()))
                clarityValue = clarityDb[mod.Id.ToString()];

            result.Append($"{missing}[{mod.Name}]({MiscUtils.GetLightGgLink(mod.Id)})\n" +
                          $"> {Format.Italics(mod.Description)}\n");

            if (clarityValue != null)
                result.Append($"> [Clarity](https://www.d2clarity.com/): {clarityValue.Description}\n");

            result.Append('\n');
        }

        return result.ToString();
    }

    public static async Task<ModCache> FetchInventory(IBungieClient bungieClient, BungieLocales lg, User oauth)
    {
        ModCache modCache;

        var path = $"Data/modCache-{lg}.json";

        if (File.Exists(path))
        {
            modCache = JsonSerializer.Deserialize<ModCache>(await File.ReadAllTextAsync(path))!;

            if (modCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return modCache;
        }

        var characterIdTask = await bungieClient.ApiAccess.Destiny2.GetProfile(oauth.DestinyMembershipType,
            oauth.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            });

        var vendorData = await bungieClient.ApiAccess.Destiny2.GetVendors(oauth.DestinyMembershipType,
            oauth.DestinyMembershipId, characterIdTask.Response.Characters.Data.Keys.First(), new[]
            {
                DestinyComponentType.VendorSales
            }, oauth.GetTokenData());

        modCache = new ModCache
        {
            InventoryExpires = ResetUtils.GetNextDailyReset(),
            ModInventory = new Dictionary<string, List<Mod>>()
        };

        modCache = await PopulateMods(bungieClient, lg, modCache, DefinitionHashes.Vendors.Ada1_350061650,
            vendorData.Response.Sales.Data);
        modCache = await PopulateMods(bungieClient, lg, modCache, DefinitionHashes.Vendors.Banshee44_672118013,
            vendorData.Response.Sales.Data);

        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(modCache));

        return modCache;
    }

    private static Task<ModCache> PopulateMods(IBungieClient bungieClient, BungieLocales lg, ModCache modCache,
        uint vendor,
        IReadOnlyDictionary<uint, PersonalDestinyVendorSaleItemSetComponent> salesData)
    {
        modCache.ModInventory?.Add(vendor.ToString(), new List<Mod>());

        var manifestItems = new List<DestinyInventoryItemDefinition>();

        foreach (var saleItemsValue in salesData[vendor].SaleItems.Values)
        {
            if (saleItemsValue.Item.Hash == null)
                continue;

            bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(
                (uint)saleItemsValue.Item.Hash, lg, out var manifestItem);

            manifestItems.Add(manifestItem);
        }

        var manifestPerks = new List<DestinySandboxPerkDefinition>();
        foreach (var destinyInventoryItemDefinition in manifestItems.Where(destinyInventoryItemDefinition =>
                     destinyInventoryItemDefinition.ItemType == DestinyItemType.Mod))
        {
            bungieClient.Repository.TryGetDestinyDefinition<DestinySandboxPerkDefinition>(
                (uint)destinyInventoryItemDefinition.Perks.First().Perk.Hash!, lg, out var result);

            manifestPerks.Add(result);
        }

        var i = 0;
        var j = 0;

        foreach (var (_, value) in salesData[vendor].SaleItems)
        {
            var item = manifestItems[i];

            if (item.ItemType != DestinyItemType.Mod)
            {
                i++;
                continue;
            }

            var modInfo = manifestPerks[j];

            modCache.ModInventory![vendor.ToString()].Add(new Mod
            {
                Name = modInfo.DisplayProperties.Name,
                Description = modInfo.DisplayProperties.Description,
                Id = (uint)value.Item.Hash!
            });

            j++;
            i++;
        }

        return Task.FromResult(modCache);
    }
}