using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Util;
using Felicity.Util.Enums;
using ActivityType = Felicity.Util.Enums.ActivityType;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

public class LootCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;

    public LootCommands(IBungieClient bungieClient)
    {
        _bungieClient = bungieClient;
    }

    [SlashCommand("loot-table", "Get loot tables from dungeons or raids.")]
    public async Task LootTable([Autocomplete(typeof(LootTableAutocomplete))] string lootTable)
    {
        await DeferAsync();

        var requestedLootTable = LootTables.KnownTables.FirstOrDefault(x => x.Name == lootTable);
        if (requestedLootTable?.Loot == null)
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = "Unable to find requested loot table.";

            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var embed = Embeds.MakeBuilder();
        embed.Title = $"{requestedLootTable.Name} loot table:";
        embed.Description = Format.Italics(requestedLootTable.Description);

        if (requestedLootTable.ActivityType == ActivityType.Dungeon)
            embed.Description +=
                $"\n\n{Format.Bold("Secret chests can drop any previously acquired armor and weapons.")}";

        embed.ThumbnailUrl = requestedLootTable.ActivityType switch
        {
            ActivityType.Dungeon => BotVariables.Images.DungeonIcon,
            ActivityType.Raid => BotVariables.Images.RaidIcon,
            _ => string.Empty
        };

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var table in requestedLootTable.Loot)
        {
            if (table.LootIds == null)
                continue;

            if (embed.Fields.Count is 2 or 5)
                embed.AddField("\u200b", '\u200b');

            embed.AddField(table.EncounterName, BuildDrops(_bungieClient, table.LootIds), true);
        }

        await FollowupAsync(embed: embed.Build());
    }

    private static string BuildDrops(IBungieClient bungieClient, List<uint> tableLootIds)
    {
        var result = string.Empty;

        foreach (var tableLootId in tableLootIds)
        {
            switch (tableLootId)
            {
                case (uint)Armor.Everything:
                    result += "\n <:consumables:996724235634491523> All Possible Drops";
                    continue;
                case (uint)Armor.Helmet:
                    result += "<:helmet:996490149728899122> ";
                    continue;
                case (uint)Armor.Gloves:
                    result += "<:gloves:996490148025995385> ";
                    continue;
                case (uint)Armor.Chest:
                    result += "<:chest:996490146922901655> ";
                    continue;
                case (uint)Armor.Boots:
                    result += "<:boots:996490145224200292> ";
                    continue;
                case (uint)Armor.Class:
                    result += "<:class:996490144066572288> ";
                    continue;
            }

            if (bungieClient.Repository.TryGetDestinyDefinition<DestinyInventoryItemDefinition>(tableLootId,
                    BungieLocales.EN, out var manifestItem))
                result +=
                    $"\n{EmoteHelper.GetWeaponType(manifestItem)} " +
                    $"[{manifestItem.DisplayProperties.Name.Replace("(Timelost)", "(TL)")}]" +
                    $"({MiscUtils.GetLightGgLink(tableLootId)})";
        }

        return result;
    }
}