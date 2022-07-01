using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Felicity.Models;
using Felicity.Models.Caches;
using Felicity.Options;
using Felicity.Util;
using Microsoft.Extensions.Options;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;

// ReSharper disable CommentTypo
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Text;

public class BasicTextCommands : ModuleBase<ShardedCommandContext>
{
    private readonly TwitchStreamDb _twitchStreamDb;
    private readonly TwitchAPI _twitchApi;
    private IOptions<TwitchOptions> _twitchOptions;

    public BasicTextCommands(TwitchStreamDb twitchStreamDb, IOptions<TwitchOptions> twitchOptions)
    {
        _twitchStreamDb = twitchStreamDb;
        _twitchOptions = twitchOptions;
        _twitchApi = new TwitchAPI
        {
            Settings =
            {
                AccessToken = twitchOptions.Value.AccessToken,
                ClientId = twitchOptions.Value.ClientId
            }
        };;
    }

    [Command("ping")]
    public async Task Pong()
    {
        await ReplyAsync("<:NOOOOOOOOOOOOOT:855149582177533983>");
    }

    [Command("fillCPs")]
    public async Task FillCPs(ulong messageId)
    {
        var msg = await Context.Channel.GetMessageAsync(messageId);
        ProcessCpData.Populate(msg);
    }

    [Command("simStream")]
    public async Task SimStream()
    {
        try
        {
            var streamId = "46719229453";
            var streamUserId = "144525628";

            var activeStreams = _twitchStreamDb.ActiveStreams.Where(x => x.StreamId.ToString() == streamId).ToList();
            if (!activeStreams.Any())
            {
                Log.Error("No active streams found for monitored channel.");
                return;
            }

            var channelInfoTask = await _twitchApi.Helix.Users.GetUsersAsync(new List<string> { streamUserId });

            if (channelInfoTask == null || channelInfoTask.Users.Length == 0)
            {
                Log.Error("No channel found for online stream.");
                return;
            }

            var channelInfo = channelInfoTask.Users.FirstOrDefault();

            var vodList =
                await _twitchApi.Helix.Videos.GetVideoAsync(userId: streamUserId, type: VideoType.Archive, first: 1);

            var vodUrl = string.Empty;
            EmbedBuilder embed = null;

            if (vodList != null && vodList.Videos.Length != 0)
            {
                var vod = vodList.Videos.First();
                vodUrl = $"https://www.twitch.tv/videos/{vod.Id}";

                var unixTimestamp = DateTime.Parse(vod.CreatedAt).ToUniversalTime().GetTimestamp();

                embed = new EmbedBuilder
                {
                    Color = Color.Red,
                    Title = vod.Title,
                    Url = vodUrl.Replace("<", "").Replace(">", ""),
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
                            Value = "Destiny 2",
                            IsInline = true
                        }
                    }
                };
            }

            var streamsToRemove = new List<ActiveStream>();

            if (!string.IsNullOrEmpty(vodUrl))
                vodUrl = $" <{vodUrl}>";

            foreach (var activeStream in activeStreams)
            {
                var message = await ((SocketTextChannel)Context.Client.GetChannel(
                        _twitchStreamDb.TwitchStreams.FirstOrDefault(x => x.Id == activeStream.ConfigId)!.ChannelId))
                    .GetMessageAsync(activeStream.MessageId);

                (message as IUserMessage)?.ModifyAsync(delegate(MessageProperties properties)
                {
                    properties.Content = $"{Format.Bold(channelInfo?.DisplayName)} was live:{vodUrl}";
                    properties.Embed = embed?.Build();
                });
                
                streamsToRemove.Add(activeStream);
            }

            _twitchStreamDb.ActiveStreams.RemoveRange(streamsToRemove);
            await _twitchStreamDb.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "OnStreamOffline");
        }
    }
}