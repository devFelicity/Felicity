using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.Records;
using DotNetBungieAPI.Models.Destiny.Responses;
using Felicity.Models;
using Felicity.Util;
using Felicity.Util.Enums;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireOAuth]
public class CraftingCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly UserDb _userDb;
    private readonly ServerDb _serverDb;

    public CraftingCommands(UserDb userDb, IBungieClient bungieClient, ServerDb serverDb)
    {
        _userDb = userDb;
        _bungieClient = bungieClient;
        _serverDb = serverDb;
    }

    [SlashCommand("recipes", "View current progression towards weapon recipes.")]
    public async Task Recipes(
        [Summary("hidecomplete", "Hide completed recipes? (default: true)")]
        bool hideComplete = true)
    {
        if(!Context.Interaction.HasResponded)
            await DeferAsync();

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        var serverLanguage = MiscUtils.GetServer(_serverDb, Context.Guild.Id).BungieLocale;

        var request = await _bungieClient.ApiAccess.Destiny2.GetProfile(user!.DestinyMembershipType,
            user.DestinyMembershipId,
            new[]
            {
                DestinyComponentType.Records,
                DestinyComponentType.CharacterEquipment,
                DestinyComponentType.CharacterInventories,
                DestinyComponentType.ProfileInventories
            }, user.GetTokenData());

        var embed = Embeds.MakeBuilder();
        embed.Title = "Craftable List";
        embed.Description = "List of craftable weapons and your progress on them.";
        embed.ThumbnailUrl =
            "https://www.bungie.net/common/destiny2_content/icons/e7e6d522d375dfa6dec055135ce6a77e.png";

        var updateDescription = false;

        foreach (var (source, weaponList) in Craftables.CraftableList)
        {
            var field = new EmbedFieldBuilder
            {
                Name = source,
                IsInline = true
            };

            foreach (var weaponId in weaponList)
            {
                var manifestRecord = await _bungieClient.DefinitionProvider.LoadDefinition<DestinyRecordDefinition>(weaponId, serverLanguage);

                var record = request.Response.ProfileRecords.Data.Records[weaponId];
                var obj = record.Objectives.First();

                if (obj.IsComplete && hideComplete)
                    continue;

                field.Value += $"\n{manifestRecord.DisplayProperties.Name}: ";

                if (obj.IsComplete)
                {
                    field.Value += "✅";
                }
                else
                {
                    var inventoryItemCount = GetItemCount(request, manifestRecord.Hash);
                    if (inventoryItemCount > 0)
                    {
                        field.Value += $"⚠️ {obj.Progress + inventoryItemCount}/{obj.CompletionValue}";
                        updateDescription = true;
                    }
                    else
                    {
                        field.Value += $"{obj.Progress}/{obj.CompletionValue}";
                    }
                }
            }

            if (string.IsNullOrEmpty((string?)field.Value)) continue;

            embed.AddField(field);

            if (embed.Fields.Count is 2 or 5)
                embed.AddField("\u200b", '\u200b');
        }

        if (updateDescription)
            embed.Description += "\n\n⚠️ = Includes incomplete deepsight weapons.";

        if (embed.Fields.Count == 0) 
            embed.Description = "You have completed all available patterns.";

        await FollowupAsync(embed: embed.Build());
    }

    private static int GetItemCount(BungieResponse<DestinyProfileResponse> request, uint recordDefinitionHash)
    {
        var allItems = request.Response.ProfileInventory.Data.Items.ToList();
        allItems.AddRange(request.Response.CharacterInventories.Data.Values.SelectMany(d => d.Items));
        allItems.AddRange(request.Response.CharacterEquipment.Data.Values.SelectMany(d => d.Items));

        var counter = allItems
            .Where(destinyItemComponent => destinyItemComponent.GetHashCode() == Craftables.GetWeaponId(recordDefinitionHash))
            .Count(destinyItemComponent => destinyItemComponent.State.HasFlag(ItemState.HighlightedObjective));

        return counter;
    }
}