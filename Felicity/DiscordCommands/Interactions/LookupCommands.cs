using System.Text;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Components;
using DotNetBungieAPI.Models.Destiny.Definitions.Collectibles;
using DotNetBungieAPI.Models.Destiny.Definitions.PresentationNodes;
using DotNetBungieAPI.Models.Destiny.Responses;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;
using Felicity.Util.Enums;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

[Group("lookup", "Various lookup commands for Destiny 2.")]
public class LookupCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly InteractiveService _interactiveService;
    private readonly UserDb _userDb;

    public LookupCommands(UserDb userDb, IBungieClient bungieClient, InteractiveService interactive)
    {
        _userDb = userDb;
        _bungieClient = bungieClient;
        _interactiveService = interactive;
    }

//    [Preconditions.RequireOAuth]
    /*[SlashCommand("guardian-ranks", "Look up triumphs and their completion status for Guardian Ranks.")]
    public async Task LookupRanks()
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (user == null)
        {
            await FollowupAsync("Failed to fetch user profile.");
            return;
        }

        if (_bungieClient.Repository.TryGetDestinyDefinition<DestinyPresentationNodeDefinition>(
                DefinitionHashes.PresentationNodes.GuardianRanks,
                BungieLocales.EN, out var node))
        {
            var pageBuilder = node.Children.PresentationNodes.Select(presentationNode => new PageBuilder
            {
                Title = presentationNode.PresentationNode.Select(x => x.DisplayProperties.Name),
                Description = presentationNode.PresentationNode.Select(x => x.DisplayProperties.Description)
            }).Cast<IPageBuilder>().ToList();

            var paginatorBuilder = new StaticPaginatorBuilder()
                .AddUser(Context.User)
                .WithPages(pageBuilder)
                .AddOption(new Emoji("◀"), PaginatorAction.Backward)
                .AddOption(new Emoji("🔢"), PaginatorAction.Jump)
                .AddOption(new Emoji("▶"), PaginatorAction.Forward)
                .WithActionOnCancellation(ActionOnStop.DisableInput)
                .WithActionOnTimeout(ActionOnStop.DisableInput)
                .Build();

            await _interactiveService.SendPaginatorAsync(paginatorBuilder, Context.Interaction,
                TimeSpan.FromMinutes(10));
        }
        else
        {
            await FollowupAsync("Failed to fetch Guardian Rank definitions.");
        }
    }*/

    [SlashCommand("wish", "Look up patterns for wishes in the Last Wish raid.")]
    public async Task LookupWish(
        [Summary("wish", "Which wish do you need?")] [Autocomplete(typeof(WishAutocomplete))]
        int wishNumber)
    {
        await DeferAsync();

        var wish = Wishes.KnownWishes[wishNumber - 1];

        var embed = new EmbedBuilder
        {
            Color = Color.Purple,
            Description = wish.Description,
            Footer = Embeds.MakeFooter(),
            ImageUrl = $"https://cdn.tryfelicity.one/images/wishes/wish-{wishNumber}.png",
            ThumbnailUrl = "https://bungie.net/common/destiny2_content/icons/fc5791eb2406bf5e6b361f3d16596693.png",
            Title = $"Wish {wish.Number}: {wish.Name}"
        };

        await FollowupAsync(embed: embed.Build());
    }

    

    [SlashCommand("guardian", "Look up a profile of a player.")]
    public async Task LookupGuardian(
        [Summary("bungie-name", "Bungie name of the requested user (name#1234). Defaults to your own.")]
        string bungieTag = "")
    {
        await DeferAsync();

        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        long membershipId;
        BungieMembershipType membershipType;
        string bungieName;

        if (string.IsNullOrEmpty(bungieTag))
        {
            var linkedUser = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);

            if (linkedUser == null)
            {
                await FollowupAsync("You aren't registered and didn't provide a bungie name.");
                return;
            }

            var linkedProfile = await BungieApiUtils.GetLatestProfile(_bungieClient, linkedUser.BungieMembershipId,
                BungieMembershipType.BungieNext);

            membershipId = linkedProfile.MembershipId;
            membershipType = linkedProfile.MembershipType;
            bungieName = $"{linkedProfile.BungieGlobalDisplayName}#{linkedProfile.BungieGlobalDisplayNameCode}";
        }
        else
        {
            var name = bungieTag.Split("#").First();
            var code = Convert.ToInt16(bungieTag.Split("#").Last());

            var goodProfile = await BungieApiUtils.GetLatestProfile(_bungieClient, name, code);
            if (goodProfile == null)
            {
                var errorEmbed = Embeds.MakeErrorEmbed();
                errorEmbed.Description = $"No profiles found matching `{bungieTag}`.";
                await FollowupAsync(embed: errorEmbed.Build());
                return;
            }

            membershipId = goodProfile.MembershipId;
            membershipType = goodProfile.MembershipType;
            bungieName = $"{goodProfile.BungieGlobalDisplayName}#{goodProfile.BungieGlobalDisplayNameCode}";
        }

        var player = _bungieClient.ApiAccess.Destiny2.GetProfile(membershipType, membershipId, new[]
        {
            DestinyComponentType.Characters,
            DestinyComponentType.Collectibles,
            DestinyComponentType.Metrics
        });

        await FollowupAsync(embed: await GenerateLookupEmbed(await player, bungieName, membershipId, membershipType,
            _bungieClient));
    }

    private static async Task<Embed> GenerateLookupEmbed(BungieResponse<DestinyProfileResponse> playerResponse,
        string bungieName,
        long membershipId, BungieMembershipType membershipType, IBungieClient bungieClient)
    {
        DestinyCharacterComponent? goodChar = null;

        var lastPlayed = new DateTime();
        foreach (var (_, value) in playerResponse.Response.Characters.Data.Where(destinyCharacterComponent =>
                     destinyCharacterComponent.Value.DateLastPlayed > lastPlayed))
        {
            lastPlayed = value.DateLastPlayed;
            goodChar = value;
        }

        if (goodChar == null)
        {
            var errorEmbed = Embeds.MakeErrorEmbed();
            errorEmbed.Description = "Failed to find players characters.";
            return errorEmbed.Build();
        }

        var memTypeAndId = $"{(int)membershipType}/{membershipId}";

        var embed = new EmbedBuilder
        {
            Color = Color.DarkMagenta,
            Title = bungieName,
            Footer = Embeds.MakeFooter(),
            Description =
                $"{Format.Code($"/invite {bungieName}")} | " +
                $"{Format.Code($"/join {bungieName}")}",
            ThumbnailUrl = BotVariables.BungieBaseUrl + goodChar.EmblemPath,
            Url = $"https://www.bungie.net/7/en/User/Profile/{memTypeAndId}"
        };

        embed.AddField("Current Season Rank",
            $"> {playerResponse.Response.Metrics.Data.Metrics[DefinitionHashes.Metrics.SeasonofDefianceRank].ObjectiveProgress.Progress:n0}",
            true);
        embed.AddField("Raid Completions", $"> {GetRaidCompletions(playerResponse.Response.Metrics.Data):n0}", true);
        embed.AddField("Triumph Score",
            $"> **Active**: {playerResponse.Response.Metrics.Data.Metrics[DefinitionHashes.Metrics.ActiveTriumphScore].ObjectiveProgress.Progress:n0}\n" +
            $"> **Lifetime**: {playerResponse.Response.Metrics.Data.Metrics[DefinitionHashes.Metrics.TotalTriumphScore].ObjectiveProgress.Progress:n0}",
            true);

        embed.AddField("General",
            $"[Braytech](https://bray.tech/{memTypeAndId})\n" +
            $"[D2Timeline](https://mijago.github.io/D2Timeline/#/display/{memTypeAndId})\n" +
            $"[Guardian.Report](https://guardian.report/?view=PVE&guardians={membershipId})\n", true);
        embed.AddField("PvE",
            $"[Dungeon.Report]({GetReportLink(membershipType, membershipId, "dungeon")})\n" +
            $"[Nightfall.Report](https://nightfall.report/guardian/{memTypeAndId})\n" +
            $"[Raid.Report]({GetReportLink(membershipType, membershipId, "raid")})", true);
        embed.AddField("PvP",
            $"[Crucible.Report](https://crucible.report/report/{memTypeAndId})\n" +
            $"[DestinyTracker](https://destinytracker.com/destiny-2/profile/bungie/{membershipId}/overview?perspective=pvp)\n" +
            $"[Trials.Report](https://trials.report/report/{memTypeAndId})", true);

        var importantCollectibles = new StringBuilder();

        var importantIdList = new List<uint>
        {
            DefinitionHashes.Collectibles.Arbalest,
            DefinitionHashes.Collectibles.Divinity,
            DefinitionHashes.Collectibles.Gjallarhorn,
            DefinitionHashes.Collectibles.IzanagisBurden,
            DefinitionHashes.Collectibles.LegendofAcrius,
            DefinitionHashes.Collectibles.OneThousandVoices,
            DefinitionHashes.Collectibles.OutbreakPerfected,
            DefinitionHashes.Collectibles.Parasite,
            DefinitionHashes.Collectibles.Riskrunner,
            DefinitionHashes.Collectibles.SleeperSimulant,
            DefinitionHashes.Collectibles.Taipan4fr,
            DefinitionHashes.Collectibles.TheHothead,
            DefinitionHashes.Collectibles.TheLament,
            DefinitionHashes.Collectibles.TheWardcliffCoil,
            DefinitionHashes.Collectibles.Thunderlord,
            DefinitionHashes.Collectibles.TractorCannon,
            DefinitionHashes.Collectibles.Witherhoard,
            DefinitionHashes.Collectibles.Xenophage,
            DefinitionHashes.Collectibles.AeonSafe,
            DefinitionHashes.Collectibles.AeonSoul,
            DefinitionHashes.Collectibles.AeonSwift,
            DefinitionHashes.Collectibles.CelestialNighthawk,
            DefinitionHashes.Collectibles.CuirassoftheFallingStar,
            DefinitionHashes.Collectibles.LunafactionBoots
        };

        var importantList = new List<DestinyCollectibleDefinition>();
        foreach (var u in importantIdList)
            importantList.Add(
                await bungieClient.DefinitionProvider
                    .LoadDefinition<DestinyCollectibleDefinition>(u, BungieLocales.EN));

        var i = 0;
        var state = false;

        var profileCollectibles = playerResponse.Response.ProfileCollectibles.Data.Collectibles;
        var characterCollectibles = playerResponse.Response.CharacterCollectibles.Data.First().Value.Collectibles
            .ToDictionary(collectible => collectible.Key, collectible => collectible.Value);

        foreach (var collectibleDefinition in importantList)
        {
            if (profileCollectibles.TryGetValue(collectibleDefinition.Hash, out var collectible))
            {
                if (!collectible.State.HasFlag(DestinyCollectibleState.NotAcquired))
                    state = true;
            }
            else
            {
                if (characterCollectibles.TryGetValue(collectibleDefinition.Hash, out var item))
                    if (!item.State.HasFlag(DestinyCollectibleState.NotAcquired))
                        state = true;
            }

            importantCollectibles.Append(state ? '✅' : '❌');
            importantCollectibles.Append(
                $" - {EmoteHelper.GetItemType(collectibleDefinition.Item.Select(x => x.ItemSubType))} {collectibleDefinition.DisplayProperties.Name}\n");

            i++;
            state = false;

            // ReSharper disable once InvertIf
            if (i is 9 or 18 or 24)
            {
                embed.AddField("Collectibles", importantCollectibles.ToString(), i is not 24);
                importantCollectibles.Clear();
            }
        }

        return embed.Build();
    }

    private static int GetRaidCompletions(DestinyMetricsComponent metricsData)
    {
        var raidList = new List<uint>
        {
            DefinitionHashes.Metrics.LeviathanCompletions,
            DefinitionHashes.Metrics.EaterofWorldsCompletions,
            DefinitionHashes.Metrics.SpireofStarsCompletions,
            DefinitionHashes.Metrics.LastWishCompletions,
            DefinitionHashes.Metrics.ScourgeofthePastCompletions,
            DefinitionHashes.Metrics.CrownofSorrowCompletions,
            DefinitionHashes.Metrics.GardenofSalvationCompletions,
            DefinitionHashes.Metrics.DeepStoneCryptCompletions,
            DefinitionHashes.Metrics.VaultofGlassCompletions,
            DefinitionHashes.Metrics.VowoftheDiscipleCompletions,
            DefinitionHashes.Metrics.KingsFallCompletions
        };

        return raidList.Where(u => metricsData.Metrics[u].ObjectiveProgress.Progress is not null)
            .Sum(u => metricsData.Metrics[u].ObjectiveProgress.Progress!.Value);
    }

    private static string GetReportLink(BungieMembershipType membershipType, long membershipId, string reportType)
    {
        string platform;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (membershipType)
        {
            case BungieMembershipType.TigerXbox:
                platform = "xb";
                break;
            case BungieMembershipType.TigerPsn:
                platform = "ps";
                break;
            case BungieMembershipType.TigerSteam:
                platform = "pc";
                break;
            case BungieMembershipType.TigerStadia:
                platform = "stadia";
                break;
            default:
                return $"https://{reportType}.report";
        }

        return $"https://{reportType}.report/{platform}/{membershipId}";
    }
}