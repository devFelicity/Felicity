using System.Text;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Service.Abstractions;
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

    [SlashCommand("gunsmith", "Fetch Banshee weapon inventory which includes D2Gunsmith and LightGG links.")]
    public async Task Gunsmith()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (user == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        var lg = MiscUtils.GetLanguage(Context.Guild, _serverDb);

        // if (!File.Exists($"Data/gsCache-{lg}.json"))
        //     await FollowupAsync("Populating vendor data, this might take some time...");

        var gsCache = await ProcessGunsmithData.FetchInventory(lg, user, _bungieClient);

        if (gsCache != null)
            await FollowupAsync(embed: ProcessGunsmithData.BuildEmbed(gsCache, Context.Client));
        else
            await FollowupAsync("An error occurred trying to build inventory.");
    }

    [SlashCommand("xur", "Fetch Xûr inventory which includes D2Gunsmith and LightGG links.")]
    public async Task Xur()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        if (ProcessXurData.IsXurHere())
        {
            var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
            if (user == null)
            {
                await FollowupAsync("Failed to fetch user profile.");
                return;
            }

            var lg = MiscUtils.GetLanguage(Context.Guild, _serverDb);

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

    [SlashCommand("ada-1", "Get list of shaders currently available at Ada-1.")]
    public async Task ShAda1()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

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
            DefinitionHashes.Vendors.Ada1_350061650, new[]
            {
                DestinyComponentType.VendorCategories, DestinyComponentType.VendorSales
            },
            user.GetTokenData());

        var shaderIndexes = vendorData.Response.Categories.Data.Categories.ElementAt(1).ItemIndexes;

        var responseString = new StringBuilder();

        foreach (var shaderIndex in shaderIndexes)
        {
            var reward = vendorData.Response.Sales.Data[shaderIndex];

            if (reward.Quantity != 1)
                continue;

            if (reward.Item.Hash is DefinitionHashes.InventoryItems.UpgradeModule)
                continue;

            if (reward.Item.Select(x => x.ItemSubType != DestinyItemSubType.Shader))
                continue;

            responseString.Append(reward.SaleStatus == VendorItemStatus.Success ? "❌" : "✅");
            responseString.Append($" [{reward.Item.Select(x => x.DisplayProperties.Name)}]({MiscUtils.GetLightGgLink(reward.Item.Select(x => x.Hash))})\n");
        }

        var embed = Embeds.MakeBuilder();

        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Ada-1, Armor Synthesis",
            IconUrl = BotVariables.Images.AdaVendorLogo
        };

        embed.Description = "Ada-1 is selling shaders that have been unavailable for quite some time,\n\n" +
                            "These cost 10,000 Glimmer each, but keep in mind that she'll only be offering 3 shaders per week during Season 20.";

        embed.AddField("Shaders", responseString.ToString());

        await FollowupAsync(embed: embed.Build());
    }

    /*[SlashCommand("mods", "Get list of mods currently available at vendors.")]
    public async Task Mods()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (user == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        var lg = MiscUtils.GetLanguage(Context.Guild, _serverDb);

        // if (!File.Exists($"Data/modCache-{lg}.json"))
        //     await FollowupAsync("Populating vendor data, this might take some time...");

        var modCache = await ProcessModData.FetchInventory(_bungieClient, lg, user);

        await FollowupAsync(embed: await ProcessModData.BuildEmbed(_bungieClient, modCache, user));
    }*/

    [SlashCommand("saint14", "Fetch Saint-14 (Trials of Osiris) reputation rewards for the week.")]
    public async Task Saint14()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

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
            errorEmbed.Title = "Saint-14";
            errorEmbed.ThumbnailUrl = BotVariables.Images.SaintVendorLogo;

            errorEmbed.Description =
                $"Because you've reset your rank {Format.Bold(vendorData.Response.Vendor.Data.Progression.CurrentResetCount.ToString())} times, " +
                "Saint-14 no longer sells weapons with fixed rolls for you.";

            await FollowupAsync(embed: errorEmbed.Build(), ephemeral: true);
            return;
        }

        var repRewards = vendorData.Response.Categories.Data.Categories.ElementAt(1).ItemIndexes;

        var embed = Embeds.MakeBuilder();
        embed.Color = Color.Purple;

        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Saint-14",
            IconUrl = BotVariables.Images.SaintVendorLogo
        };

        embed.Description =
            "A legendary hero and the former Titan Vanguard, Saint-14 now manages the PvP game mode Trials of Osiris.\n\n"
            + "These rewards change perks on weekly reset.";

        var i = 0;

        foreach (var repReward in repRewards)
        {
            var reward = vendorData.Response.Sales.Data[repReward];

            if (reward.Quantity != 1)
                continue;

            if (reward.Item.Hash is DefinitionHashes.InventoryItems.PowerfulTrialsGear
                or DefinitionHashes.InventoryItems.ResetRank_1514009869
                or DefinitionHashes.InventoryItems.ResetRank_2133694745)
                continue;

            if (reward.Item.Select(x => x.ItemType != DestinyItemType.Weapon))
                continue;

            var fullMessage =
                $"{EmoteHelper.GetItemType(reward.Item.Select(x => x.ItemSubType))} " +
                $"[{reward.Item.Select(x => x.DisplayProperties.Name)}]" +
                $"({MiscUtils.GetLightGgLink(reward.Item.Select(x => x.Hash))})\n";

            for (var j = 0; j < 4; j++)
            {
                var plug = vendorData.Response.ItemComponents.Sockets.Data[repReward].Sockets.ElementAt(j + 1).Plug;

                fullMessage += EmoteHelper.GetEmote(Context.Client,
                    plug.Select(x => x.DisplayProperties.Icon.RelativePath),
                    plug.Select(x => x.DisplayProperties.Name), plug.Select(x => x.Hash));
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
            Value = vendorData.Response.Vendor.Data.Progression.Level
        });

        await FollowupAsync(embed: embed.Build());
    }
}