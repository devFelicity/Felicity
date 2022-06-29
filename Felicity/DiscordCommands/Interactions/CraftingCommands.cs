using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.HashReferences;
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
                        field.Value += $"⚠️ {obj.Progress + inventoryItemCount}/{obj.CompletionValue}";
                    else
                        field.Value += $"{obj.Progress}/{obj.CompletionValue}";
                }
            }

            if (string.IsNullOrEmpty((string?)field.Value)) continue;

            embed.AddField(field);

            if (embed.Fields.Count is 2 or 5)
                embed.AddField("\u200b", '\u200b');
        }

        await FollowupAsync(embed: embed.Build());
    }

    private static int GetItemCount(BungieResponse<DestinyProfileResponse> request, uint recordDefinitionHash)
    {
        var allItems = request.Response.ProfileInventory.Data.Items.ToList();
        allItems.AddRange(request.Response.CharacterInventories.Data.Values.SelectMany(d => d.Items));
        allItems.AddRange(request.Response.CharacterEquipment.Data.Values.SelectMany(d => d.Items));

        var counter = allItems
            .Where(destinyItemComponent => destinyItemComponent.GetHashCode() == GetWeaponId(recordDefinitionHash))
            .Count(destinyItemComponent => destinyItemComponent.State.HasFlag(ItemState.HighlightedObjective));

        return counter;
    }

    private static uint GetWeaponId(uint recordDefinitionHash)
    {
        return recordDefinitionHash switch
        {
            DefinitionHashes.Records.BumpintheNight => DefinitionHashes.InventoryItems.BumpintheNight_1959650777,
            DefinitionHashes.Records.Firefright => DefinitionHashes.InventoryItems.Firefright_2778013407,
            DefinitionHashes.Records.HollowDenial => DefinitionHashes.InventoryItems.HollowDenial_2323544076,
            DefinitionHashes.Records.NezarecsWhisper => DefinitionHashes.InventoryItems.NezarecsWhisper_254636484,
            DefinitionHashes.Records.TearsofContrition => DefinitionHashes.InventoryItems.TearsofContrition_1366394399,
            DefinitionHashes.Records.WithoutRemorse => DefinitionHashes.InventoryItems.WithoutRemorse_1478986057,
            DefinitionHashes.Records.Austringer => DefinitionHashes.InventoryItems.Austringer_3055790362,
            DefinitionHashes.Records.Beloved => DefinitionHashes.InventoryItems.Beloved_3107853529,
            DefinitionHashes.Records.CALUSMiniTool => DefinitionHashes.InventoryItems.CALUSMiniTool_2490988246,
            DefinitionHashes.Records.DrangBaroque => DefinitionHashes.InventoryItems.DrangBaroque_502356570,
            DefinitionHashes.Records.FixedOdds => DefinitionHashes.InventoryItems.FixedOdds_2194955522,
            DefinitionHashes.Records.TheEpicurean => DefinitionHashes.InventoryItems.TheEpicurean_2263839058,
            DefinitionHashes.Records.ExplosivePersonality => DefinitionHashes.InventoryItems
                .ExplosivePersonality_4096943616,
            DefinitionHashes.Records.PieceofMind => DefinitionHashes.InventoryItems.PieceofMind_2097055732,
            DefinitionHashes.Records.RecurrentImpact => DefinitionHashes.InventoryItems.RecurrentImpact_1572896086,
            DefinitionHashes.Records.SweetSorrow => DefinitionHashes.InventoryItems.SweetSorrow_1248372789,
            DefinitionHashes.Records.Thoughtless => DefinitionHashes.InventoryItems.Thoughtless_4067556514,
            DefinitionHashes.Records.UnderYourSkin => DefinitionHashes.InventoryItems.UnderYourSkin_232928045,
            DefinitionHashes.Records.Cataclysmic => DefinitionHashes.InventoryItems.Cataclysmic_999767358,
            DefinitionHashes.Records.Deliverance => DefinitionHashes.InventoryItems.Deliverance_768621510,
            DefinitionHashes.Records.Forbearance => DefinitionHashes.InventoryItems.Forbearance_613334176,
            DefinitionHashes.Records.Insidious => DefinitionHashes.InventoryItems.Insidious_3428521585,
            DefinitionHashes.Records.LubraesRuin => DefinitionHashes.InventoryItems.LubraesRuin_2534546147,
            DefinitionHashes.Records.Submission => DefinitionHashes.InventoryItems.Submission_3886416794,
            DefinitionHashes.Records.CometoPass => DefinitionHashes.InventoryItems.CometoPass_927567426,
            DefinitionHashes.Records.EdgeofAction => DefinitionHashes.InventoryItems.EdgeofAction_2535142413,
            DefinitionHashes.Records.EdgeofConcurrence => DefinitionHashes.InventoryItems.EdgeofConcurrence_542203595,
            DefinitionHashes.Records.EdgeofIntent => DefinitionHashes.InventoryItems.EdgeofIntent_14194600,
            DefinitionHashes.Records.FathersSins => DefinitionHashes.InventoryItems.FathersSins_3865728990,
            DefinitionHashes.Records.FelTaradiddle => DefinitionHashes.InventoryItems.FelTaradiddle_1399109800,
            DefinitionHashes.Records.Tarnation => DefinitionHashes.InventoryItems.Tarnation_2721157927,
            DefinitionHashes.Records.EmpiricalEvidence => DefinitionHashes.InventoryItems.EmpiricalEvidence_2607304614,
            DefinitionHashes.Records.ForensicNightmare => DefinitionHashes.InventoryItems.ForensicNightmare_1526296434,
            DefinitionHashes.Records.LikelySuspect => DefinitionHashes.InventoryItems.LikelySuspect_1994645182,
            DefinitionHashes.Records.OsteoStriga => DefinitionHashes.InventoryItems.OsteoStriga_46524085,
            DefinitionHashes.Records.PalmyraB => DefinitionHashes.InventoryItems.PalmyraB_3489657138,
            DefinitionHashes.Records.PointedInquiry => DefinitionHashes.InventoryItems.PointedInquiry_297296830,
            DefinitionHashes.Records.RagnhildD => DefinitionHashes.InventoryItems.RagnhildD_4225322581,
            DefinitionHashes.Records.RedHerring => DefinitionHashes.InventoryItems.RedHerring_3175851496,
            DefinitionHashes.Records.Syncopation53 => DefinitionHashes.InventoryItems.Syncopation53_2856514843,
            DefinitionHashes.Records.TheEnigma => DefinitionHashes.InventoryItems.TheEnigma_2595497736,
            _ => 0
        };
    }
}