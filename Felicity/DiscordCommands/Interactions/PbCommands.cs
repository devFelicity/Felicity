using System.Text;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireOAuth]
[Group("pb", "Gets your personal best times for each category.")]
public class PbCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly UserDb _userDb;

    public PbCommands(IBungieClient bungieClient, UserDb userDb)
    {
        _bungieClient = bungieClient;
        _userDb = userDb;
    }

    [SlashCommand("raids", "Gets your fastest raids.")]
    public async Task PbRaids()
    {
        var currentUser = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (currentUser == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        var profileMetrics = await _bungieClient.ApiAccess.Destiny2.GetProfile(currentUser.DestinyMembershipType,
            currentUser.DestinyMembershipId, new[]
            {
                DestinyComponentType.Metrics
            }, currentUser.GetTokenData());

        var value = new StringBuilder();
        var metricList = new List<KeyValuePair<string, uint>>
        {
            new("Last Wish", DefinitionHashes.Metrics.LastWishTimeTrial_552340969),
            new("Garden of Salvation", DefinitionHashes.Metrics.KingsFallTimeTrial_399420098),
            new("Deep Stone Crypt", DefinitionHashes.Metrics.DeepStoneCryptTimeTrial_3679202587),
            new("Vault of Glass", DefinitionHashes.Metrics.VaultofGlassTimeTrial_905219689),
            new("Vow of the Disciple", DefinitionHashes.Metrics.VowoftheDiscipleTimeTrial_3775579868),
            new("King's Fall", DefinitionHashes.Metrics.KingsFallTimeTrial_399420098),
            new("Root of Nightmares", DefinitionHashes.Metrics.RootofNightmaresTimeTrial_58319253)
        };

        foreach (var metric in metricList)
        {
            var time = profileMetrics.Response.Metrics.Data.Metrics[metric.Value].ObjectiveProgress.Progress
                ?.FormatUIDisplayValue(profileMetrics.Response.Metrics.Data.Metrics[metric.Value].ObjectiveProgress
                    .Objective.GetValueOrNull()!);
            value.Append($"> `{time}` - **{metric.Key}**\n");
        }

        var embed = Embeds.MakeBuilder();
        embed.Title = $"Personal best clear times for {currentUser.BungieName}";
        embed.Description =
            "⚠️ These values are only what in-game stat trackers show, real times as well as checkpoint vs full clears and lowmans will be available in a future update.\n\n"
            + value;

        await FollowupAsync(embed: embed.Build());
    }
}