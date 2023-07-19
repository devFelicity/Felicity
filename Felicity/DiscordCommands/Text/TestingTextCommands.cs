using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions;
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

    [Command("vendorUser", RunMode = RunMode.Async)]
    public async Task VendorUser()
    {
        var sw = Stopwatch.StartNew();
        var validUsers = _userDb.Users.Where(x => x.OAuthRefreshExpires > DateTime.Now).ToList();
        var nowTime = DateTime.Now;
        var tasks = new List<Task<string>>();
        
        foreach (var validUser in validUsers)
        {
            var user = validUser;

            if (validUser.OAuthTokenExpires < nowTime)
            {
                try
                {
                    user = await validUser.RefreshToken(_bungieClient, nowTime);
                    await _userDb.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{user.BungieName} - {ex.GetType()}: {ex.Message}");
                    continue;
                }
            }

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var characterId = await _bungieClient.ApiAccess.Destiny2.GetProfile(user.DestinyMembershipType,
                        user.DestinyMembershipId, new[] { DestinyComponentType.Characters });

                    var saintVendor = await _bungieClient.ApiAccess.Destiny2.GetVendor(user.DestinyMembershipType,
                        user.DestinyMembershipId, characterId.Response.Characters.Data.FirstOrDefault().Value.CharacterId,
                        DefinitionHashes.Vendors.Saint14, new[] { DestinyComponentType.Vendors }, user.GetTokenData());

                    var saladinVendor = await _bungieClient.ApiAccess.Destiny2.GetVendor(user.DestinyMembershipType,
                        user.DestinyMembershipId, characterId.Response.Characters.Data.FirstOrDefault().Value.CharacterId,
                        DefinitionHashes.Vendors.LordSaladin, new[] { DestinyComponentType.Vendors }, user.GetTokenData());

                    Console.WriteLine($"Finished task for {user.BungieName}");
                    return $"{user.BungieName}: Saint-14: {saintVendor.Response.Vendor.Data.Progression.CurrentResetCount}, Saladin: {saladinVendor.Response.Vendor.Data.Progression.CurrentResetCount}";
                }
                catch
                {
                    Console.WriteLine($"Finished task for {user.BungieName}");
                    return $"{user.BungieName}: Invalid Response";
                }
            }));
        }

        var results = await Task.WhenAll(tasks);

        await File.WriteAllTextAsync("Data/tmp-vendorUsers.txt", string.Join(Environment.NewLine, results.ToList()));
        await Context.Channel.SendFileAsync("Data/tmp-vendorUsers.txt", $"Query executed in {sw.Elapsed.Humanize()}");
    }

    [Command("lastActive")]
    public async Task LastActive()
    {
        var server = Context.Client.GetGuild(960484926950637608);
        var channel = server?.GetTextChannel(989885760884842526);
        if (channel != null)
        {
            var messageList = channel.GetMessagesAsync(1);
            await foreach (var message in messageList)
                if (message != null)
                {
                    await ReplyAsync(message.First().Timestamp.ToString());

                    var timeDiff = DateTime.UtcNow - message.First().Timestamp;
                    if (timeDiff <= TimeSpan.FromMinutes(15))
                    {
                        // post message
                    }

                    // don't post message
                    await ReplyAsync(timeDiff.Humanize());
                }
                else
                {
                    await ReplyAsync("Cannot parse message.");
                }
        }
        else
        {
            await ReplyAsync("Cannot parse channel.");
        }
    }

    [Command("vendorJson")]
    public async Task VendorJson(ulong userId, uint vendorId)
    {
        var user = _userDb.Users.First(x => x.DiscordId == userId);

        var characterIdTask = await _bungieClient.ApiAccess.Destiny2.GetProfile(user.DestinyMembershipType,
            user.DestinyMembershipId, new[]
            {
                DestinyComponentType.Characters
            }, user.GetTokenData());

        var request = await _bungieClient.ApiAccess.Destiny2.GetVendor(user.DestinyMembershipType,
            user.DestinyMembershipId,
            characterIdTask.Response.Characters.Data.Keys.First(), vendorId, new[]
            {
                DestinyComponentType.Vendors, DestinyComponentType.VendorCategories, DestinyComponentType.VendorSales,
                DestinyComponentType.ItemSockets
            }, user.GetTokenData());

        await File.WriteAllTextAsync("tmpVendor.json", JsonSerializer.Serialize(request));

        await Context.Channel.SendFileAsync("tmpVendor.json");
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

    [Command("updateManifest")]
    public async Task UpdateManifest()
    {
        var msg = await ReplyAsync("Checking for manifest updates...");

        if (await _bungieClient.DefinitionProvider.CheckForUpdates())
        {
            await msg.ModifyAsync(x => x.Content = "Update found, updating...");
            await _bungieClient.DefinitionProvider.Update();
            await msg.ModifyAsync(x => x.Content = "Downloaded files, clearing cache...");
            _bungieClient.Repository.Clear();
            await msg.ModifyAsync(x => x.Content = "Reading new files...");
            var manifest = await _bungieClient.ApiAccess.Destiny2.GetDestinyManifest();
            await _bungieClient.DefinitionProvider.ChangeManifestVersion(manifest.Response.Version);
            await _bungieClient.DefinitionProvider.ReadToRepository(_bungieClient.Repository);
            await msg.ModifyAsync(x => x.Content = "Done.");
        }
        else
        {
            await msg.ModifyAsync(x => x.Content = "No update found.");
        }
    }
}