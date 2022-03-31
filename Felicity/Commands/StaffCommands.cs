using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Felicity.Helpers;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands;

public class StaffCommands : ModuleBase<SocketCommandContext>
{
    [Command("listServers")]
    public async Task List()
    {
        var msg = Context.Client.Guilds.Aggregate(string.Empty, (current, guild) => current + $"Name: {guild.Name} // ID: {guild.Id}\n");

        await ReplyAsync(msg);
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
}