using System.Diagnostics;
using Discord;
using Discord.Commands;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.Activities;
using DotNetBungieAPI.Models.Destiny.Definitions.ActivityModes;
using DotNetBungieAPI.Models.Requests;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Models.Caches;
using Felicity.Util;
using Humanizer;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Text;

public class BasicTextCommands : ModuleBase<ShardedCommandContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly TwitchStreamDb _twitchStreamDb;
    private readonly UserDb _userDb;

    public BasicTextCommands(TwitchStreamDb twitchStreamDb, UserDb userDb, IBungieClient bungieClient)
    {
        _twitchStreamDb = twitchStreamDb;
        _userDb = userDb;
        _bungieClient = bungieClient;
    }

    [Command("ping")]
    public async Task Pong()
    {
        // ReSharper disable once StringLiteralTypo
        await Context.Message.ReplyAsync("<:NOOOOOOOOOOOOOT:855149582177533983>");
    }

    [Command("fillCPs")]
    public async Task FillCPs(ulong messageId)
    {
        var msg = await Context.Channel.GetMessageAsync(messageId);
        ProcessCpData.Populate(msg);
    }

    [Command("metrics", RunMode = RunMode.Async)]
    public async Task Metrics()
    {
        var serverList = Context.Client.Guilds;
        await Context.Client.DownloadUsersAsync(serverList);
        var userList = new List<ulong>();

        foreach (var clientGuild in serverList)
        foreach (var clientGuildUser in clientGuild.Users)
        {
            if (clientGuildUser.IsBot)
                continue;

            if (!userList.Contains(clientGuildUser.Id))
                userList.Add(clientGuildUser.Id);
        }

        var manifest = await _bungieClient.DefinitionProvider.GetCurrentManifest();
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

        var embed = Embeds.MakeBuilder();
        embed.Title = "Felicity Metrics";
        embed.Description = "Overview of bot metrics.";
        embed.Color = Color.Teal;
        embed.ThumbnailUrl = "https://icons.iconarchive.com/icons/graphicloads/100-flat/256/analytics-icon.png";

        embed.AddField("Bot Version", BotVariables.Version, true);
        embed.AddField("Bot Uptime", uptime.Humanize(), true);
        embed.AddField("Discord Servers", $"{serverList.Count:n0}", true);
        embed.AddField("Discord Users", $"{userList.Count:n0}", true);
        embed.AddField("Streams", _twitchStreamDb.TwitchStreams.ToList().Count, true);
        embed.AddField("Registered Users", $"{_userDb.Users.ToList().Count:n0}", true);
        embed.AddField("Manifest Version", manifest.Version, true);

        await Context.Message.ReplyAsync(embed: embed.Build());
    }

    [Command("commonActivities", RunMode = RunMode.Async)]
    public async Task CommonActivities(string targetName)
    {
        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (user == null)
        {
            await ReplyAsync("Failed to fetch user profile.");
            return;
        }

        var activeCharacters = await _bungieClient.ApiAccess.Destiny2.GetProfile(user.DestinyMembershipType,
            user.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            });

        var characterIdList = activeCharacters.Response.Characters.Data.Keys.ToList();

        var activityList = new List<long>();

        foreach (var characterId in characterIdList)
        {
            var listQuery = await _bungieClient.ApiAccess.Destiny2.GetActivityHistory(user.DestinyMembershipType,
                user.DestinyMembershipId, characterId, 250);

            activityList.AddRange(
                listQuery.Response.Activities.Select(activity => activity.ActivityDetails.InstanceId));
        }

        var targetPlayer = await _bungieClient.ApiAccess.Destiny2.SearchDestinyPlayerByBungieName(
            BungieMembershipType.All,
            new ExactSearchRequest
            {
                DisplayName = targetName.Split("#")[0],
                DisplayNameCode = Convert.ToInt16(targetName.Split("#")[1])
            });

        if (targetPlayer.Response.Count == 0)
        {
            await ReplyAsync("Target player not found.");
            return;
        }

        var targetPlayerId = targetPlayer.Response.First().MembershipId;
        var title =
            $"Activities in common between: {user.BungieName} and {targetPlayer.Response.First().BungieGlobalDisplayName}#{targetPlayer.Response.First().BungieGlobalDisplayNameCode}";
        var response = title + "\n";

        foreach (var activityId in activityList)
        {
            var validPgcr = false;
            var pgcr = await _bungieClient.ApiAccess.Destiny2.GetPostGameCarnageReport(activityId);

            foreach (var destinyPostGameCarnageReportEntry in pgcr.Response.Entries)
                if (destinyPostGameCarnageReportEntry.Player.DestinyUserInfo.MembershipId == targetPlayerId)
                    validPgcr = true;

            if (validPgcr)
                response +=
                    $"{pgcr.Response.Period:g} - {pgcr.Response.ActivityDetails.Mode} - {pgcr.Response.ActivityDetails.ActivityReference.Select(x => x.DisplayProperties.Name)}\n";
        }

        await File.WriteAllTextAsync("tmp.txt", response);

        await Context.Channel.SendFileAsync("tmp.txt", "");
    }

    [Command("cpCount", RunMode = RunMode.Async)]
    public async Task ActivityList(int mode)
    {
        if (mode != 4 && mode != 82)
        {
            await ReplyAsync("Unknown mode, use 4 for raid or 82 for dungeon.");
            return;
        }

        var bungieNameList = new List<string>
        {
            "ScrubsInTubs#4331", "ttvLuckstruck9#0961", "ttvScrubsInTubs#8727",
            "ttvScrubsInTubs#0188", "ttvScrubsInTubs#3580", "ttvScrubsInTubs#1409",
            "ttvScrubsInTubs#2378", "ttvScrubsInTubs#7098", "ttvScrubsInTubs#2264",
            "ttvScrubsInTubs#0252", "ttvScrubsInTubs#5319"
        };

        var activityList = new List<ActivityReport>();
        var previousWeeklyReset = ResetUtils.GetNextWeeklyReset(DayOfWeek.Tuesday).AddDays(-7);

        foreach (var bungieName in bungieNameList)
        {
            var player = await BungieApiUtils.GetLatestProfile(_bungieClient, bungieName.Split("#")[0],
                Convert.ToInt16(bungieName.Split("#")[1]));

            if (player == null)
            {
                await ReplyAsync("Could not find player.");
                continue;
            }

            var characterTask = await _bungieClient.ApiAccess.Destiny2.GetProfile(player.MembershipType,
                player.MembershipId, new[]
                {
                    DestinyComponentType.Characters
                });

            var characterIds = characterTask.Response.Characters.Data.Keys.ToList();

            foreach (var characterId in characterIds)
            {
                Console.WriteLine($"Checking character {characterId}...");

                var t = await _bungieClient.ApiAccess.Destiny2.GetActivityHistory(player.MembershipType,
                    player.MembershipId,
                    characterId, 100, (DestinyActivityModeType)mode);

                foreach (var historicalStat in t.Response.Activities)
                    if (historicalStat.Period > previousWeeklyReset)
                    {
                        Console.WriteLine(
                            $"Adding {historicalStat.ActivityDetails.InstanceId} from {historicalStat.Period}");
                        activityList.Add(new ActivityReport
                        {
                            ActivityId = historicalStat.ActivityDetails.ActivityReference.Hash,
                            InstanceId = historicalStat.ActivityDetails.InstanceId
                        });
                    }
            }
        }

        var grouped = activityList.GroupBy(x => x.ActivityId).ToList();

        var response = Format.Bold($"Since {previousWeeklyReset}") +
                       $", {activityList.Count} instances have been used to give out:\n";

        var responseList = new Dictionary<string, string>();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var keyPair in grouped)
        {
            if (keyPair.Key == null)
                continue;

            if (!_bungieClient.Repository.TryGetDestinyDefinition<DestinyActivityDefinition>((uint)keyPair.Key,
                    BungieLocales.EN,
                    out var activityDef))
                continue;

            var theList = keyPair.ToList();

            var characterList = new List<long>();

            foreach (var activityReport in theList)
            {
                var pgcr =
                    await _bungieClient.ApiAccess.Destiny2.GetPostGameCarnageReport(activityReport.InstanceId);

                foreach (var destinyPostGameCarnageReportEntry in pgcr.Response.Entries)
                    if (!characterList.Contains(destinyPostGameCarnageReportEntry.CharacterId))
                        characterList.Add(destinyPostGameCarnageReportEntry.CharacterId);
            }

            responseList.Add(activityDef.DisplayProperties.Name,
                $"{characterList.Count} checkpoints on {Format.Bold(activityDef.DisplayProperties.Name)}.\n");
        }

        response = responseList.OrderBy(x => x.Key)
            .Aggregate(response, (current, keyValuePair) => current + keyValuePair.Value);

        await ReplyAsync(response);
    }

    private class ActivityReport
    {
        public uint? ActivityId { get; init; }
        public long InstanceId { get; init; }
    }
}