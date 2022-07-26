using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.Metrics;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireOAuth]
public class MetricsCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly UserDb _userDb;

    public MetricsCommands(IBungieClient bungieClient, UserDb userDb)
    {
        _bungieClient = bungieClient;
        _userDb = userDb;
    }

    [SlashCommand("metrics", "Fetch metrics from your Destiny profile.")]
    public async Task Metrics(
        [Autocomplete(typeof(MetricAutocomplete))] [Summary("query", "Specific metric you want to pull values for.")]
        uint metricId)
    {
        var currentUser = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (currentUser == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        if (!_bungieClient.Repository.TryGetDestinyDefinition<DestinyMetricDefinition>(metricId, BungieLocales.EN,
                out var metricDefinition))
        {
            await FollowupAsync("Failed to fetch metrics.");
            return;
        }

        var profileMetrics = await _bungieClient.ApiAccess.Destiny2.GetProfile(currentUser.DestinyMembershipType,
            currentUser.DestinyMembershipId, new[]
            {
                DestinyComponentType.Metrics
            }, currentUser.GetTokenData());

        var value = profileMetrics.Response.Metrics.Data.Metrics[metricId].ObjectiveProgress.Progress
            ?.FormatUIDisplayValue(profileMetrics.Response.Metrics.Data.Metrics[metricId].ObjectiveProgress.Objective
                .GetValueOrNull());

        var embed = Embeds.MakeBuilder();
        embed.AddField(metricDefinition.DisplayProperties.Name, value);
        embed.AddField("Objective", metricDefinition.TrackingObjective.Select(x => x.ProgressDescription));

        await FollowupAsync(embed: embed.Build());
    }
}