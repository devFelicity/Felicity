using Discord;
using Discord.WebSocket;
using Felicity.Models;
using Felicity.Options;
using Felicity.Util;
using Humanizer;
using Microsoft.Extensions.Options;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Felicity.Services.Hosted;

public class TwitchClientService : BackgroundService
{
    private static LiveStreamMonitorService? _monitorService;
    private readonly DiscordShardedClient _discordClient;
    private readonly IOptions<TwitchOptions> _twitchOptions;
    private readonly TwitchStreamDb _twitchStreamDb;
    private TwitchAPI? _twitchApi;

    public TwitchClientService(DiscordShardedClient discordClient,
        IOptions<TwitchOptions> twitchOptions, TwitchStreamDb twitchStreamDb)
    {
        _discordClient = discordClient;
        _twitchOptions = twitchOptions;
        _twitchStreamDb = twitchStreamDb;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _twitchApi = new TwitchAPI
        {
            Settings =
            {
                AccessToken = _twitchOptions.Value.AccessToken,
                ClientId = _twitchOptions.Value.ClientId
            }
        };

        ConfigureMonitor();

        return Task.CompletedTask;
    }

    private void ConfigureMonitor()
    {
        var streamList = new List<string>();

        foreach (var twitchStream in _twitchStreamDb.TwitchStreams)
            if (!streamList.Contains(twitchStream.TwitchName))
                streamList.Add(twitchStream.TwitchName);

        _monitorService = new LiveStreamMonitorService(_twitchApi);
        _monitorService.OnStreamOnline += OnStreamOnline;
        _monitorService.OnStreamOffline += OnStreamOffline;

        if (streamList.Count > 0)
        {
            _monitorService.SetChannelsByName(streamList);
            Log.Information($"Listening to Twitch streams from: {string.Join(", ", streamList)}");
            _monitorService.Start();
        }
        else
        {
            Log.Information("No streams to listen to.");
        }
    }

    private async void OnStreamOnline(object? sender, OnStreamOnlineArgs e)
    {
        Log.Information($"Processing online Twitch stream by {e.Channel} - Stream ID: {e.Stream.Id}");

        var streamList = _twitchStreamDb.TwitchStreams.Where(x =>
            string.Equals(x.TwitchName, e.Channel, StringComparison.CurrentCultureIgnoreCase));

        if (_twitchApi == null)
        {
            Log.Error("Twitch API is not initialized.");
            return;
        }

        var channelInfoTask = await _twitchApi.Helix.Users.GetUsersAsync(new List<string> { e.Stream.UserId });

        if (channelInfoTask == null || channelInfoTask.Users.Length == 0)
        {
            Log.Error("No channel found for online stream.");
            return;
        }

        var channelInfo = channelInfoTask.Users.FirstOrDefault();
        var timeStarted = e.Stream.StartedAt.GetTimestamp();

        var gameBoxImageTask = await _twitchApi.Helix.Games.GetGamesAsync(new List<string> { e.Stream.GameId });
        var gameBoxImage = gameBoxImageTask.Games.FirstOrDefault()?.BoxArtUrl;

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = e.Stream.UserName,
                IconUrl = channelInfo?.ProfileImageUrl
            },
            Color = Color.Green,
            ThumbnailUrl = gameBoxImage?.Replace("{width}x{height}", "150x200"),
            Title = e.Stream.Title,
            Url = $"https://twitch.tv/{e.Stream.UserName}",
            ImageUrl = e.Stream.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
            Footer = Embeds.MakeFooter(),
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

        foreach (var stream in streamList)
            try
            {
                var activeStream = new ActiveStream
                {
                    ConfigId = stream.Id,
                    StreamId = e.Stream.Id
                };

                var mention = "";
                if (stream.MentionEveryone)
                    mention = "@everyone ";
                else if (stream.MentionRole != 0)
                    mention = $"<@&{stream.MentionRole}> ";

                var mentionUser = stream.UserId == 0
                    ? e.Channel
                    : $"<@{stream.UserId}>";

                var message = await _discordClient.GetGuild(stream.ServerId)
                    .GetTextChannel(stream.ChannelId)
                    .SendMessageAsync(
                        $"{mentionUser} is now live: <https://twitch.tv/{e.Stream.UserName}>\n\n{mention}",
                        false, embed.Build());

                activeStream.MessageId = message.Id;

                _twitchStreamDb.ActiveStreams.Add(activeStream);
            }
            catch (Exception exception)
            {
                Log.Error($"Error in OnStreamOnline: {exception.GetType()}: {exception.Message}");
            }
    }

    private async void OnStreamOffline(object? sender, OnStreamOfflineArgs e)
    {
        Log.Information($"Processing offline Twitch stream by {e.Channel} - Stream ID: {e.Stream.Id}");

        if (_twitchApi == null)
        {
            Log.Error("Twitch API is not initialized.");
            return;
        }

        var activeStreams = _twitchStreamDb.ActiveStreams.Where(x => x.StreamId == e.Stream.Id);
        if (!activeStreams.Any())
        {
            Log.Error("No active streams found for monitored channel.");
            return;
        }

        var channelInfoTask = await _twitchApi.Helix.Users.GetUsersAsync(new List<string> { e.Stream.UserId });

        if (channelInfoTask == null || channelInfoTask.Users.Length == 0)
        {
            Log.Error("No channel found for online stream.");
            return;
        }

        var channelInfo = channelInfoTask.Users.FirstOrDefault();

        var vodList = await _twitchApi.Helix.Videos.GetVideoAsync(userId: e.Stream.UserId, type: VideoType.Archive, first: 1);

        var vodUrl = string.Empty;
        EmbedBuilder embed;

        if (vodList != null && vodList.Videos.Length != 0)
        {
            var vod = vodList.Videos.First();
            vodUrl = $" <https://www.twitch.tv/videos/{vod.Id}>";

            var unixTimestamp = DateTime.Parse(vod.CreatedAt).ToUniversalTime().GetTimestamp();
             
            embed = new EmbedBuilder
            {
                Color = Color.Red,
                Title = vod.Title,
                Url = vodUrl,
                ImageUrl = vod.ThumbnailUrl.Replace("%{width}x%{height}", "1280x720"),
                ThumbnailUrl = channelInfo?.ProfileImageUrl,
                Footer = Embeds.MakeFooter(),
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
        }
        else
        {
            embed = new EmbedBuilder
            {
                Color = Color.Purple,
                Title = e.Stream.Title,
                ImageUrl = e.Stream.ThumbnailUrl.Replace("{width}x{height}", "1280x720"),
                ThumbnailUrl = channelInfo?.ProfileImageUrl,
                Footer = Embeds.MakeFooter(),
                Fields = new List<EmbedFieldBuilder>
                {
                    new()
                    {
                        Name = "Started", Value = e.Stream.StartedAt.ToString("F"),
                        IsInline = true
                    },
                    new()
                    {
                        Name = "Duration", Value = (e.Stream.StartedAt - DateTime.Now).Humanize(),
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
        }

        var streamsToRemove = new List<ActiveStream>();

        foreach (var activeStream in activeStreams)
        {
            var message =
                ((SocketTextChannel)_discordClient.GetChannel(
                    _twitchStreamDb.TwitchStreams.FirstOrDefault(x => x.Id == activeStream.ConfigId)!.ChannelId))
                .GetMessageAsync(activeStream.MessageId)
                .Result;
            try
            {
                (message as IUserMessage)?.ModifyAsync(delegate(MessageProperties properties)
                {
                    properties.Content = $"{Format.Bold(e.Channel)} was live:{vodUrl}";
                    properties.Embed = embed.Build();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Error in Twitch OnStreamOffline\n{ex.GetType()}: {ex.Message}");
            }

            streamsToRemove.Add(activeStream);
        }

        _twitchStreamDb.ActiveStreams.RemoveRange(streamsToRemove);
    }

    public void RestartMonitor()
    {
        _monitorService?.Stop();
        ConfigureMonitor();

        Log.Information("Restarted TwitchMonitor.");
    }
}