using System.Text.Json;
using Discord;
using Discord.WebSocket;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Components;
using DotNetBungieAPI.Models.Destiny.Definitions.Collectibles;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Models.Destiny.Definitions.PresentationNodes;
using Felicity.Util;
using Serilog;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Models.Caches;

public class XurCache
{
    public XurInventory XurInventory { get; set; } = new();
    public int XurLocation { get; set; }
    public DateTime InventoryExpires { get; set; } = DateTime.UtcNow;
}

public class XurInventory
{
    public WeaponType Weapons { get; set; } = new();
    public ArmorType Armor { get; set; } = new();
}

public class ArmorType
{
    public List<Armor> Exotic { get; set; } = new();
    public string? LegendarySet { get; set; }
}

public class Armor
{
    public uint ArmorId { get; set; }
    public string? ArmorName { get; set; }
    public DestinyClass CharacterClass { get; set; } = DestinyClass.Unknown;
    public Stats Stats { get; set; } = new();
}

public class Stats
{
    public int Mobility { get; set; }
    public int Resilience { get; set; }
    public int Recovery { get; set; }
    public int Discipline { get; set; }
    public int Intellect { get; set; }
    public int Strength { get; set; }
}

public class WeaponType
{
    public List<Weapon> Exotic { get; set; } = new();
    public List<Weapon> Legendary { get; set; } = new();
}

public class Weapon
{
    public uint WeaponId { get; set; }
    public string Name { get; set; } = "";
    public Dictionary<string, Perk> Perks { get; set; } = new();
}

public class Perk
{
    public uint? PerkId { get; set; }
    public string? Perkname { get; set; }
    public string? IconPath { get; set; }
}

public static class ProcessXurData
{
    public static Embed BuildUnavailableEmbed()
    {
        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Xûr, Agent of the Nine",
            IconUrl = BotVariables.Images.XurVendorLogo
        };
        embed.Description =
            $"Xûr is not currently selling his wares.\nHe will arrive <t:{ResetUtils.GetNextWeeklyReset((int)DayOfWeek.Friday).GetTimestamp()}:R>.";
        embed.Color = Color.Red;

        return embed.Build();
    }

    public static Embed BuildEmbed(XurCache self, BaseSocketClient discordClient)
    {
        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Xûr, Agent of the Nine",
            IconUrl = BotVariables.Images.XurVendorLogo
        };
        embed.Description = $"Xûr is currently selling his wares on {Format.Bold(GetXurLocation(self.XurLocation))}";

        var exoticWeapons = WeaponHelper.PopulateWeaponPerks(discordClient, self.XurInventory.Weapons.Exotic, true);
        var legendaryWeapons = WeaponHelper.PopulateWeaponPerks(discordClient, self.XurInventory.Weapons.Legendary, false);

        var exoticArmors = "";
        foreach (var exoticArmor in self.XurInventory.Armor.Exotic)
        {
            exoticArmors +=
                $"[{exoticArmor.ArmorName}]({MiscUtils.GetLightGgLink(exoticArmor.ArmorId)}) [{WeaponHelper.TotalStats(exoticArmor.Stats)}]\n";
            exoticArmors +=
                $"{EmoteHelper.GetEmote(discordClient, "", "Mobility", 0)} {exoticArmor.Stats.Mobility:00} ";
            exoticArmors +=
                $"{EmoteHelper.GetEmote(discordClient, "", "Resilience", 0)} {exoticArmor.Stats.Resilience:00} ";
            exoticArmors +=
                $"{EmoteHelper.GetEmote(discordClient, "", "Recovery", 0)} {exoticArmor.Stats.Recovery:00} ";
            exoticArmors +=
                $"{EmoteHelper.GetEmote(discordClient, "", "Discipline", 0)} {exoticArmor.Stats.Discipline:00} ";
            exoticArmors +=
                $"{EmoteHelper.GetEmote(discordClient, "", "Intellect", 0)} {exoticArmor.Stats.Intellect:00} ";
            exoticArmors +=
                $"{EmoteHelper.GetEmote(discordClient, "", "Strength", 0)} {exoticArmor.Stats.Strength:00}\n";
        }

        embed.AddField("Exotic Weapons", exoticWeapons, true);
        embed.AddField("Exotic Armor", exoticArmors, true);
        embed.AddField("\u200b", '\u200b');
        embed.AddField("Legendary Weapons", legendaryWeapons, true);
        embed.AddField("Legendary Armor",
            $"[{self.XurInventory.Armor.LegendarySet}]({WeaponHelper.BuildLightGGLink(self.XurInventory.Armor.LegendarySet!)})",
            true);

        return embed.Build();
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

    public static async Task<XurCache?> FetchInventory(BungieLocales lg, User oauth, IBungieClient bungieClient)
    {
        XurCache? xurCache;

        var path = $"Data/xurCache-{lg}.json";

        if (File.Exists(path))
        {
            xurCache = JsonSerializer.Deserialize<XurCache>(await File.ReadAllTextAsync(path));

            if (xurCache != null && xurCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return xurCache;
        }

        var characterIdTask = await bungieClient.ApiAccess.Destiny2.GetProfile(oauth.DestinyMembershipType,
            oauth.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            });

        var vendorData = await bungieClient.ApiAccess.Destiny2.GetVendor(oauth.DestinyMembershipType,
            oauth.DestinyMembershipId, characterIdTask.Response.Characters.Data.Keys.First(),
            DefinitionHashes.Vendors.Xûr_2190858386, new[]
            {
                DestinyComponentType.ItemStats, DestinyComponentType.ItemSockets,
                DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                DestinyComponentType.VendorSales
            }, oauth.GetTokenData());

        if (vendorData.Response.Sales.Data.Keys.Count == 0)
        {
            Log.Error("Xûr inventory lookup failed.");
            return null;
        }

        var xurSales = vendorData.Response.Sales.Data;
        var xurSockets = vendorData.Response.ItemComponents.Sockets.Data;
        var xurStats = vendorData.Response.ItemComponents.Stats.Data;

        var xurExotics = new List<DestinyVendorSaleItemComponent>();
        var xurWeps = new List<DestinyVendorSaleItemComponent>();
        var xurArmor = new List<DestinyVendorSaleItemComponent>();

        foreach (var (key, value) in xurSales)
        {
            if (key == 1) // Exotic Engram
                continue;
            if (vendorData.Response.Categories.Data.Categories.ElementAt(0).ItemIndexes.Contains(key))
                xurExotics.Add(value);
            if (vendorData.Response.Categories.Data.Categories.ElementAt(1).ItemIndexes.Contains(key))
                xurWeps.Add(value);
            if (vendorData.Response.Categories.Data.Categories.ElementAt(2).ItemIndexes.Contains(key))
                xurWeps.Add(value);
            if (vendorData.Response.Categories.Data.Categories.ElementAt(3).ItemIndexes.Contains(key))
                xurArmor.Add(value);
        }

        xurCache = new XurCache
        {
            XurLocation = vendorData.Response.Vendor.Data.VendorLocationIndex,
            InventoryExpires = vendorData.Response.Vendor.Data.NextRefreshDate
        };

        var manifestFetch = xurExotics.Select(vendorItem => vendorItem.Item.Hash).ToList();

        foreach (var item in manifestFetch.Where(item => item != null))
        {
            bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>((uint)item!, lg,
                out var manifestItem);

            var vendorItem = xurExotics.FirstOrDefault(destinyVendorSaleItemComponent =>
                destinyVendorSaleItemComponent.Item.Hash == manifestItem.Hash)!;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (manifestItem.ItemType)
            {
                case DestinyItemType.Weapon:
                    xurCache.XurInventory.Weapons.Exotic.Add(new Weapon
                    {
                        Name = manifestItem.DisplayProperties.Name,
                        WeaponId = (uint)vendorItem.Item.Hash!
                    });
                    break;
                case DestinyItemType.Armor:
                    xurCache.XurInventory.Armor.Exotic.Add(new Armor
                    {
                        ArmorId = (uint)vendorItem.Item.Hash!,
                        ArmorName = manifestItem.DisplayProperties.Name,
                        CharacterClass = manifestItem.ClassType,
                        Stats = new Stats
                        {
                            Mobility =
                                xurStats[vendorItem.VendorItemIndex].Stats[DefinitionHashes.Stats.Mobility].Value,
                            Resilience =
                                xurStats[vendorItem.VendorItemIndex].Stats[DefinitionHashes.Stats.Resilience].Value,
                            Recovery =
                                xurStats[vendorItem.VendorItemIndex].Stats[DefinitionHashes.Stats.Recovery].Value,
                            Discipline =
                                xurStats[vendorItem.VendorItemIndex].Stats[DefinitionHashes.Stats.Discipline].Value,
                            Intellect = xurStats[vendorItem.VendorItemIndex].Stats[DefinitionHashes.Stats.Intellect]
                                .Value,
                            Strength = xurStats[vendorItem.VendorItemIndex].Stats[DefinitionHashes.Stats.Strength].Value
                        }
                    });
                    break;
            }
        }

        manifestFetch = xurWeps.Select(vendorItem => vendorItem.Item.Hash).ToList();

        foreach (var item in manifestFetch.Where(item => item != null))
        {
            bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>((uint)item!, lg,
                out var manifestItem);

            var vendorItem = xurWeps.FirstOrDefault(destinyVendorSaleItemComponent =>
                destinyVendorSaleItemComponent.Item.Hash == manifestItem.Hash)!;

            var weapon = new Weapon
            {
                Name = manifestItem.DisplayProperties.Name,
                WeaponId = (uint)vendorItem.Item.Hash!,
                Perks = await WeaponHelper.BuildPerks(bungieClient, lg, manifestItem.Inventory.TierTypeEnumValue,
                    xurSockets[vendorItem.VendorItemIndex])
            };

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (manifestItem.Inventory.TierTypeEnumValue)
            {
                case ItemTierType.Exotic:
                    xurCache.XurInventory.Weapons.Exotic.Add(weapon);
                    break;
                case ItemTierType.Superior:
                    xurCache.XurInventory.Weapons.Legendary.Add(weapon);
                    break;
                default:
                    continue;
            }
        }

        bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(
            (uint)xurArmor.First().Item.Hash!, lg, out var armorSetCollectible);

        var collectibleHash = armorSetCollectible.Collectible.Hash;
        if (collectibleHash != null)
        {
            var hash = (uint)collectibleHash;

            bungieClient.Repository.TryGetDestinyDefinition<DestinyCollectibleDefinition>(hash, lg,
                out var armorSetCollectibleDefinition);

            bungieClient.Repository.TryGetDestinyDefinition<DestinyPresentationNodeDefinition>(
                (uint)armorSetCollectibleDefinition.ParentNodes.First().Hash!, lg, out var parentNode);

            xurCache.XurInventory.Armor.LegendarySet = parentNode.DisplayProperties.Name;
        }

        await File.WriteAllTextAsync($"Data/xurCache-{lg}.json", JsonSerializer.Serialize(xurCache));

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
            DayOfWeek.Friday => currentTime.Hour >= 17,
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