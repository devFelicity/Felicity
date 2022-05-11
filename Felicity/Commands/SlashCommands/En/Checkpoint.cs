using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Felicity.Structs;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.SlashCommands.En;

[Group("checkpoint", "Get join codes for various in-game checkpoints.")]
public class Checkpoint : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("support", "Get an invite to the checkpoint server.")]
    public async Task En_CheckpointSupport()
    {
        await RespondAsync(embed: ProcessCPData.BuildServerEmbed());
    }

    [SlashCommand("search", "Search available checkpoints.")]
    public async Task En_CheckpointSearch(
        [Autocomplete(typeof(CheckpointAutocomplete))]
        [Summary("activity", "Activity you want to get a checkpoint for.")]
        string activity
    )
    {
        await DeferAsync();

        if (activity == "Other")
        {
            await FollowupAsync(embed: ProcessCPData.BuildSavedEmbed());
            return;
        }

        await FollowupAsync(embed: ProcessCPData.BuildCPEmbed(activity));
    }
}

public class CheckpointAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        await Task.Delay(0);

        var cpCache = ProcessCPData.FromJson();

        var result = cpCache.CpInventory.Checkpoints
            .Select(activeCp => new AutocompleteResult(activeCp.Name, activeCp.Name)).ToList();

        result = result.OrderBy(x => x.Name).ToList();

        result.Add(new AutocompleteResult("--- Others", "Other"));

        return AutocompletionResult.FromSuccess(result);
    }
}