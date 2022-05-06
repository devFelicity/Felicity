using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Felicity.Configs;
using Felicity.Enums;
using Felicity.Helpers;
using Newtonsoft.Json;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Felicity.Services;

internal static class TwitchService
{
    private static TwitchAPI Api;
    private static DiscordSocketClient Client;
    private static LiveStreamMonitorService monitorService;
    private static readonly List<ActiveStream> activeStreams = new();

    public static void Setup(DiscordSocketClient client)
    {
        Log.Information("Setting up TwitchMonitor");

        Client = client;
        Api = new TwitchAPI
        {
            Settings =
            {
                AccessToken = ConfigHelper.GetBotSettings().TwitchAccessToken,
                ClientId = ConfigHelper.GetBotSettings().TwitchClientId
            }
        };
    }

    public static void ConfigureMonitor()
    {
        var serverSettings = ServerConfig.FromJson();
        if (serverSettings == null)
            return;

        // key = serverid, value = settings
        foreach (var serverSetting in serverSettings.Settings)
        {
            if (serverSetting.Value.TwitchStreams.Count <= 0) continue;

            // key = twitch name
            // value = settings
            foreach (var twitchStream in serverSetting.Value.TwitchStreams)
            {
                var newStream = new ActiveStream {TwitchName = twitchStream.Key};
                newStream.TwitchStreams.Add(Convert.ToUInt64(serverSetting.Key), new TwitchStream
                {
                    Mention = twitchStream.Value.Mention,
                    MentionEveryone = twitchStream.Value.MentionEveryone,
                    UserId = twitchStream.Value.UserId,
                    ChannelId = twitchStream.Value.ChannelId
                });

                if(!activeStreams.Contains(newStream))
                    activeStreams.Add(newStream);
            }
        }

        var streamNames = new List<string>();
        foreach (var activeStream in activeStreams.Where(activeStream => !streamNames.Contains(activeStream.TwitchName)))
            streamNames.Add(activeStream.TwitchName);

        monitorService = new LiveStreamMonitorService(Api);
        monitorService.OnStreamOnline += OnStreamOnline;
        monitorService.OnStreamOffline += OnStreamOffline;
        monitorService.SetChannelsByName(streamNames);
        Log.Information($"Listening to Twitch streams from: {string.Join(", ", streamNames)}");
        monitorService.Start();
    }

    public static void RestartMonitor()
    {
        monitorService.Stop();
        ConfigureMonitor();

        Log.Information("Restarted TwitchMonitor");
    }

    private static async void OnStreamOnline(object sender, OnStreamOnlineArgs e)
    {
        Log.Information($"Processing online Twitch stream by {e.Channel} - Stream ID: {e.Stream.Id}");

        var filePath = $"Configs/{e.Channel.ToLower()}-{e.Stream.Id}.txt";

        if (File.Exists(filePath))
        {
            Log.Warning("Stream was already posted.");
            return;
        }

        var channelInfo = Api.Helix.Users.GetUsersAsync(new List<string> {e.Stream.UserId}).Result.Users
            .FirstOrDefault();
        var timeStarted = (int) e.Stream.StartedAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        var embed = new EmbedBuilder
        {
            Color = Color.Purple,
            ThumbnailUrl = channelInfo?.ProfileImageUrl,
            Title = e.Stream.Title,
            Url = $"https://twitch.tv/{e.Stream.UserName}",
            ImageUrl = e.Stream.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
            Footer = new EmbedFooterBuilder
            {
                IconUrl = Images.FelicityLogo,
                Text = Strings.FelicityVersion
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new()
                {
                    Name = "Game",
                    Value = string.IsNullOrEmpty(e.Stream.GameName) ? "No Game" : e.Stream.GameName,
                    IsInline = true
                },
                new()
                {
                    Name = "Started",
                    Value = $"<t:{timeStarted}:R>",
                    IsInline = true
                }
            }
        };

        var currentStream = activeStreams.First(x => x.TwitchName == e.Channel.ToLower());

        var messageIds = new Dictionary<ulong, ulong>();

        foreach (var activeStream in currentStream.TwitchStreams)
            try
            {
                var mention = "";
                if (activeStream.Value.MentionEveryone)
                    mention = "@everyone ";
                else if (activeStream.Value.Mention != 0)
                    mention = $"<@&{activeStream.Value.Mention}> ";

                var mentionUser = activeStream.Value.UserId == 0 ? e.Channel : $"<@{activeStream.Value.UserId}>";

                var message = await Client.GetGuild(activeStream.Key).GetTextChannel(activeStream.Value.ChannelId)
                    .SendMessageAsync(
                        $"{mentionUser} is now live: <https://twitch.tv/{e.Stream.UserName}>\n\n{mention}",
                        false, embed.Build());

                messageIds.Add(activeStream.Value.ChannelId, message.Id);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Twitch OnStreamOnline");
                Log.Error($"{ex.GetType()}: {ex.Message}");
            }

        await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(messageIds));
    }

    private static void OnStreamOffline(object sender, OnStreamOfflineArgs e)
    {
        Log.Information($"Processing offline Twitch stream by {e.Channel} - Stream ID: {e.Stream.Id}");

        var filePath = $"Configs/{e.Channel.ToLower()}-{e.Stream.Id}.txt";

        if (!File.Exists(filePath))
        {
            Log.Error("No monitored stream found linked to ending stream.");
            return;
        }

        var channelInfo = Api.Helix.Users.GetUsersAsync(new List<string> {e.Stream.UserId}).Result.Users
            .FirstOrDefault();

        if (channelInfo == null)
        {
            Log.Error("No channel found for stream.");
            return;
        }

        var vodList = Api.Helix.Videos.GetVideoAsync(userId: e.Stream.UserId, type: VideoType.Archive, first: 1).Result;
        if (vodList.Videos.Length != 0)
        {
            var vod = vodList.Videos.First();
            var vodUrl = $"https://www.twitch.tv/videos/{vod.Id}";

            var unixTimestamp = (int) DateTime.Parse(vod.CreatedAt).ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            var embed = new EmbedBuilder
            {
                Color = Color.Purple,
                Title = vod.Title,
                Url = vodUrl,
                ImageUrl = vod.ThumbnailUrl.Replace("%{width}x%{height}", "1280x720"),
                ThumbnailUrl = channelInfo.ProfileImageUrl,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Images.FelicityLogo,
                    Text = Strings.FelicityVersion
                },
                Fields = new List<EmbedFieldBuilder>
                {
                    new()
                    {
                        Name = "Started", Value = $"<t:{unixTimestamp}:f>",
                        IsInline = true
                    },
                    new()
                    {
                        Name = "Duration", Value = vod.Duration,
                        IsInline = true
                    },
                    new()
                    {
                        Name = "Game",
                        Value = string.IsNullOrEmpty(e.Stream.GameName) ? "No Game" : e.Stream.GameName,
                        IsInline = true
                    }
                }
            };

            var messageIdList = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText(filePath));

            if (messageIdList != null)
            {
                foreach (var (channelId, messageId) in messageIdList)
                {
                    var message = ((SocketTextChannel) Client.GetChannel(channelId)).GetMessageAsync(messageId)
                        .Result;
                    try
                    {
                        (message as IUserMessage)?.ModifyAsync(delegate(MessageProperties properties)
                        {
                            properties.Content = $"{Format.Bold(e.Channel)} was live: <{vodUrl}>";
                            properties.Embed = embed.Build();
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error in Twitch OnStreamOffline");
                        Log.Error($"{ex.GetType()}: {ex.Message}");
                    }
                }
            }

            File.Delete(filePath);
        }
        else
        {
            try
            {
                var embed = new EmbedBuilder
                {
                    Color = Color.Purple,
                    Title = e.Stream.Title,
                    ImageUrl = e.Stream.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
                    ThumbnailUrl = channelInfo.ProfileImageUrl,
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Images.FelicityLogo,
                        Text = Strings.FelicityVersion
                    },
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new()
                        {
                            Name = "Started", Value = e.Stream.StartedAt.ToString("F"),
                            IsInline = true
                        },
                        new()
                        {
                            Name = "Duration", Value = (e.Stream.StartedAt - DateTime.Now).TotalHours,
                            IsInline = true
                        },
                        new()
                        {
                            Name = "Game",
                            Value = string.IsNullOrEmpty(e.Stream.GameName) ? "No Game" : e.Stream.GameName,
                            IsInline = true
                        }
                    }
                };

                var messageIdList = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText(filePath));
                if (messageIdList != null)
                {
                    foreach (var (channelId, messageId) in messageIdList)
                    {
                        var message = ((SocketTextChannel)Client.GetChannel(channelId)).GetMessageAsync(messageId)
                            .Result;

                        (message as IUserMessage)?.ModifyAsync(delegate (MessageProperties properties)
                        {
                            properties.Content = $"{Format.Bold(e.Channel)} was live:";
                            properties.Embed = embed.Build();
                        });
                    }
                }

                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Twitch OnStreamOffline");
                Log.Error($"{ex.GetType()}: {ex.Message}");
            }
        }
    }

    public static bool UserExists(string twitchName)
    {
        var test = Api.Helix.Users.GetUsersAsync(logins: new List<string> {twitchName}).Result;
        return test.Users.Length > 0;
    }

    private class ActiveStream
    {
        public string TwitchName { get; init; }
        public Dictionary<ulong, TwitchStream> TwitchStreams { get; } = new();
    }
}