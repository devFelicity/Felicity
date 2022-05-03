using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BungieSharper.Entities;
using BungieSharper.Entities.GroupsV2;
using BungieSharper.Entities.User;
using Discord;
using Discord.Commands;
using Felicity.Helpers;
using Felicity.Services;
using Felicity.Structs;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.TestCommands;

public class D2TestCommands : ModuleBase<SocketCommandContext>
{
    [RequireOwner]
    [Command("addEmote")]
    public async Task AddEmote(string name, string url)
    {
        if (Context.Guild.Emotes.Any(guildEmote => guildEmote.Name == name))
        {
            await ReplyAsync("Emote with that name already exists.");
            return;
        }

        if (!ConfigHelper.GetEmoteSettings().ServerIDs.Contains(Context.Guild.Id))
        {
            await ReplyAsync("This server is not a designated emote bank.");
            return;
        }

        var imageBytes = new HttpClient().GetByteArrayAsync(url).Result;
        var newEmote = Context.Guild.CreateEmoteAsync(name, new Image(new MemoryStream(imageBytes))).Result;

        await ReplyAsync($"New emote created: {newEmote}");
    }

    [Command("getClan")]
    public async Task GetClan(string bungieTag)
    {
        var name = bungieTag.Split("#").First();
        var code = Convert.ToInt16(bungieTag.Split("#").Last());

        var userInfoCard = APIService.GetApiClient().Api.Destiny2_SearchDestinyPlayerByBungieName(BungieMembershipType.All,
            new ExactSearchRequest
            {
                DisplayName = name,
                DisplayNameCode = code
            }).Result.First();

        var clans = APIService.GetApiClient().Api.GroupV2_GetGroupsForMember(GroupsForMemberFilter.All, GroupType.Clan,
            userInfoCard.MembershipId, userInfoCard.MembershipType).Result.Results;

        var groupMemberships = clans.ToList();
        if (groupMemberships.Any())
        {
            var clan = groupMemberships.First();
            if (clan != null)
                    await ReplyAsync(
                        $"{clan.Group.Name} [{clan.Group.ClanInfo.ClanCallsign}] ({clan.Group.GroupId})\n" +
                        $"About: {clan.Group.About}\n" +
                        $"Motto: {clan.Group.Motto}\n" +
                        $"Created: {clan.Group.CreationDate:F}\n" +
                        $"Members: {clan.Group.MemberCount}\n" +
                        $"D2 Level: {clan.Group.ClanInfo.D2ClanProgressions[584850370].Level}\n\n" +
                        $"{bungieTag} joined {clan.Member.JoinDate:F}");
            else
                await ReplyAsync($"Failed to fetch clan info for {bungieTag}");
        }
    }
}