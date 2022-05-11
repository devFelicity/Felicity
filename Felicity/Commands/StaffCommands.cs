using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Felicity.Helpers;
using Felicity.Services;
using Serilog;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands;

public class StaffCommands : ModuleBase<SocketCommandContext>
{
    [Command("listServers")]
    public async Task List()
    {
        var serverList = Context.Client.Guilds.OrderBy(x => x.Name);
        var msg = serverList.Aggregate(string.Empty,
            (current, guild) =>
                current + $"- {Format.Bold(guild.Name)} ({guild.Id}) [{Format.Italics(guild.Owner.ToString())}]\n");

        await ReplyAsync(msg);
    }

    [Command("spam")]
    public async Task Spam(IUser user, int times)
    {
        for (var i = 0; i < times; i++)
        {
            await ReplyAsync(user.Mention);
        }
    }

    [Command("leaveServer")]
    public async Task LeaveServer(ulong serverId)
    {
        var server = Context.Client.GetGuild(serverId);

        if (server == null)
        {
            await ReplyAsync("Server not found.");
            return;
        }

        await server.LeaveAsync();
        await ReplyAsync($"Left server `{server.Name}`");
    }

    [Command("ban")]
    public async Task Ban(ulong userId, string reason)
    {
        try
        {
            await Context.Guild.AddBanAsync(userId, 0, reason, RequestOptions.Default);
            await ReplyAsync($"Successfully banned user ID {userId}.");
        }
        catch (Exception ex)
        {
            await ReplyAsync($"Failed to apply ban: {ex.GetType()} - {ex.Message}");
        }
    }

    [Command("staff")]
    public async Task Staff()
    {
        await ReplyAsync(Context.User.IsStaff().ToString());
    }

    [Command("restartTwitch")]
    public async Task RestartTwitch()
    {
        if (!Context.User.IsStaff())
            return;

        try
        {
            TwitchService.RestartMonitor();
            await ReplyAsync("Restarted TwitchMonitor");
        }
        catch (Exception ex)
        {
            var log = $"{ex.GetType()}: {ex.Message}";
            Log.Error(log);
            await ReplyAsync(log);
        }
    }
}