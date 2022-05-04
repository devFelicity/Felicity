using System.Collections.Generic;
using System.IO;
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
using Felicity.Structs;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Commands.SlashCommands;

[Group("vendor", "Group of commands related to vendors and their available items.")]
public class VendorCommands : InteractionModuleBase<SocketInteractionContext>
{
    [RequireOAuth]
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
            if(File.Exists("Data/xurCache.json"))
                File.Delete("Data/xurCache.json");

            await FollowupAsync(embed: ProcessXurData.BuildUnavailableEmbed());
        }
    }

    [RequireOAuth]
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