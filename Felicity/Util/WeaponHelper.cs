using Discord.WebSocket;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Components;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models.Caches;
using System.Web;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Felicity.Util;

internal static class WeaponHelper
{
    public static string PopulateWeaponPerks(
        BaseSocketClient discordClient,
        List<Weapon> weapons, 
        bool gunsmithLink)
    {
        var result = "";

        foreach (var weapon in weapons)
        {
            if (weapon.DestinyItemType != null) result += EmoteHelper.GetItemType(weapon.DestinyItemType);

            if (weapon.Perks.Count == 0)
            {
                result += $"[{weapon.Name}]({MiscUtils.GetLightGgLink(weapon.WeaponId)}/)\n\n";
            }
            else
            {
                if (gunsmithLink)
                    result += $"[{weapon.Name}]({BuildGunsmithLink(weapon.WeaponId, weapon.Perks)})\n";
                else
                    result += $"[{weapon.Name}]({MiscUtils.GetLightGgLink(weapon.WeaponId)}/) | ";

                foreach (var (_, value) in weapon.Perks)
                    result += EmoteHelper.GetEmote(discordClient, value.IconPath!, value.Perkname!, value.PerkId);

                result += "\n";
            }
        }

        return result;
    }

    public static Task<Dictionary<string, Perk>> BuildPerks(
        IBungieClient bungieClient,
        ItemTierType inventoryTierType,
        DestinyItemSocketsComponent weaponPerk)
    {
        var response = new Dictionary<string, Perk>();

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        var goodPerkList = inventoryTierType switch
        {
            ItemTierType.Exotic => new[] { 1, 2, 3, 4 },
            ItemTierType.Superior => new[] { 3, 4 },
            _ => Array.Empty<int>()
        };

        if (goodPerkList.Length == 0)
            return Task.FromResult(response);

        var i = 0;

        foreach (var destinyItemSocketState in weaponPerk.Sockets)
        {
            if (goodPerkList.Contains(i))
            {
                if (destinyItemSocketState.Plug.Hash == null) continue;
                if (!destinyItemSocketState.IsVisible) continue;

                response.Add(response.Count.ToString(), new Perk
                {
                    PerkId = destinyItemSocketState.Plug.Hash
                });
            }

            i++;
        }

        var fetchList = (from keyPair in response
                let valuePerkId = keyPair.Value.PerkId
                where valuePerkId != null
                select (uint)valuePerkId)
            .ToList();

        foreach (var fetchPerk in fetchList)
        {
            bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(fetchPerk,
                out var manifestFetch);

            foreach (var perk in response.Where(perk => perk.Value.PerkId == manifestFetch.Hash))
            {
                perk.Value.Perkname = manifestFetch.DisplayProperties.Name;
                perk.Value.IconPath = manifestFetch.DisplayProperties.Icon.RelativePath;
            }
        }

        return Task.FromResult(response);
    }

    public static string BuildLightGGLink(string armorLegendarySet)
    {
        var search = armorLegendarySet.ToLower().Replace("suit", "")
            .Replace("set", "").Replace("armor", "");
        return $"https://www.light.gg/db/all?page=1&f=12({HttpUtility.UrlEncode(search.TrimEnd(' '))}),3";
    }

    private static string BuildGunsmithLink(
        uint exoticWeaponWeaponId,
        Dictionary<string, Perk> exoticWeaponPerks)
    {
        var result = $"https://d2gunsmith.com/w/{exoticWeaponWeaponId}?s=";

        result = exoticWeaponPerks.Values.Aggregate(result, (current, value) => current + (value.PerkId + ","));

        return result.TrimEnd(',');
    }

    public static int TotalStats(Stats exoticArmorStats)
    {
        return exoticArmorStats.Mobility + exoticArmorStats.Resilience + exoticArmorStats.Recovery +
               exoticArmorStats.Discipline + exoticArmorStats.Intellect + exoticArmorStats.Strength;
    }
}