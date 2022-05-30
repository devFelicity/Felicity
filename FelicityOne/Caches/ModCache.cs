using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Definitions;
using BungieSharper.Entities.Destiny.Responses;
using Discord;
using FelicityOne.Configs;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using J = Newtonsoft.Json.JsonPropertyAttribute;

#pragma warning disable CS8618

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Caches;

public class ModCache
{
    [J("ModInventory")] public Dictionary<string, List<Mod>> ModInventory { get; set; }
    [J("InventoryExpires")] public DateTime InventoryExpires { get; set; }
}

public class Mod
{
    [J("id")] public uint Id { get; set; }
    [J("name")] public string Name { get; set; }
    [J("description")] public string Description { get; set; }
}

public static class ProcessModData
{
    public static Embed BuildEmbed(this ModCache self, OAuthConfig oauth, DestinyMembership destinyMembership,
        bool checkInventory = false)
    {
        var embed = Extensions.GenerateVendorEmbed("Mod Vendors", "", checkInventory
            ? "Ada-1 and Banshee-44 can both be found in the Tower.\n[*] = not owned."
            : "You can use `/vendor mods` to view unacquired mods from this list.\nAda-1 and Banshee-44 can both be found in the Tower.");

        var adaMods = GetMods(self, VendorIds.Ada1, checkInventory, oauth, destinyMembership);
        var bansheeMods = GetMods(self, VendorIds.Banshee44, checkInventory, oauth, destinyMembership);

        embed.AddField("Ada-1", adaMods, true);
        embed.AddField("Banshee-44", bansheeMods, true);

        return embed.Build();
    }

    private static string GetMods(ModCache self, VendorIds vendor, bool checkInventory, OAuthConfig oauth,
        DestinyMembership destinyMembership)
    {
        var result = "";

        var vendorData = BungieAPI.GetApiClient().Api.Destiny2_GetVendor(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, (uint) vendor, new[]
                {
                    DestinyComponentType.VendorSales
                }, oauth.AccessToken)
            .Result;

        foreach (var mod in self.ModInventory[((uint) vendor).ToString()])
        {
            var missing = "";
            if (checkInventory)
                missing = vendorData.Sales.Data
                    .Where(destinyVendorSaleItemComponent =>
                        destinyVendorSaleItemComponent.Value.ItemHash == mod.Id)
                    .Where(destinyVendorSaleItemComponent =>
                        destinyVendorSaleItemComponent.Value.SaleStatus == VendorItemStatus.Success).Aggregate(missing,
                        (current, _) => "[*] " + current);

            result = result + $"[{mod.Name}](https://www.light.gg/db/items/{mod.Id}){missing}\n" +
                     $"- {Format.Italics(mod.Description)}\n\n";
        }

        return result;
    }

    public static ModCache FetchInventory(Lang lg, OAuthConfig oauth, DestinyMembership destinyMembership)
    {
        ModCache modCache;

        var path = $"Data/modCache-{lg}.json";

        if (File.Exists(path))
        {
            modCache = ConfigHelper.FromJson<ModCache>(File.ReadAllText(path))!;

            if (modCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return modCache;
        }

        var vendorData = BungieAPI.GetApiClient().Api.Destiny2_GetVendors(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, new[]
                {
                    DestinyComponentType.VendorSales
                }, authToken: oauth.AccessToken)
            .Result;

        var currentTime = DateTime.UtcNow;

        modCache = new ModCache
        {
            InventoryExpires = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day + 1, 17, 0, 0),
            ModInventory = new Dictionary<string, List<Mod>>()
        };

        modCache = PopulateMods(lg, modCache, VendorIds.Ada1, vendorData.Sales.Data);
        modCache = PopulateMods(lg, modCache, VendorIds.Banshee44, vendorData.Sales.Data);

        File.WriteAllText(path, ConfigHelper.ToJson(modCache));

        return modCache;
    }

    private static ModCache PopulateMods(Lang lg, ModCache modCache, VendorIds vendor,
        IReadOnlyDictionary<uint, PersonalDestinyVendorSaleItemSetComponent> salesData)
    {
        modCache.ModInventory.Add(((uint) vendor).ToString(), new List<Mod>());

        var manifestItems = BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lg,
            salesData[(uint) vendor].SaleItems.Values.Select(saleItem => saleItem.ItemHash).ToList());

        var manifestPerks = BungieAPI.GetManifestDefinition<DestinySandboxPerkDefinition>(lg,
            (from inventoryItem in manifestItems
                where inventoryItem.ItemType == DestinyItemType.Mod
                select inventoryItem.Perks.First().PerkHash).ToList());

        var i = 0;
        var j = 0;

        foreach (var (_, value) in salesData[(uint) vendor].SaleItems)
        {
            var item = manifestItems[i];

            if (item.ItemType != DestinyItemType.Mod)
            {
                i++;
                continue;
            }

            var modInfo = manifestPerks[j];

            modCache.ModInventory[((uint) vendor).ToString()].Add(new Mod
            {
                Name = modInfo.DisplayProperties.Name,
                Description = modInfo.DisplayProperties.Description,
                Id = value.ItemHash
            });

            j++;
            i++;
        }

        return modCache;
    }
}