using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ceen;
using Discord;
using Discord.Commands;
using Felicity.Configs;
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands;

public class StaffCommands : ModuleBase<SocketCommandContext>
{
    [Command("listServers")]
    public async Task List()
    {
        var msg = Context.Client.Guilds.Aggregate(string.Empty,
            (current, guild) => current + $"Name: {guild.Name} // ID: {guild.Id}\n");

        await ReplyAsync(msg);
    }

    [Command("addServer")]
    public async Task AddServer(ulong serverId)
    {
        var currentConfig = ServerConfig.FromJson();

        currentConfig.Settings ??= new Dictionary<string, ServerSetting>();

        currentConfig.Settings.Add(serverId.ToString(), new ServerSetting());
        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(currentConfig));

        await ReplyAsync($"Wrote new server \"{serverId}\"");
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
            await Log.ErrorAsync(log);
            await ReplyAsync(log);
        }
    }
}