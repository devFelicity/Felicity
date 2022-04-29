using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIHelper;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Entities.Vendors;
using Discord;
using Discord.Interactions;
using Felicity.Enums;
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Commands.SlashCommands;

[Group("vendor", "Group of commands related to vendors and their available items.")]
public class VendorCommands : InteractionModuleBase<SocketInteractionContext>
{
    [RequireOAuthPrecondition]
    [SlashCommand("mods", "Get list of mods currently available at vendors.")]
    public async Task Mods()
    {
        await DeferAsync();

        var oauth = Context.User.OAuth();

        var destinyMembership = oauth.DestinyMembership;
        var vendorData = APIService.GetApiClient().Api.Destiny2_GetVendors(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, new[]
                {
                    DestinyComponentType.VendorSales
                }, authToken: oauth.AccessToken)
            .Result;

        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl =
                    "https://www.bungie.net/common/destiny2_content/icons/DestinyPresentationNodeDefinition_1b273a0d5a6aba677747b2e7412ea6fd.png",
                Name = "Mod Vendors"
            },
            Description = "* = not owned",
            Footer = new EmbedFooterBuilder
            {
                Text = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0}",
                IconUrl = "https://whaskell.pw/images/felicity_circle.jpg"
            }
        };

        embed.AddField("Ada-1", GetMods(vendorData.Sales.Data[(uint) Vendors.Ada1].SaleItems), true);
        embed.AddField("Banshee-44", GetMods(vendorData.Sales.Data[(uint) Vendors.Banshee44].SaleItems), true);

        await FollowupAsync(embed: embed.Build());
    }

    private static string GetMods(Dictionary<int, DestinyVendorSaleItemComponent> saleItems)
    {
        var result = "";

        foreach (var (_, value) in saleItems)
        {
            var item = ManifestConnection.GetInventoryItemById(
                unchecked((int) value.ItemHash));

            if (item.ItemType != DestinyItemType.Mod) continue;

            if (value.SaleStatus == VendorItemStatus.Success)
                result += "*";

            result += $"{item.DisplayProperties.Name}\n";
        }

        return result;
    }
}