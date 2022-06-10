using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Definitions;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.HashReferences;
using FelicityOne.Caches;
using FelicityOne.Enums;
using FelicityOne.Events;
using FelicityOne.Helpers;
using Serilog;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands.SlashCommands;

[RequireOAuth]
[Group("vendor", "Group of commands related to vendors and their available items.")]
public class Vendor : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("xur", "Fetch Xûr inventory which includes D2Gunsmith and LightGG links.")]
    public async Task Xur()
    {
        await DeferAsync();

        if (ProcessXurData.IsXurHere())
        {
            var oauth = Context.User.OAuth();
            var destinyMembership = oauth.DestinyMembership;

            var lg = Context.Guild.Language();

            if (!File.Exists($"Data/xurCache-{lg}.json"))
                await FollowupAsync("Populating vendor data, this might take some time...");

            var xurCache = ProcessXurData.FetchInventory(lg, oauth, destinyMembership);

            if (xurCache != null)
                await FollowupAsync(embed: xurCache.BuildEmbed());
            else
                Log.Error("Unable to parse Xûr inventory.");
        }
        else
        {
            ProcessXurData.ClearCache();

            await FollowupAsync(embed: ProcessXurData.BuildUnavailableEmbed());
        }
    }

    [SlashCommand("saint14", "Fetch Saint-14 (Trials of Osiris) reputation rewards for the week.")]
    public async Task Saint14()
    {
        await DeferAsync();

        var oauth = Context.User.OAuth();
        var destinyMembership = oauth.DestinyMembership;

        var vendorData = BungieAPI.GetApiClient().Api.Destiny2_GetVendor(destinyMembership.CharacterIds.First(),
                destinyMembership.MembershipId,
                destinyMembership.MembershipType, DefinitionHashes.Vendors.Saint14, new[]
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

        var repRewards = vendorData.Categories.Data.Categories.ElementAt(1).ItemIndexes;

        var embed = Extensions.GenerateVendorEmbed("Saint-14", Images.SaintVendorLogo,
            "A legendary hero and the former Titan Vanguard, Saint-14 now manages the PvP gamemode Trials of Osiris.\n\n" +
            "These rewards change perks based on weekly reset.");

        var i = 0;
        var lg = Context.Guild.Language();

        foreach (var repReward in repRewards)
        {
            var reward = vendorData.Sales.Data[repReward];

            if (reward.Quantity != 1)
                continue;

            if(reward.ItemHash is /* powerful engram */ 262604612 or /* reset rank */ 2133694745)
                continue;

            var plugHash1 = vendorData.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(3).PlugHash;
            var plugHash2 = vendorData.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(4).PlugHash;

            var manifestItems =
                BungieAPI.GetManifestDefinition<DestinyInventoryItemDefinition>(lg,
                    new[] {reward.ItemHash, (uint) plugHash1, (uint) plugHash2});

            if (manifestItems[0].ItemType != DestinyItemType.Weapon)
                continue;

            var fullMessage =
                $"[{manifestItems[0].DisplayProperties.Name}]({WeaponHelper.BuildLightGGLink(manifestItems[0].Hash)})\n";

            fullMessage += EmoteHelper.GetEmote(manifestItems[1].DisplayProperties.Icon,
                manifestItems[1].DisplayProperties.Name, plugHash1);
            fullMessage += EmoteHelper.GetEmote(manifestItems[2].DisplayProperties.Icon,
                manifestItems[2].DisplayProperties.Name, plugHash2);

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

        var lg = Context.Guild.Language();

        if (!File.Exists($"Data/modCache-{lg}.json"))
            await FollowupAsync("Populating vendor data, this might take some time...");

        var modCache = ProcessModData.FetchInventory(lg, oauth, destinyMembership);

        await FollowupAsync(embed: modCache.BuildEmbed(oauth, destinyMembership, true));
    }
}