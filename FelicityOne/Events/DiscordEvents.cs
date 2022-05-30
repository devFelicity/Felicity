using Discord;
using Discord.WebSocket;
using FelicityOne.Caches;
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

    public static Task HandleLeft(SocketGuild arg1, SocketUser arg2)
    {
        throw new NotImplementedException();
    }

    public static Task HandleVC(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
    {
        throw new NotImplementedException();
    }

    public static Task HandleJoinedGuild(SocketGuild arg)
    {
        throw new NotImplementedException();
    }

    public static Task HandleLeftGuild(SocketGuild arg)
    {
        throw new NotImplementedException();
    }

    public static Task HandleInviteCreated(SocketInvite arg)
    {
        Console.WriteLine($"{arg.Inviter} created invite {Format.Code(arg.Code)} in {arg.Guild.Name}");
        return Task.CompletedTask;
    }
}