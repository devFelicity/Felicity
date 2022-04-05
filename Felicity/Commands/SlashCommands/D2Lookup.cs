using System;
using System.Linq;
using System.Threading.Tasks;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.User;
using Discord.Interactions;
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.SlashCommands;

[Group("lookup", "Various lookup commands for Destiny2")]
public class D2Lookup : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("guardian", "Look up a profile of a player")]
    public async Task Guardian(
        [Summary("bungiename", "Bungie name of the requested user (name#1234)")] string bungieTag)
    {
        await DeferAsync();

        var name = bungieTag.Split("#").First();
        var code = Convert.ToInt16(bungieTag.Split("#").Last());

        var userInfoCard = APIService.GetApiClient().Api.Destiny2_SearchDestinyPlayerByBungieName(BungieMembershipType.All,
            new ExactSearchRequest
            {
                DisplayName = name,
                DisplayNameCode = code
            }).Result.First();

        var player = APIService.GetApiClient().Api.Destiny2_GetProfile(userInfoCard.MembershipId,
            userInfoCard.MembershipType, new[]
            {
                DestinyComponentType.Characters
            }).Result;

        await FollowupAsync("", new[] {player.GenerateLookupEmbed(userInfoCard)});
    }
}