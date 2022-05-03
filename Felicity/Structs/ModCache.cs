using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIHelper;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Responses;
using Discord;
using Felicity.Configs;
using Felicity.Enums;
using Felicity.Helpers;
using Felicity.Services;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Structs;

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
    private static ModCache FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ModCache>(json, Converter.Settings);
    }

    private static string ToJson(this ModCache self)
    {
        return JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public static Embed BuildEmbed(this ModCache self, OAuthConfig oauth, DestinyMembership destinyMembership,
        bool checkInventory = false)
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = "https://bungie.net/common/destiny2_content/icons/23599621d4c63076c647384028d96ca4.png",
                Name = "Mod Vendors"
            },
            Description = checkInventory
                ? "Ada-1 and Banshee-44 can both be found in the Tower.\n* = not owned."
                : "You can use `/vendor mods` to view unacquired mods from this list.\nAda-1 and Banshee-44 can both be found in the Tower.",
            Footer = new EmbedFooterBuilder
            {
                Text = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0} | Links go to light.gg",
                IconUrl = "https://whaskell.pw/images/felicity_circle.jpg"
            }
        };

        var adaMods = GetMods(self, Vendors.Ada1, checkInventory, oauth, destinyMembership);
        var bansheeMods = GetMods(self, Vendors.Banshee44, checkInventory, oauth, destinyMembership);

        embed.AddField("Ada-1", adaMods, true);
        embed.AddField("Banshee-44", bansheeMods, true);

        return embed.Build();
    }

    private static string GetMods(ModCache self, Vendors vendor, bool checkInventory, OAuthConfig oauth,
        DestinyMembership destinyMembership)
    {
        var result = "";

        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendor(destinyMembership.CharacterIds.First(),
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
                        (current, _) => current + "*");

            result = result + $"[{mod.Name}](https://www.light.gg/db/items/{mod.Id}){missing}\n" +
                     $"- {Format.Italics(mod.Description)}\n\n";
        }

        return result;
    }

    public static ModCache FetchInventory(OAuthConfig oauth, DestinyMembership destinyMembership)
    {
        ModCache modCache;

        const string path = "Data/modCache.json";

        if (File.Exists(path))
        {
            modCache = FromJson(File.ReadAllText(path));

            if (modCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return modCache;
        }

        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendors(destinyMembership.CharacterIds.First(),
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

        modCache = PopulateMods(modCache, Vendors.Ada1, vendorData.Sales.Data);
        modCache = PopulateMods(modCache, Vendors.Banshee44, vendorData.Sales.Data);

        File.WriteAllText(path, modCache.ToJson());

        return modCache;
    }

    private static ModCache PopulateMods(ModCache modCache, Vendors vendor,
        IReadOnlyDictionary<uint, PersonalDestinyVendorSaleItemSetComponent> salesData)
    {
        modCache.ModInventory.Add(((uint) vendor).ToString(), new List<Mod>());

        foreach (var (_, value) in salesData[(uint) vendor].SaleItems)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) value.ItemHash));

            if (item.ItemType != DestinyItemType.Mod)
                continue;

            var modInfo =
                ManifestConnection.GetSandboxDefinitionById(unchecked((int) item.Perks.First().PerkHash));

            modCache.ModInventory[((uint) vendor).ToString()].Add(new Mod
            {
                Name = modInfo.DisplayProperties.Name,
                Description = modInfo.DisplayProperties.Description,
                Id = value.ItemHash
            });
        }

        return modCache;
    }
}