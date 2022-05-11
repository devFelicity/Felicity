using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Felicity.Services;

internal class TmpVCService
{
    public static void GoAFarmers(DiscordSocketClient client, SocketUser user,
        SocketVoiceState state)
    {
        if (state.VoiceChannel != null)
        {
            if (state.VoiceChannel.CategoryId != 965739860033941536) return;
        }

        if (state.VoiceChannel is {Name: "VC Hub"})
        {
            var chan = client.GetGuild(965739860033941534).CreateVoiceChannelAsync($"{user.Username}'s Farm", properties =>
            {
                properties.CategoryId = 965739860033941536;
                properties.UserLimit = 3;
            }).Result;
            (user as SocketGuildUser)?.ModifyAsync(x => x.ChannelId = chan.Id);
            chan.AddPermissionOverwriteAsync(user,
                new OverwritePermissions(manageChannel: PermValue.Allow, moveMembers: PermValue.Allow));
        }

        foreach (var emptyChan in client.GetGuild(965739860033941534).VoiceChannels)
        {
            if (emptyChan.Name.Equals("VC Hub")) continue;

            if (emptyChan.Users.Count != 0) continue;

            emptyChan.DeleteAsync();
        }
    }
}