using System.Web;
using Felicity.Models.Caches;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Felicity.Util;

internal static class WeaponHelper
{
    public static string GetMasterworkType(uint hash)
    {
        return hash switch
        {
            150943607 => "Range",
            199695019 => "Range",
            518224747 => "Handling",
            892374263 => "Accuracy",
            1486919755 => "Impact",
            1590375901 => "Stability",
            2203506848 => "Draw Time",
            3353797898 => "Charge Time",
            3673787993 => "Shield Duration",
            3928770367 => "Blast Radius",
            4105787909 => "Velocity",
            4283235143 => "Reload",
            _ => "Unknown"
        };
    }

    public static string BuildLightGGLink(string armorLegendarySet)
    {
        var search = armorLegendarySet.ToLower().Replace("suit", "")
            .Replace("set", "").Replace("armor", "");
        return $"https://www.light.gg/db/all?page=1&f=12({HttpUtility.UrlEncode(search.TrimEnd(' '))}),3";
    }

    public static string BuildGunsmithLink(uint exoticWeaponWeaponId, Dictionary<string, Perk> exoticWeaponPerks)
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