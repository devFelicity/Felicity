using Discord;
using Discord.Interactions;
using FelicityOne.Caches;
using FelicityOne.Helpers;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands.SlashCommands;

[Group("checkpoint", "Get join codes for various in-game checkpoints.")]
public class Checkpoint : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("support", "Get an invite to the checkpoint server.")]
    public async Task CheckpointSupport()
    {
        await RespondAsync(embed: ProcessCPData.BuildServerEmbed());
    }

    [SlashCommand("search", "Search available checkpoints.")]
    public async Task CheckpointSearch(
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

        var cpCache = ConfigHelper.FromJson<CheckpointCache>(await File.ReadAllTextAsync("Data/cpCache.json"));

        var result = cpCache?.CpInventory.Checkpoints
            .Select(activeCp => new AutocompleteResult(activeCp.Name, activeCp.Name)).ToList();

        if (result == null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful,
                "Failed to fetch checkpoints from cache.");
        result = result.OrderBy(x => x.Name).ToList();

        result.Add(new AutocompleteResult("--- Others", "Other"));

        return AutocompletionResult.FromSuccess(result);

    }
}