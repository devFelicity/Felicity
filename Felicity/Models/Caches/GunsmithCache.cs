using System.Text.Json;
using Discord;
using Discord.WebSocket;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Util;
using Serilog;

namespace Felicity.Models.Caches;

public class GunsmithCache
{
    public DateTime InventoryExpires { get; set; } = DateTime.UtcNow;
    public List<Weapon> GunsmithInventory { get; set; } = new();
}

public static class ProcessGunsmithData
{
    public static Embed BuildEmbed(GunsmithCache self, BaseSocketClient discordClient)
    {
        var embed = Embeds.MakeBuilder();
        embed.Title = "Banshee-44";
        embed.ThumbnailUrl = BotVariables.Images.GunsmithVendorLogo;
        embed.Description =
            "Banshee-44 has lived many lives. As master weaponsmith for the Tower, he supplies Guardians with only the best.";

        var weapons = WeaponHelper.PopulateWeaponPerks(discordClient, self.GunsmithInventory, false);
        embed.AddField("Weapons", weapons, true);

        return embed.Build();
    }

    public static async Task<GunsmithCache?> FetchInventory(BungieLocales lg, User oauth, IBungieClient bungieClient)
    {
        /*var path = $"Data/gsCache-{lg}.json";

        if (File.Exists(path))
        {
            gsCache = JsonSerializer.Deserialize<GunsmithCache>(await File.ReadAllTextAsync(path));

            if (gsCache != null && gsCache.InventoryExpires < DateTime.UtcNow)
                File.Delete(path);
            else
                return gsCache;
        }*/

        var characterIdTask = await bungieClient.ApiAccess.Destiny2.GetProfile(oauth.DestinyMembershipType,
            oauth.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            });

        var vendorData = await bungieClient.ApiAccess.Destiny2.GetVendor(oauth.DestinyMembershipType,
            oauth.DestinyMembershipId, characterIdTask.Response.Characters.Data.Keys.First(),
            DefinitionHashes.Vendors.Banshee44_672118013, new[]
            {
                DestinyComponentType.ItemSockets,
                DestinyComponentType.VendorCategories,
                DestinyComponentType.VendorSales
            }, oauth.GetTokenData());

        if (vendorData.Response.Sales.Data.Keys.Count == 0)
        {
            Log.Error("Gunsmith inventory lookup failed.");
            return null;
        }

        var weaponList = new List<Weapon>();

        foreach (var itemIndex in vendorData.Response.Categories.Data.Categories.ElementAt(2).ItemIndexes)
        {
            if (!vendorData.Response.Sales.Data[itemIndex].Item.TryGetDefinition(out var itemDefinition, lg))
                continue;

            var weapon = new Weapon
            {
                Name = itemDefinition.DisplayProperties.Name,
                DestinyItemType = itemDefinition.ItemSubType,
                WeaponId = itemDefinition.Hash,
                Perks = await WeaponHelper.BuildPerks(bungieClient, lg, ItemTierType.Superior,
                    vendorData.Response.ItemComponents.Sockets.Data[itemIndex])
            };

            weaponList.Add(weapon);
        }

        var gsCache = new GunsmithCache
        {
            InventoryExpires = ResetUtils.GetNextDailyReset(),
            GunsmithInventory = weaponList
        };

        // await File.WriteAllTextAsync($"Data/gsCache-{lg}.json", JsonSerializer.Serialize(gsCache));

        return gsCache;
    }
}