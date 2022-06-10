using BungieSharper.Entities.Destiny;
using BungieSharper.Entities.Destiny.Definitions.Records;
using Discord;
using Discord.Interactions;
using FelicityOne.Enums;
using FelicityOne.Events;
using FelicityOne.Helpers;
using FelicityOne.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands.SlashCommands;

[RequireOAuth]
public class CraftingCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("recipes", "View current progression towards weapon recipes.")]
    public async Task Recipes()
    {
        await DeferAsync();

        var oauth = Context.User.OAuth();

        var request = BungieAPI.GetApiClient().Api.Destiny2_GetProfile(oauth.DestinyMembership.MembershipId,
            oauth.DestinyMembership.MembershipType, new[]
            {
                DestinyComponentType.Records
            }, oauth.AccessToken).Result;

        var embed = new EmbedBuilder
        {
            Title = "Craftable List",
            Color = ConfigService.GetEmbedColor(),
            Description = "List of craftable weapons and your progress on them.",
            ThumbnailUrl = "https://www.bungie.net/common/destiny2_content/icons/e7e6d522d375dfa6dec055135ce6a77e.png",
            Footer = Extensions.GenerateEmbedFooter()
        };

        var manifestList = new List<uint>();

        foreach (var keyPair in Craftables.craftableList)
            manifestList.AddRange(keyPair.Value);

        var manifestEntries =
            BungieAPI.GetManifestDefinition<DestinyRecordDefinition>(Context.Guild.Language(), manifestList);

        foreach (var keyPair in Craftables.craftableList)
        {
            var field = new EmbedFieldBuilder
            {
                Name = keyPair.Key,
                IsInline = true
            };

            foreach (var weaponId in keyPair.Value)
            {
                var recordDefinition = manifestEntries.Find(definition => definition.Hash == weaponId);

                field.Value += $"\n{recordDefinition.DisplayProperties.Name}: ";

                var record = request.ProfileRecords.Data.Records[weaponId];

                var obj = record.Objectives.First();
                field.Value += obj.Complete ? "✅" : $"{obj.Progress}/{obj.CompletionValue}";
            }

            embed.AddField(field);

            if (embed.Fields.Count is 2 or 5)
                embed.AddField("\u200b", '\u200b');
        }

        await FollowupAsync(embed: embed.Build());
    }
}