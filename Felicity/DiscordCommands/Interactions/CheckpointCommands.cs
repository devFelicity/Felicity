using System.Text;
using Discord;
using Discord.Interactions;
using Felicity.Models;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

public class CheckpointCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("checkpoint-list", "List all available checkpoints and their status.")]
    public async Task CheckpointList()
    {
        await DeferAsync();

        var checkpointList = await CheckpointParser.Fetch();

        var embed = Embeds.MakeBuilder();

        embed.Author = new EmbedAuthorBuilder
        {
            Name = "D2Checkpoint.com",
            IconUrl = "https://cdn.tryfelicity.one/images/d2cp.png",
            Url = "https://d2checkpoint.com"
        };

        embed.ThumbnailUrl =
            "https://www.bungie.net/common/destiny2_content/icons/8b1bfd1c1ce1cab51d23c78235a6e067.png";

        embed.Title = "All available checkpoints:";

        var sb = new StringBuilder();

        if (checkpointList != null)
            foreach (var officialCp in checkpointList.Official)
                sb.Append(
                    $"{Format.Bold(officialCp.Activity)} - {officialCp.Encounter} [{officialCp.Players}/{officialCp.MaxPlayers}]" +
                    $"\n{Format.Code($"/join {officialCp.Name}")}\n\n");
        else
            sb.Append("Checkpoint list unavailable.");

        embed.Description = sb.ToString();

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("checkpoint", "Find a checkpoint and join it.")]
    public async Task Checkpoint([Autocomplete(typeof(CheckpointAutocomplete))] [Summary("name", "Activity/Encounter to search for:")] int displayOrder)
    {
        await DeferAsync();

        var checkpointList = await CheckpointParser.Fetch();

        var failed = checkpointList == null;

        var currentCheckpoint = checkpointList?.Official.FirstOrDefault(x => x.DisplayOrder == displayOrder);
        if (currentCheckpoint == null) failed = true;

        if (failed)
        {
            var errEmbed = Embeds.MakeErrorEmbed();
            errEmbed.Description = "Failed to fetch checkpoint list.";

            await FollowupAsync(embed: errEmbed.Build());
            return;
        }

        var embed = Embeds.MakeBuilder();

        embed.Author = new EmbedAuthorBuilder
        {
            Name = "D2Checkpoint.com",
            IconUrl = "https://cdn.tryfelicity.one/images/d2cp.png",
            Url = "https://d2checkpoint.com"
        };

#pragma warning disable CS8602
        embed.ThumbnailUrl = currentCheckpoint.IconUrl;
        embed.ImageUrl = currentCheckpoint.ImgUrl;
        embed.Title = $"{currentCheckpoint.Activity} - {currentCheckpoint.Encounter}";
        embed.Description = Format.Code($"/join {currentCheckpoint.Name}");
        embed.AddField("Players", $"{currentCheckpoint.Players}/{currentCheckpoint.MaxPlayers}", true);
        embed.AddField("Difficulty", currentCheckpoint.DifficultyTier.ToString(), true);
#pragma warning restore CS8602

        await FollowupAsync(embed: embed.Build());
    }
}