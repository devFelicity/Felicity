using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using FelicityOne.Caches;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using FelicityOne.Services;

namespace FelicityOne.Events;

internal static class DiscordEvents
{
    public static Task HandleMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2,
        ISocketMessageChannel arg3)
    {
        if (arg3.Id == ConfigService.GetBotSettings().CheckpointChannel)
            ProcessCPData.Populate(arg2);

        return Task.CompletedTask;
    }

    public static async Task HandleJoin(SocketGuildUser arg)
    {
        var serverSettings = ConfigService.GetServerSettings(arg.Guild.Id);
        if (serverSettings != null)
            if (serverSettings.MemberEvents.MemberJoined)
                await arg.Guild.GetTextChannel(serverSettings.MemberEvents.LogChannel).SendMessageAsync(
                    embed: Extensions.GenerateUserEmbed(arg).Build());
    }

    public static async Task HandleLeft(SocketGuild arg1, SocketUser arg2)
    {
        var serverSettings = ConfigService.GetServerSettings(arg1.Id);
        if (serverSettings != null)
            if (serverSettings.MemberEvents.MemberLeft)
                await arg1.GetTextChannel(serverSettings.MemberEvents.LogChannel).SendMessageAsync(
                    embed: Extensions.GenerateUserEmbed(arg2).Build());
    }

    public static Task HandleJoinedGuild(SocketGuild arg)
    {
        var config = ConfigService.GetBotSettings();
        var banned = config.BannedUsers.Any(bannedUser => bannedUser.Id == arg.Id);

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = "Felicity was added to a server."
            },
            Color = ConfigService.GetEmbedColor(),
            Title = arg.Name,
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new()
                {
                    Name = "Owner",
                    Value = arg.Owner,
                    IsInline = true
                },
                new()
                {
                    Name = "Banned?",
                    Value = banned,
                    IsInline = true
                }
            }
        };

        if (arg.IconUrl != null)
            embed.ThumbnailUrl = arg.IconUrl;

        if (arg.Description != null)
            embed.Description = arg.Description;

        if (banned)
            arg.LeaveAsync();

        LogService.DiscordLogChannel.SendMessageAsync(embed: embed.Build());

        return Task.CompletedTask;
    }

    public static Task HandleLeftGuild(SocketGuild arg)
    {
        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = "Felicity was removed from a server."
            },
            Color = ConfigService.GetEmbedColor(),
            Title = arg.Name,
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            }
        };

        LogService.DiscordLogChannel.SendMessageAsync(embed: embed.Build());

        return Task.CompletedTask;
    }

    public static Task HandleInviteCreated(SocketInvite arg)
    {
        Console.WriteLine($"{arg.Inviter} created invite {Format.Code(arg.Code)} in {arg.Guild.Name}");
        return Task.CompletedTask;
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static Task HandlePresenceUpdated(SocketUser arg1, SocketPresence arg2, SocketPresence arg3)
    {
        return Task.CompletedTask;
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static Task HandleVC(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
    {
        return Task.CompletedTask;
    }
}