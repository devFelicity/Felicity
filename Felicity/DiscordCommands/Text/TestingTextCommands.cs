using System.Diagnostics;
using Discord;
using Discord.Commands;
using DotNetBungieAPI.Clients;
using Felicity.Models;
using Felicity.Models.Caches;
using Felicity.Util;
using Humanizer;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Text;

public class BasicTextCommands : ModuleBase<ShardedCommandContext>
{
    private readonly TwitchStreamDb _twitchStreamDb;
    private readonly UserDb _userDb;
    private readonly IBungieClient _bungieClient;

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
        {
            foreach (var clientGuildUser in clientGuild.Users)
            {
                if (clientGuildUser.IsBot)
                    continue;

                if (!userList.Contains(clientGuildUser.Id))
                    userList.Add(clientGuildUser.Id);
            }
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
        embed.AddField("Discord Servers", serverList.Count, true);
        embed.AddField("Discord Users", userList.Count, true);
        embed.AddField("Streams", _twitchStreamDb.TwitchStreams.ToList().Count, true);
        embed.AddField("Registered Users", _userDb.Users.ToList().Count, true);
        embed.AddField("Manifest Version", manifest.Version, true);

        await Context.Message.ReplyAsync(embed: embed.Build());
    }
}