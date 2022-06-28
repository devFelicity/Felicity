using Discord.Commands;
using Discord.WebSocket;
using DotNetBungieAPI.Models;
using Felicity.DbObjects;

namespace Felicity.DiscordCommands.Text;

public class TestingTextCommands : ModuleBase<ShardedCommandContext>
{
    private readonly ServerDb _serverDb;
    private readonly TwitchStreamDb _streamDb;

    public TestingTextCommands(ServerDb serverDb, TwitchStreamDb streamDb)
    {
        _serverDb = serverDb;
        _streamDb = streamDb;
    }

    [Command("addServer")]
    public async Task AddServer(ulong serverId)
    {
        if (!_serverDb.Servers.Any(x => x.ServerId == serverId))
        {
            _serverDb.Servers.Add(new Server
            {
                ServerId = serverId,
                BungieLocale = BungieLocales.EN
            });

            await _serverDb.SaveChangesAsync();

            await ReplyAsync($"Added server {serverId}");
        }
        else
        {
            await ReplyAsync("Server already exists in db.");
        }
    }

    [Command("editServer"), RequireContext(ContextType.Guild)]
    public async Task EditServer(ulong modRole)
    {
        var server = _serverDb.Servers.FirstOrDefault(x => x.ServerId == Context.Guild.Id);
        if (server != null)
        {
            if ((Context.User as SocketGuildUser)!.GuildPermissions.ManageGuild)
            {
                await _serverDb.SaveChangesAsync();
                await ReplyAsync("Edited server successfully.");
            }
        }
        else
        {
            await ReplyAsync("Server does not exist.");
        }
    }

    [Command("addStream")]
    public async Task AddStream(string twitchName, ulong serverId, ulong channelId)
    {
        _streamDb.TwitchStreams.Add(new TwitchStream
        {
            TwitchName = twitchName,
            ServerId = serverId,
            ChannelId = channelId
        });

        await _streamDb.SaveChangesAsync();

        await ReplyAsync("Added stream.");
    }
}