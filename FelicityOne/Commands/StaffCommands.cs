using Discord;
using Discord.Commands;
using FelicityOne.Caches;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands;

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

    [Command("fillCPs")]
    public Task FillCPs(ulong messageId)
    {
        var msg = Context.Channel.GetMessageAsync(messageId).Result;
        ProcessCPData.Populate(msg);
        return Task.CompletedTask;
    }
}