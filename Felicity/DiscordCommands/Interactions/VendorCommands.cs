using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using Felicity.Models;
using Felicity.Models.Caches;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireOAuth]
[Group("vendor", "Group of commands related to vendors and their available items.")]
public class VendorCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly ServerDb _serverDb;
    private readonly UserDb _userDb;

    public VendorCommands(ServerDb serverDb, UserDb userDb, IBungieClient bungieClient)
    {
        _serverDb = serverDb;
        _userDb = userDb;
        _bungieClient = bungieClient;
    }

    [SlashCommand("xur", "Fetch Xûr inventory which includes D2Gunsmith and LightGG links.")]
    public async Task Xur()
    {
        if (ProcessXurData.IsXurHere())
        {
            var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
            if (user == null)
            {
                await FollowupAsync("Failed to fetch user profile.");
                return;
            }

            var lg = _serverDb.Servers.FirstOrDefault(x => x.ServerId == Context.Guild.Id)?.BungieLocale ??
                     BungieLocales.EN;

            // if (!File.Exists($"Data/xurCache-{lg}.json"))
            //     await FollowupAsync("Populating vendor data, this might take some time...");

            var xurCache = await ProcessXurData.FetchInventory(lg, user, _bungieClient);

            if (xurCache != null)
                await FollowupAsync(embed: ProcessXurData.BuildEmbed(xurCache, Context.Client));
            else
                await FollowupAsync("An error occurred trying to build inventory.");
        }
        else
        {
            ProcessXurData.ClearCache();

            await FollowupAsync(embed: ProcessXurData.BuildUnavailableEmbed());
        }
    }

    [SlashCommand("mods", "Get list of mods currently available at vendors.")]
    public async Task Mods()
    {
        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (user == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        var server = _serverDb.Servers.FirstOrDefault(x => x.ServerId == Context.Guild.Id);
        var lg = server?.BungieLocale ?? BungieLocales.EN;

        // if (!File.Exists($"Data/modCache-{lg}.json"))
        //     await FollowupAsync("Populating vendor data, this might take some time...");

        var modCache = await ProcessModData.FetchInventory(_bungieClient, lg, user);

        await FollowupAsync(embed: await ProcessModData.BuildEmbed(_bungieClient, modCache, user));
    }

    [SlashCommand("saint14", "Fetch Saint-14 (Trials of Osiris) reputation rewards for the week.")]
    public async Task Saint14()
    {
        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (user == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        var characterIdTask = await _bungieClient.ApiAccess.Destiny2.GetProfile(user.DestinyMembershipType,
            user.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            });

        var vendorData = await _bungieClient.ApiAccess.Destiny2.GetVendor(user.DestinyMembershipType,
            user.DestinyMembershipId, characterIdTask.Response.Characters.Data.Keys.First(),
            DefinitionHashes.Vendors.Saint14, new[]
            {
                DestinyComponentType.ItemPerks, DestinyComponentType.ItemSockets,
                DestinyComponentType.Vendors, DestinyComponentType.VendorCategories,
                DestinyComponentType.VendorSales
            },
            user.GetTokenData());

        if (vendorData.Response.Vendor.Data.Progression.CurrentResetCount >= 3)
        {
            var errorEmbed = Embeds.MakeBuilder();
            errorEmbed.Author = new EmbedAuthorBuilder
            {
                Name = "Saint-14",
                IconUrl = BotVariables.Images.SaintVendorLogo
            };

            errorEmbed.Description =
                $"Because you've reset your rank {Format.Bold(vendorData.Response.Vendor.Data.Progression.CurrentResetCount.ToString())} times, " +
                "Saint-14 no longer sells weapons with fixed rolls for you.";

            await FollowupAsync(embed: errorEmbed.Build(), ephemeral: true);
            return;
        }

        var repRewards = vendorData.Response.Categories.Data.Categories.ElementAt(1).ItemIndexes;

        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Saint-14",
            IconUrl = BotVariables.Images.SaintVendorLogo
        };
        embed.Description =
            "A legendary hero and the former Titan Vanguard, Saint-14 now manages the PvP game mode Trials of Osiris.\n\n"
            + "These rewards change perks based on weekly reset.";

        var i = 0;
        var server = _serverDb.Servers.FirstOrDefault(x => x.ServerId == Context.Guild.Id);

        var lg = server?.BungieLocale ?? BungieLocales.EN;

        foreach (var repReward in repRewards)
        {
            var reward = vendorData.Response.Sales.Data[repReward];

            if (reward.Quantity != 1)
                continue;

            if (reward.Item.Hash is DefinitionHashes.InventoryItems.PowerfulTrialsGear
                or DefinitionHashes.InventoryItems.ResetRank_1514009869)
                continue;

            var plugHash1 = vendorData.Response.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(3).Plug.Hash;
            var plugHash2 = vendorData.Response.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(4).Plug.Hash;

            _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>((uint)reward.Item.Hash!,
                lg, out var result1);
            _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>((uint)plugHash1!, lg,
                out var result2);
            _bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>((uint)plugHash2!, lg,
                out var result3);

            var manifestItems = new[]
            {
                result1,
                result2,
                result3
            };

            if (manifestItems[0].ItemType != DestinyItemType.Weapon)
                continue;

            var fullMessage =
                $"[{manifestItems[0].DisplayProperties.Name}]({WeaponHelper.BuildLightGGLink(manifestItems[0].Hash)})\n";

            fullMessage += EmoteHelper.GetEmote(Context.Client, manifestItems[1].DisplayProperties.Icon.RelativePath,
                manifestItems[1].DisplayProperties.Name, plugHash1);
            fullMessage += EmoteHelper.GetEmote(Context.Client, manifestItems[2].DisplayProperties.Icon.RelativePath,
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
            Value = vendorData.Response.Vendor.Data.Progression.Level
        });

        await FollowupAsync(embed: embed.Build());
    }
}