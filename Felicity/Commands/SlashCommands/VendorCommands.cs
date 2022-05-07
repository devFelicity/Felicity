using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIHelper;
using BungieSharper.Entities.Destiny;
using Discord;
using Discord.Interactions;
using Felicity.Enums;
using Felicity.Helpers;
using Felicity.Services;
using Felicity.Structs;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Commands.SlashCommands;

[RequireOAuth]
[Group("vendor", "Group of commands related to vendors and their available items.")]
public class VendorCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("xur", "Fetch Xûr inventory which includes D2Gunsmith and LightGG links.")]
    public async Task Xur()
    {
        await DeferAsync();

        if (ProcessXurData.IsXurHere())
        {
            var oauth = Context.User.OAuth();
            var destinyMembership = oauth.DestinyMembership;

            var xurCache = ProcessXurData.FetchInventory(oauth, destinyMembership);

            await FollowupAsync(embed: xurCache.BuildEmbed());
        }
        else
        {
            if (File.Exists("Data/xurCache.json"))
                File.Delete("Data/xurCache.json");

            await FollowupAsync(embed: ProcessXurData.BuildUnavailableEmbed());
        }
    }

    [SlashCommand("saint14", "Fetch Saint-14 (Trials of Osiris) reputation rewards for the week.")]
    public async Task Saint()
    {
        await DeferAsync();

        var oauth = Context.User.OAuth();
        var destinyMembership = oauth.DestinyMembership;

        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendor(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, (uint) VendorIds.Saint14, new[]
                {
                    DestinyComponentType.ItemPerks, DestinyComponentType.ItemSockets,
                    DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                    DestinyComponentType.VendorSales
                },
                oauth.AccessToken)
            .Result;

        if (vendorData.Vendor.Data.Progression.CurrentResetCount >= 3)
        {
            await FollowupAsync(embed: Extensions.GenerateVendorEmbed("Saint-14", Images.SaintVendorLogo,
                    $"Because you've reset your rank {Format.Bold(vendorData.Vendor.Data.Progression.CurrentResetCount.ToString())} times, " +
                    "Saint-14 no longer sells weapons with fixed rolls for you.")
                .Build(), ephemeral: true);
            return;
        }

        var repRewards = vendorData.Categories.Data.Categories.ElementAt(3).ItemIndexes;

        var embed = Extensions.GenerateVendorEmbed("Saint-14", Images.SaintVendorLogo,
            "A legendary hero and the former Titan Vanguard, Saint-14 now manages the PvP gamemode Trials of Osiris.\n\n" +
            "These rewards change perks based on weekly reset.");

        var i = 0;

        foreach (var repReward in repRewards)
        {
            var reward = vendorData.Sales.Data[repReward];

            if (reward.Quantity != 1)
                continue;

            var manifestItem = ManifestConnection.GetInventoryItemById(unchecked((int) reward.ItemHash));

            if (manifestItem.ItemType != DestinyItemType.Weapon)
                continue;

            var name = manifestItem.DisplayProperties.Name;

            var fullMessage = $"[{name}]({WeaponHelper.BuildLightGGLink(manifestItem.Hash)})\n";

            var plugHash1 = vendorData.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(3).PlugHash;
            if (plugHash1 != null)
            {
                var perk1 = ManifestConnection.GetInventoryItemById(unchecked((int) plugHash1));
                fullMessage += EmoteHelper.GetEmote(perk1.DisplayProperties.Icon, perk1.DisplayProperties.Name);
            }

            var plugHash2 = vendorData.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(4).PlugHash;
            if (plugHash2 != null)
            {
                var perk2 = ManifestConnection.GetInventoryItemById(unchecked((int) plugHash2));
                fullMessage += EmoteHelper.GetEmote(perk2.DisplayProperties.Icon, perk2.DisplayProperties.Name);
            }

            var fieldName = i == 0 ? "Rank 10" : "Rank 16";

            embed.AddField(new EmbedFieldBuilder
            {
                IsInline = true,
                Name = fieldName,
                Value = fullMessage
            });

            i++;
        }

        embed.AddField(new EmbedFieldBuilder
        {
            IsInline = true,
            Name = "Current Rank",
            Value = vendorData.Vendor.Data.Progression.Level
        });

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("mods", "Get list of mods currently available at vendors.")]
    public async Task Mods()
    {
        await DeferAsync();

        var oauth = Context.User.OAuth();
        var destinyMembership = oauth.DestinyMembership;

        var modCache = ProcessModData.FetchInventory(oauth, destinyMembership);

        await FollowupAsync(embed: modCache.BuildEmbed(oauth, destinyMembership, true));
    }
}