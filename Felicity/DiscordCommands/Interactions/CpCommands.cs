using Discord;
using Discord.Interactions;
using Felicity.Models.Caches;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

[Group("checkpoint", "Get join codes for various in-game checkpoints.")]
public class Checkpoint : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("support", "Get an invite to the checkpoint server.")]
    public async Task CheckpointSupport()
    {
        await RespondAsync(embed: ProcessCpData.BuildServerEmbed());
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
            await FollowupAsync(embed: ProcessCpData.BuildSavedEmbed());
            return;
        }

        await FollowupAsync(embed: ProcessCpData.BuildCpEmbed(activity));
    }
}

