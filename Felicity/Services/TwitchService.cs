using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
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
    private static LiveStreamMonitorService monitorService;

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

        ConfigureMonitor();
        monitorService.Start();
    }

    public static void ConfigureMonitor()
    {
        var members = ConfigHelper.GetTwitchSettings().Users.Select(user => user.Value.Name).ToList();
        monitorService = new LiveStreamMonitorService(TwitchService.Api);
        monitorService.OnStreamOnline += TwitchService.OnStreamOnline;
        monitorService.OnStreamOffline += TwitchService.OnStreamOffline;
        monitorService.SetChannelsByName(members);
        Log.Information($"Listening to Twitch streams from: {string.Join(", ", members)}");
    }

    public static void RestartMonitor()
    {
        monitorService.Stop();
        ConfigureMonitor();
        monitorService.Start();

        Log.Information("Restarted TwitchMonitor");
    }

    public static async void OnStreamOnline(object sender, OnStreamOnlineArgs e)
    {
        Log.Information($"Processing online Twitch stream by {e.Channel} - Stream ID: {e.Stream.Id}");

        var currentUser =
            (from user in ConfigHelper.GetTwitchSettings().Users
                where string.Equals(user.Value.Name, e.Stream.UserName, StringComparison.CurrentCultureIgnoreCase)
                select user.Value).FirstOrDefault();

        if (currentUser == null)
        {
            Log.Error("User not found in Twitch config.");
            return;
        }

        var filePath = $"Configs/{currentUser.Name.ToLower()}-{e.Stream.Id}.txt";

        if (File.Exists(filePath))
        {
            Log.Warning("Stream was already posted.");
            return;
        }

        var channelInfo = Api.Helix.Users.GetUsersAsync(new List<string> {e.Stream.UserId}).Result.Users
            .FirstOrDefault();
        var timeStarted = (int)e.Stream.StartedAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        var embed = new EmbedBuilder
        {
            Color = Color.Purple,
            Author = new EmbedAuthorBuilder
            {
                IconUrl = channelInfo == null ? "" : channelInfo.ProfileImageUrl,
                Name = $"{e.Stream.UserName} is now live on Twitch:",
                Url = $"https://twitch.tv/{e.Stream.UserName}"
            },
            Title = e.Stream.Title,
            Url = $"https://twitch.tv/{e.Stream.UserName}",
            ImageUrl = e.Stream.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
            Footer = new EmbedFooterBuilder
            {
                IconUrl = "https://whaskell.pw/images/felicity.jpg",
                Text = "Felicity // whaskell.pw"
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new() {Name = "─── Game ───", Value = e.Stream.GameName, IsInline = true},
                new() {Name = "─── Started ───", Value = $"<t:{timeStarted}:R>", IsInline = true}
            }
        };

        try
        {
            var mention = "";
            if (currentUser.MentionEveryone)
                mention = "@everyone ";
            else if (currentUser.Mention != 0)
                mention = $"<@&{currentUser.Mention}> ";

            var message = await Client.GetGuild(currentUser.ServerId).GetTextChannel(currentUser.ChannelId)
                .SendMessageAsync(
                    $"<@{currentUser.UserId}> is now live: <https://twitch.tv/{e.Stream.UserName}>\n{mention}",
                    false, embed.Build());

            await File.WriteAllTextAsync(filePath, message.Id.ToString());
        }
        catch (Exception ex)
        {
            Log.Error("Error in Twitch OnStreamOnline");
            Log.Error($"{ex.GetType()}: {ex.Message}");
        }
    }

    public static void OnStreamOffline(object sender, OnStreamOfflineArgs e)
    {
        Log.Information($"Processing offline Twitch stream by {e.Channel} - Stream ID: {e.Stream.Id}");
        var userFromConfig =
            (from user in ConfigHelper.GetTwitchSettings().Users
                where string.Equals(user.Value.Name, e.Channel, StringComparison.CurrentCultureIgnoreCase)
                select user.Value).FirstOrDefault();

        if (userFromConfig == null)
        {
            Log.Error("Twitch stream user not found in config.");
            return;
        }

        var filePath = $"Configs/{e.Channel.ToLower()}-{e.Stream.Id}.txt";

        if (!File.Exists(filePath))
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

        var messageId = ulong.Parse(File.ReadAllText(filePath));
        var message = ((SocketTextChannel)Client.GetChannel(userFromConfig.ChannelId)).GetMessageAsync(messageId).Result;

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
                new() {Name = "─── Started  ───", Value = $"<t:{e.Stream.StartedAt}:f>", IsInline = true},
                new() {Name = "─── Duration ───", Value = vod.Duration, IsInline = true},
                new() {Name = "─── Game     ───", Value = e.Stream.GameName, IsInline = false},
                new() {Name = "─── Views    ───", Value = vod.ViewCount, IsInline = true}
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

        File.Delete(filePath);
    }
}