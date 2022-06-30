using Discord.Interactions;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.Models;
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
    private readonly ServerDb _serverDb;
    private readonly UserDb _userDb;
    private readonly IBungieClient _bungieClient;

    public VendorCommands(ServerDb serverDb, UserDb userDb, IBungieClient bungieClient)
    {
        _serverDb = serverDb;
        _userDb = userDb;
        _bungieClient = bungieClient;
    }

    [SlashCommand("xur", "Fetch Xûr inventory which includes D2Gunsmith and LightGG links.")]
    public async Task Xur()
    {
        await DeferAsync();

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
        await DeferAsync();

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
}