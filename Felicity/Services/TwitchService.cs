using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Felicity.Configs;
using Felicity.Helpers;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Felicity.Services;

internal class TwitchService
{
    public static TwitchAPI Api;
    public static DiscordSocketClient Client;

    // ulong is announcement message id
    private static readonly Dictionary<User, ulong> monitoredLiveStreams = new();

    public static void Setup(DiscordSocketClient client)
    {
        Log.Information("Setting up TwitchMonitor");

        Client = client;
        Api = new TwitchAPI
        {
            Settings =
            {
                AccessToken = ConfigHelper.GetTwitchSettings().AccessToken,
                ClientId = ConfigHelper.GetTwitchSettings().ClientId
            }
        };

        var members = ConfigHelper.GetTwitchSettings().Users.Select(user => user.Value.Name).ToList();

        var monitorService = new LiveStreamMonitorService(Api);
        monitorService.SetChannelsByName(members);
        monitorService.OnStreamOnline += OnStreamOnline;
        monitorService.OnStreamOffline += OnStreamOffline;
        monitorService.Start();

        Log.Information($"Listening to Twitch streams from: {string.Join(", ", members)}");
    }

    private static async void OnStreamOnline(object sender, OnStreamOnlineArgs e)
    {
        Log.Information($"Processing online Twitch stream by {e.Channel}");

        var myStream = e.Stream;
        var channelInfo = Api.Helix.Users.GetUsersAsync(new List<string> {e.Stream.UserId}).Result.Users
            .FirstOrDefault();
        var timeStarted = (int) myStream.StartedAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        var embed = new EmbedBuilder
        {
            Color = Color.Purple,
            Author = new EmbedAuthorBuilder
            {
                IconUrl = channelInfo == null ? "" : channelInfo.ProfileImageUrl,
                Name = $"{myStream.UserName} is now live on Twitch:",
                Url = $"https://twitch.tv/{myStream.UserName}"
            },
            Title = myStream.Title,
            Url = $"https://twitch.tv/{myStream.UserName}",
            ImageUrl = myStream.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
            Footer = new EmbedFooterBuilder
            {
                IconUrl = "https://whaskell.pw/images/felicity.jpg",
                Text = "Felicity // whaskell.pw"
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new() {Name = "─── Game ───", Value = myStream.GameName, IsInline = true},
                new() {Name = "─── Started ───", Value = $"<t:{timeStarted}:R>", IsInline = true}
            }
        };

        var currentUser =
            (from user in ConfigHelper.GetTwitchSettings().Users
                where string.Equals(user.Value.Name, myStream.UserName, StringComparison.CurrentCultureIgnoreCase)
                select user.Value).FirstOrDefault();

        try
        {
            if (currentUser == null)
                return;

            var mention = "";
            if (currentUser.MentionEveryone)
                mention = "@everyone ";
            else if (currentUser.Mention != 0)
                mention = $"<@&{currentUser.Mention}> ";

            var message = await Client.GetGuild(currentUser.ServerId).GetTextChannel(currentUser.ChannelId)
                .SendMessageAsync(
                    $"<@{currentUser.UserId}> is now live: <https://twitch.tv/{myStream.UserName}>\n{mention}",
                    false, embed.Build());

            monitoredLiveStreams.Add(currentUser, message.Id);
        }
        catch (Exception ex)
        {
            Log.Error("Error in Twitch OnStreamOnline");
            Log.Error($"{ex.GetType()}: {ex.Message}");
        }
    }

    private static void OnStreamOffline(object sender, OnStreamOfflineArgs e)
    {
        Log.Information($"Processing offline Twitch stream by {e.Channel}");
        var userFromConfig =
            (from user in ConfigHelper.GetTwitchSettings().Users
                where string.Equals(user.Value.Name, e.Channel, StringComparison.CurrentCultureIgnoreCase)
                select user.Value).FirstOrDefault();

        if (userFromConfig == null)
        {
            Log.Error("Twitch stream user not found in config.");
            return;
        }

        if (!monitoredLiveStreams.ContainsKey(userFromConfig))
        {
            Log.Error("No monitored stream found linked to ending stream.");
            return;
        }

        var channelInfo = Api.Helix.Users.GetUsersAsync(new List<string> { e.Stream.UserId }).Result.Users
            .FirstOrDefault();

        if (channelInfo == null)
        {
            Log.Error("No channel found for stream.");
            return;
        }

        // ReSharper disable once UseDeconstruction
        var monitoredLiveStream =
            (from user in monitoredLiveStreams
                where string.Equals(user.Key.Name, userFromConfig.Name, StringComparison.CurrentCultureIgnoreCase)
                select user).FirstOrDefault();

        var message = ((SocketTextChannel)Client.GetChannel(userFromConfig.ChannelId)).GetMessageAsync(monitoredLiveStream.Value).Result;

        var vod = Api.Helix.Videos.GetVideoAsync(userId: e.Stream.UserId, type: VideoType.Archive, first: 1).Result.Videos.First();
        var vodUrl = $"https://www.twitch.tv/videos/{vod.Id}";

        var embed = new EmbedBuilder
        {
            Color = Color.Purple,
            Author = new EmbedAuthorBuilder
            {
                IconUrl = channelInfo.ProfileImageUrl,
                Name = $"{e.Stream.UserName} was live on Twitch:",
                Url = vodUrl
            },
            Title = e.Stream.Title,
            Url = vodUrl,
            ImageUrl = vod.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
            Footer = new EmbedFooterBuilder
            {
                IconUrl = "https://whaskell.pw/images/felicity.jpg",
                Text = "Felicity // whaskell.pw"
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new() {Name = "─── Started ───", Value = $"<t:{e.Stream.StartedAt}:f>", IsInline = true},
                new() {Name = "─── Duration ───", Value = vod.Duration, IsInline = true},
                new() {Name = "─── Game ───", Value = e.Stream.GameName, IsInline = false},
                new() {Name = "─── Views ───", Value = vod.ViewCount, IsInline = true}
            }
        };

        try
        {
            (message as SocketUserMessage)?.ModifyAsync(delegate(MessageProperties properties)
            {
                properties.Content = $"<@{userFromConfig.UserId}> is was live: <{vodUrl}>";
                properties.Embed = embed.Build();
            });
        }
        catch (Exception ex)
        {
            Log.Error("Error in Twitch OnStreamOffline");
            Log.Error($"{ex.GetType()}: {ex.Message}");
        }

        monitoredLiveStreams.Remove(monitoredLiveStream.Key);
    }
}