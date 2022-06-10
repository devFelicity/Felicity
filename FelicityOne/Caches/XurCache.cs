using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Definitions;
using BungieSharper.Entities.Destiny.Definitions.Collectibles;
using BungieSharper.Entities.Destiny.Definitions.Presentation;
using BungieSharper.Entities.Destiny.Entities.Items;
using BungieSharper.Entities.Destiny.Entities.Vendors;
using Discord;
using DotNetBungieAPI.HashReferences;
using FelicityOne.Configs;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using Serilog;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

namespace FelicityOne.Caches;

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
    public static Embed BuildUnavailableEmbed()
    {
        return Extensions.GenerateMessageEmbed("Xûr, Agent of the Nine",
                Images.XurVendorLogo,
                $"Xûr is not currently selling his wares.\nHe will arrive <t:{EndGameGl.GetNextWeeklyReset((int) DayOfWeek.Friday).GetTimestamp()}:R>.")
            .Build();
    }

    public static Embed BuildEmbed(this XurCache self)
    {
        var embed = Extensions.GenerateVendorEmbed("Xûr, Agent of the Nine",
            Images.XurVendorLogo,
            "Xûr is currently selling his wares on " + Format.Bold(GetXurLocation(self.XurLocation)));

        var exoticWeapons = PopulateWeaponPerks(self.XurInventory.Weapons.Exotic, true);
        var legendaryWeapons = PopulateWeaponPerks(self.XurInventory.Weapons.Legendary, false);

        var exoticArmors = "";
        foreach (var exoticArmor in self.XurInventory.Armor.Exotic)
        {
            exoticArmors +=
                $"[{exoticArmor.ArmorName}]({WeaponHelper.BuildLightGGLink(exoticArmor.ArmorId)}) [{WeaponHelper.TotalStats(exoticArmor.Stats)}]\n";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Mobility", 0)} {exoticArmor.Stats.Mobility:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Resilience", 0)} {exoticArmor.Stats.Resilience:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Recovery", 0)} {exoticArmor.Stats.Recovery:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Discipline", 0)} {exoticArmor.Stats.Discipline:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Intellect", 0)} {exoticArmor.Stats.Intellect:00} ";
            exoticArmors += $"{EmoteHelper.GetEmote("", "Strength", 0)} {exoticArmor.Stats.Strength:00}\n";
        }

        embed.AddField("Exotic Weapons", exoticWeapons, true);
        embed.AddField("Exotic Armor", exoticArmors, true);
        embed.AddField("\u200b", '\u200b');
        embed.AddField("Legendary Weapons", legendaryWeapons, true);
        embed.AddField("Legendary Armor",
            $"[{self.XurInventory.Armor.LegendarySet}]({WeaponHelper.BuildLightGGLink(self.XurInventory.Armor.LegendarySet)})",
            true);

        return embed.Build();
    }

    private static string PopulateWeaponPerks(List<Weapon> weapons, bool gunsmithLink)
    {
        var result = "";

        foreach (var weapon in weapons)
            if (weapon.Perks.Count == 0)
            {
                result += $"[{weapon.Name}]({WeaponHelper.BuildLightGGLink(weapon.WeaponId)}/)\n\n";
            }
            else
            {
                if (gunsmithLink)
                    result += $"[{weapon.Name}]({WeaponHelper.BuildGunsmithLink(weapon.WeaponId, weapon.Perks)})\n";
                else
                    result += $"[{weapon.Name}]({WeaponHelper.BuildLightGGLink(weapon.WeaponId)}/) | ";

                foreach (var (_, value) in weapon.Perks)
                    result += EmoteHelper.GetEmote(value.IconPath, value.Perkname, value.PerkId);

                result += "\n";
            }

        return result;
    }

    private static string GetXurLocation(int xurLocation)
    {
        return xurLocation switch
        {
            0 => "the Tower (Hangar)",
            1 => "the EDZ (Winding Cove)",
            2 => "Nessus (Watcher's Grave)",
            _ => "an unknown location."
        };
    }

    private static Dictionary<string, Perk> BuildPerks(Lang lg, TierType inventoryTierType,
        DestinyItemSocketsComponent xurPerk)
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

                response.Add(response.Count.ToString(), new Perk
                {
                    PerkId = destinyItemSocketState.PlugHash
                });
            }

            i++;
        }

        var fetchList = (from keyPair in response
                let valuePerkId = keyPair.Value.PerkId
                where valuePerkId != null
                select (uint) valuePerkId)
            .ToList();

        var manifestFetch = BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lg, fetchList);

        foreach (var perk in response)
        foreach (var destinyInventoryItemDefinition in manifestFetch.Where(destinyInventoryItemDefinition =>
                     perk.Value.PerkId == destinyInventoryItemDefinition.Hash))
        {
            perk.Value.Perkname = destinyInventoryItemDefinition.DisplayProperties.Name;
            perk.Value.IconPath = destinyInventoryItemDefinition.DisplayProperties.Icon;
        }

        return response;
    }

    public static XurCache? FetchInventory(Lang lg, OAuthConfig oauth, DestinyMembership destinyMembership)
    {
        XurCache? xurCache;

        var path = $"Data/xurCache-{lg}.json";

        if (File.Exists(path))
        {
            xurCache = ConfigHelper.FromJson<XurCache>(File.ReadAllText(path));

            if (xurCache != null && xurCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return xurCache;
        }

        var vendorData = BungieAPI.GetApiClient()
            .Api.Destiny2_GetVendor(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, DefinitionHashes.Vendors.Xûr_2190858386, new[]
                {
                    DestinyComponentType.ItemStats, DestinyComponentType.ItemSockets,
                    DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                    DestinyComponentType.VendorSales
                },
                oauth.AccessToken)
            .Result;

        if (vendorData.Sales.Data.Keys.Count == 0)
        {
            Log.Error("Xur inventory lookup failed.");
            return null;
        }

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

        var manifestFetch = xurExotics.Select(vendorItem => vendorItem.ItemHash).ToList();
        var manifestResult = BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lg, manifestFetch);

        foreach (var item in manifestResult)
        {
            var vendorItem = xurExotics.FirstOrDefault(destinyVendorSaleItemComponent =>
                destinyVendorSaleItemComponent.ItemHash == item.Hash)!;

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
                            Resilience =
                                xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Resilience].Value,
                            Recovery = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Recovery].Value,
                            Discipline =
                                xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Discipline].Value,
                            Intellect = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Intellect].Value,
                            Strength = xurStats[vendorItem.VendorItemIndex].Stats[(uint) StatIds.Strength].Value
                        }
                    });
                    break;
            }
        }

        manifestFetch = xurWeps.Select(vendorItem => vendorItem.ItemHash).ToList();
        manifestResult = BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lg, manifestFetch);

        foreach (var item in manifestResult)
        {
            var vendorItem = xurWeps.FirstOrDefault(destinyVendorSaleItemComponent =>
                destinyVendorSaleItemComponent.ItemHash == item.Hash)!;

            var weapon = new Weapon
            {
                Name = item.DisplayProperties.Name,
                WeaponId = vendorItem.ItemHash,
                Perks = BuildPerks(lg, item.Inventory.TierType, xurSockets[vendorItem.VendorItemIndex])
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

        var armorSetCollectible =
            BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lg, new[] {xurArmor.First().ItemHash});

        var collectibleHash = armorSetCollectible.First().CollectibleHash;
        if (collectibleHash != null)
        {
            var hash = (uint) collectibleHash;
            var armorSetCollectibleDefinition =
                BungieAPI.GetManifestDefinition<DestinyCollectibleDefinition>(lg, new[] {hash});

            var parentNode =
                BungieAPI.GetManifestDefinition<DestinyPresentationNodeDefinition>(lg,
                    new[] {armorSetCollectibleDefinition.First().ParentNodeHashes.First()});

            xurCache.XurInventory.Armor.LegendarySet = parentNode.First().DisplayProperties.Name;
        }

        File.WriteAllText($"Data/xurCache-{lg}.json", ConfigHelper.ToJson(xurCache));

        return xurCache;
    }

    public static bool IsXurHere()
    {
        var currentTime = DateTime.UtcNow;
        return currentTime.DayOfWeek switch
        {
            DayOfWeek.Saturday => true,
            DayOfWeek.Sunday => true,
            DayOfWeek.Monday => true,
            DayOfWeek.Tuesday => currentTime.Hour < 17,
            DayOfWeek.Wednesday => false,
            DayOfWeek.Thursday => false,
            DayOfWeek.Friday => currentTime.Hour > 17,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static void ClearCache()
    {
        foreach (var enumerateFile in Directory.EnumerateFiles("Data"))
            if (enumerateFile.StartsWith("xurCache"))
                File.Delete(enumerateFile);
    }
}