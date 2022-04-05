using System;
using System.Linq;
using System.Threading.Tasks;
using BungieSharper.Entities;
using BungieSharper.Entities.GroupsV2;
using BungieSharper.Entities.User;
using Discord.Commands;
using Felicity.Services;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.TestCommands;

public class D2TestCommands : ModuleBase<SocketCommandContext>
{
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