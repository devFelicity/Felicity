using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIHelper;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Entities.Items;
using BungieSharper.Entities.Destiny.Entities.Vendors;
using Discord;
using Felicity.Configs;
using Felicity.Enums;
using Felicity.Helpers;
using Felicity.Services;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace Felicity.Structs;

public class XurCache
{
    [J("XurInventory")] public XurInventory XurInventory { get; set; } = new();
    [J("XurLocation")] public int XurLocation { get; set; }
    [J("InventoryExpires")] public DateTime InventoryExpires { get; set; } = DateTime.UtcNow;
}

public class XurInventory
{
    [J("Weapons")] public WeaponType Weapons { get; set; } = new();
    [J("Armor")] public ArmorType Armor { get; set; } = new();
}

public class ArmorType
{
    [J("Exotic")] public List<Armor> Exotic { get; set; } = new();
    [J("Legendary")] public string LegendarySet { get; set; }
}

public class Armor
{
    [J("armorId")] public uint ArmorId { get; set; }
    [J("armorName")] public string ArmorName { get; set; }
    [J("characterClass")] public DestinyClass CharacterClass { get; set; } = DestinyClass.Unknown;
    [J("stats")] public Stats Stats { get; set; } = new();
}

public class Stats
{
    [J("mobility")] public int Mobility { get; set; }
    [J("resilience")] public int Resilience { get; set; }
    [J("recovery")] public int Recovery { get; set; }
    [J("discipline")] public int Discipline { get; set; }
    [J("intellect")] public int Intellect { get; set; }
    [J("strength")] public int Strength { get; set; }
}

public class WeaponType
{
    [J("Exotic")] public List<Weapon> Exotic { get; set; } = new();
    [J("Legendary")] public List<Weapon> Legendary { get; set; } = new();
}

public class Weapon
{
    [J("id")] public uint WeaponId { get; set; }
    [J("name")] public string Name { get; set; } = "";

    [J("perks", NullValueHandling = N.Ignore)]
    public Dictionary<string, Perk> Perks { get; set; } = new();
}

public class Perk
{
    [J("perkid")] public uint? PerkId { get; set; }
    [J("perkname")] public string Perkname { get; set; }
    [J("iconPath")] public string IconPath { get; set; }
}

public static class ProcessXurData
{
    private static string ToJson(this XurCache self)
    {
        return JsonConvert.SerializeObject(self, Converter.Settings);
    }

    private static XurCache FromJson(string json)
    {
        return JsonConvert.DeserializeObject<XurCache>(json, Converter.Settings);
    }

    public static Embed BuildEmbed(this XurCache self)
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = "https://www.bungie.net/img/destiny_content/vendor/icons/xur_large_icon.png",
                Name = "Xûr, Agent of the Nine"
            },
            Description = "Xûr is currently selling his wares on " + Format.Bold(GetXurLocation(self.XurLocation)),
            Footer = new EmbedFooterBuilder
            {
                Text = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0}",
                IconUrl = "https://whaskell.pw/images/felicity_circle.jpg"
            }
        };

        var exoticWeapons = PopulateWeaponPerks(self.XurInventory.Weapons.Exotic);
        var legendaryWeapons = PopulateWeaponPerks(self.XurInventory.Weapons.Legendary, false);

        var exoticArmors = "";
        foreach (var exoticArmor in self.XurInventory.Armor.Exotic)
        {
            exoticArmors +=
                $"[{exoticArmor.ArmorName}]({BuildLightGGLink(exoticArmor.ArmorId)}) [{TotalStats(exoticArmor.Stats)}]\n";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Mobility")} {exoticArmor.Stats.Mobility:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Resilience")} {exoticArmor.Stats.Resilience:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Recovery")} {exoticArmor.Stats.Recovery:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Discipline")} {exoticArmor.Stats.Discipline:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Intellect")} {exoticArmor.Stats.Intellect:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Strength")} {exoticArmor.Stats.Strength:00}\n";
        }

        embed.AddField("Exotic Weapons", exoticWeapons, true);
        embed.AddField("Exotic Armor", exoticArmors, true);
        embed.AddField("\u200b", '\u200b');
        embed.AddField("Legendary Weapons", legendaryWeapons, true);
        embed.AddField("Legendary Armor",
            $"[{self.XurInventory.Armor.LegendarySet}]({BuildLightGGLink(self.XurInventory.Armor.LegendarySet)})",
            true);

        return embed.Build();
    }

    private static string PopulateWeaponPerks(List<Weapon> weapons, bool gunsmithLink = true)
    {
        var result = "";

        foreach (var weapon in weapons)
            if (weapon.Perks.Count == 0)
            {
                result += $"[{weapon.Name}]({BuildLightGGLink(weapon.WeaponId)}/)\n\n";
            }
            else
            {
                if (gunsmithLink)
                    result += $"[{weapon.Name}]({BuildGunsmithLink(weapon.WeaponId, weapon.Perks)})\n";
                else
                    result += $"[{weapon.Name}]({BuildLightGGLink(weapon.WeaponId)}/) | ";

                foreach (var (_, value) in weapon.Perks)
                    result += EmoteHelper.GetEmote(value.IconPath, value.Perkname);

                result += "\n";
            }

        return result;
    }

    private static string BuildLightGGLink(uint itemId)
    {
        return $"https://light.gg/db/items/{itemId}";
    }

    private static string BuildLightGGLink(string armorLegendarySet)
    {
        var search = armorLegendarySet.ToLower().Replace("suit", "")
            .Replace("set", "").Replace("armor", "");
        return $"https://www.light.gg/db/all?page=1&f=12({search}),3";
    }

    private static string BuildGunsmithLink(uint exoticWeaponWeaponId, Dictionary<string, Perk> exoticWeaponPerks)
    {
        var result = $"https://d2gunsmith.com/w/{exoticWeaponWeaponId}?s=";

        foreach (var value in exoticWeaponPerks.Values)
            result += value.PerkId + ",";

        return result.TrimEnd(',');
    }

    private static int TotalStats(Stats exoticArmorStats)
    {
        return exoticArmorStats.Mobility + exoticArmorStats.Resilience + exoticArmorStats.Recovery +
               exoticArmorStats.Discipline + exoticArmorStats.Intellect + exoticArmorStats.Strength;
    }

    private static string GetXurLocation(int selfXurLocation)
    {
        return selfXurLocation switch
        {
            1 => "the EDZ.",
            2 => "Nessus.",
            _ => "an unknown location."
        };
    }

    private static Dictionary<string, Perk> BuildPerks(TierType inventoryTierType, DestinyItemSocketsComponent xurPerk)
    {
        var response = new Dictionary<string, Perk>();

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        var goodPerkList = inventoryTierType switch
        {
            TierType.Exotic => new[] {1, 2, 3, 4},
            TierType.Superior => new[] {3, 4},
            _ => Array.Empty<int>()
        };

        if (goodPerkList.Length == 0)
            return response;

        var i = 0;

        foreach (var destinyItemSocketState in xurPerk.Sockets)
        {
            if (goodPerkList.Contains(i))
            {
                if (destinyItemSocketState.PlugHash == null) continue;
                if (!destinyItemSocketState.IsVisible) continue;

                var item = ManifestConnection.GetInventoryItemById(unchecked((int) destinyItemSocketState.PlugHash));

                response.Add(response.Count.ToString(), new Perk
                {
                    PerkId = destinyItemSocketState.PlugHash,
                    Perkname = item.DisplayProperties.Name,
                    IconPath = item.DisplayProperties.Icon
                });
            }

            i++;
        }

        return response;
    }

    public static XurCache FetchInventory(OAuthConfig oauth, DestinyMembership destinyMembership)
    {
        XurCache xurCache;

        const string path = "Data/xurCache.json";

        if (File.Exists(path))
        {
            xurCache = FromJson(File.ReadAllText(path));

            if (xurCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return xurCache;
        }

        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendor(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, (uint) Vendors.Xur, new[]
                {
                    DestinyComponentType.ItemStats, DestinyComponentType.ItemSockets,
                    DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                    DestinyComponentType.VendorSales
                },
                oauth.AccessToken)
            .Result;

        var xurSales = vendorData.Sales.Data;
        var xurSockets = vendorData.ItemComponents.Sockets.Data;
        var xurStats = vendorData.ItemComponents.Stats.Data;

        var xurExotics = new List<DestinyVendorSaleItemComponent>();
        var xurWeps = new List<DestinyVendorSaleItemComponent>();
        var xurArmor = new List<DestinyVendorSaleItemComponent>();

        foreach (var (key, value) in xurSales)
        {
            if (key == 1) // Exotic Engram
                continue;
            if (vendorData.Categories.Data.Categories.ElementAt(0).ItemIndexes.Contains(key))
                xurExotics.Add(value);
            if (vendorData.Categories.Data.Categories.ElementAt(1).ItemIndexes.Contains(key))
                xurWeps.Add(value);
            if (vendorData.Categories.Data.Categories.ElementAt(2).ItemIndexes.Contains(key))
                xurWeps.Add(value);
            if (vendorData.Categories.Data.Categories.ElementAt(3).ItemIndexes.Contains(key))
                xurArmor.Add(value);
        }

        xurCache = new XurCache
        {
            XurLocation = vendorData.Vendor.Data.VendorLocationIndex,
            InventoryExpires = vendorData.Vendor.Data.NextRefreshDate
        };

        foreach (var vendorItem in xurExotics)
        {
            var item = ManifestConnection.GetInventoryItemById(unchecked((int) vendorItem.ItemHash));

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (item.ItemType)
            {
                case DestinyItemType.Weapon:
                    xurCache.XurInventory.Weapons.Exotic.Add(new Weapon
                    {
                        Name = item.DisplayProperties.Name,
                        WeaponId = vendorItem.ItemHash
                    });
                    break;
                case DestinyItemType.Armor:
                    xurCache.XurInventory.Armor.Exotic.Add(new Armor
                    {
                        ArmorId = vendorItem.ItemHash,
                        ArmorName = item.DisplayProperties.Name,
                        CharacterClass = item.ClassType,
                        Stats = new Stats
                        {
                            Mobility = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Mobility].Value,
                            Resilience = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Resilience].Value,
                            Recovery = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Recovery].Value,
                            Discipline = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Discipline].Value,
                            Intellect = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Intellect].Value,
                            Strength = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Strength].Value
                        }
                    });
                    break;
            }
        }

        foreach (var vendorItem in xurWeps)
        {
            var hash = unchecked((int) vendorItem.ItemHash);
            var item = ManifestConnection.GetInventoryItemById(hash);

            var weapon = new Weapon
            {
                Name = item.DisplayProperties.Name,
                WeaponId = vendorItem.ItemHash,
                Perks = BuildPerks(item.Inventory.TierType, xurSockets[vendorItem.VendorItemIndex])
            };

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (item.Inventory.TierType)
            {
                case TierType.Exotic:
                    xurCache.XurInventory.Weapons.Exotic.Add(weapon);
                    break;
                case TierType.Superior:
                    xurCache.XurInventory.Weapons.Legendary.Add(weapon);
                    break;
                default:
                    continue;
            }
        }

        var armorSetCollectibleHash =
            ManifestConnection.GetInventoryItemById(unchecked((int) xurArmor.First().ItemHash)).CollectibleHash;

        if (armorSetCollectibleHash != null)
        {
            var armorSetCollectibleDefinition =
                ManifestConnection.GetItemCollectibleId(unchecked((int) armorSetCollectibleHash));

            var parentNode =
                ManifestConnection.GetPresentationNodeId(
                    unchecked((int) armorSetCollectibleDefinition.ParentNodeHashes.First()));

            xurCache.XurInventory.Armor.LegendarySet = parentNode.DisplayProperties.Name;
        }

        File.WriteAllText("Data/xurCache.json", xurCache.ToJson());

        return xurCache;
    }
}