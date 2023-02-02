using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Requests;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Util;
using Humanizer;
using RunMode = Discord.Commands.RunMode;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Text;

public class BasicTextCommands : ModuleBase<ShardedCommandContext>
{
    private readonly IBungieClient _bungieClient;
    private readonly InteractionService _interactionService;
    private readonly TwitchStreamDb _twitchStreamDb;
    private readonly UserDb _userDb;

    public BasicTextCommands(TwitchStreamDb twitchStreamDb, UserDb userDb, IBungieClient bungieClient,
        InteractionService interactionService)
    {
        _twitchStreamDb = twitchStreamDb;
        _userDb = userDb;
        _bungieClient = bungieClient;
        _interactionService = interactionService;
    }

    [Command("serverList")]
    public async Task ServerList()
    {
        var sb = new StringBuilder();

        foreach (var socketGuild in Context.Client.Guilds)
            sb.Append($"{socketGuild.Id} - {socketGuild.Name}\n");

        await File.WriteAllTextAsync("serverList.txt", sb.ToString());

        await Context.Channel.SendFileAsync("serverList.txt");
    }

    [Command("leaveServer")]
    public async Task LeaveServer(ulong serverId)
    {
        try
        {
            await Context.Client.GetGuild(serverId).LeaveAsync();
        }
        catch (Exception e)
        {
            await ReplyAsync($"{e.GetType()}: {e.Message}");
        }
    }

    [Command("clarity")]
    public async Task Clarity(uint itemHash)
    {
        var clarityDb = await ClarityParser.Fetch();

        var clarityValue = clarityDb?[itemHash.ToString()];

        var returnString = new StringBuilder();

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (clarityValue?.Type)
        {
            case TypeEnum.ArmorPerkExotic:
                returnString.Append(
                    $"**Name:** {clarityValue.Name} ({clarityValue.Hash})\n**Item:** {clarityValue.ItemName} ({clarityValue.ItemHash})\n{Format.Code(clarityValue.Description?.ClarityClean())}");
                break;
            case TypeEnum.WeaponMod:
            case TypeEnum.WeaponOriginTrait:
            case TypeEnum.WeaponPerk:
            case TypeEnum.WeaponPerkEnhanced:
                returnString.Append(
                    $"**Name:** {clarityValue.Name} ({clarityValue.Hash})\n{Format.Code(clarityValue.Description?.ClarityClean())}");
                break;
            default:
                await ReplyAsync("Unknown type.");
                return;
        }

        await ReplyAsync(returnString.ToString());
    }

    [Command("help")]
    public async Task Help()
    {
        var commands = _interactionService.SlashCommands.ToList();
        var embedBuilder = new EmbedBuilder();

        foreach (var command in commands)
        {
            embedBuilder.Description += Format.Bold(command.Name) + "\n";
            var description = command.Description ?? "No description available";
            embedBuilder.Description += $"> {description}\n\n";
        }

        await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
    }

    [Command("ping")]
    public async Task Pong()
    {
        // ReSharper disable once StringLiteralTypo
        await Context.Message.ReplyAsync("<:NOOOOOOOOOOOOOT:855149582177533983>");
    }

    [Command("metrics", RunMode = RunMode.Async)]
    public async Task Metrics()
    {
        var serverList = Context.Client.Guilds;

        /*try
        {
            await Context.Client.DownloadUsersAsync(serverList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var userList = new List<ulong>();

        foreach (var clientGuild in serverList)
        foreach (var clientGuildUser in clientGuild.Users)
        {
            if (clientGuildUser.IsBot)
                continue;

            if (!userList.Contains(clientGuildUser.Id))
                userList.Add(clientGuildUser.Id);
        }*/

        var userCount = serverList.Aggregate(0, (current, server) => current + server.Users.Count);

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
        embed.AddField("Discord Users", $"{userCount:n0}", true);
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
}