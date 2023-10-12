using System.Text;
using System.Text.Json;
using Discord;
using Discord.Commands;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Text;

[Preconditions.RequireBotModerator]
public class EmblemTextCommands : ModuleBase<ShardedCommandContext>
{
    private readonly IBungieClient _client;

    public EmblemTextCommands(IBungieClient client)
    {
        _client = client;
    }

    [Command("storeEmblems")]
    public async Task StoreEmblems()
    {
        var knownEmblems = FetchAllEmblems();

        await File.WriteAllTextAsync($"Data/emblems/{DateTime.UtcNow:yy-MM-dd}.json",
            JsonSerializer.Serialize(knownEmblems));

        await ReplyAsync($"Saved {knownEmblems.Count} emblems.");
    }

    [Command("compareEmblems")]
    public async Task CompareEmblems(string originDate)
    {
        var previousEmblems =
            JsonSerializer.Deserialize<List<uint>>(await File.ReadAllTextAsync($"Data/emblems/{originDate}.json"));
        var newEmblems = FetchAllEmblems();

        if (previousEmblems != null)
        {
            var sb = new StringBuilder();

            var uniqueEmblems = newEmblems.Except(previousEmblems).ToList();
            foreach (var uniqueEmblem in uniqueEmblems)
                if (_client.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(uniqueEmblem,
                        out var emblemDefinition))
                    sb.Append(
                        $"{emblemDefinition.DisplayProperties.Name}: {emblemDefinition.SecondaryIcon.AbsolutePath}\n");

            var bytes = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            await Context.Channel.SendFileAsync(new FileAttachment(bytes, "emblemComparison.txt"));
        }
    }

    private List<uint> FetchAllEmblems()
    {
        var knownEmblems = new List<uint>();
        foreach (var itemDefinition in _client.Repository.GetAll<DestinyInventoryItemDefinition>())
        {
            if (itemDefinition.ItemType != DestinyItemType.Emblem)
                continue;

            if (itemDefinition.Redacted)
                continue;

            if (!knownEmblems.Contains(itemDefinition.Hash))
                knownEmblems.Add(itemDefinition.Hash);
        }

        return knownEmblems;
    }
}