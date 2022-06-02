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

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static Task HandleVC(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
    {
        return Task.CompletedTask;
    }

    public static Task HandleJoinedGuild(SocketGuild arg)
    {
        var config = ConfigService.GetBotSettings();
        var banned = config.BannedUsers.Any(bannedUser => bannedUser.Id == arg.Id);

        var serverInvites = arg.GetInvitesAsync().Result;
        var invite = serverInvites.Count != 0 ? serverInvites.First().Url : arg.GetVanityInviteAsync().Result.Url;

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = arg.Name,
                IconUrl = arg.IconUrl
            },
            Title = "Felicity was added to a server.",
            Url = invite,
            ThumbnailUrl = arg.IconUrl,
            Description = arg.Description,
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
                    Name = "Members",
                    Value = arg.Users.Count,
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

        if (banned) arg.LeaveAsync();

        LogService.DiscordLogChannel.SendMessageAsync(embed: embed.Build());

        return Task.CompletedTask;
    }

    public static Task HandleLeftGuild(SocketGuild arg)
    {
        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = arg.Name,
                IconUrl = arg.IconUrl
            },
            Title = "Felicity was removed from a server.",
            ThumbnailUrl = arg.IconUrl,
            Description = arg.Description,
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
                }
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
}