using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Felicity.Helpers;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Felicity.Services;

internal class TwitchService
{
    public static TwitchAPI Api;
    public static DiscordSocketClient Client;

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

        var currentUser = ConfigHelper.GetTwitchSettings().Users
            .Where(user => user.Value.Name.ToLower().Equals(myStream.UserName.ToLower())).Select(user => user.Value)
            .FirstOrDefault();

        try
        {
            if (currentUser == null)
                return;

            var mention = "";
            if (currentUser.MentionEveryone)
                mention = "@everyone ";
            else if (currentUser.Mention != 0)
                mention = $"<@&{currentUser.Mention}> ";

            await Client.GetGuild(currentUser.ServerId).GetTextChannel(currentUser.ChannelId)
                .SendMessageAsync(
                    $"<@{currentUser.UserId}> is now live: <https://twitch.tv/{myStream.UserName}>\n{mention}",
                    false, embed.Build());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}