using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.TestCommands;

public class GoACommands : ModuleBase<SocketCommandContext>
{
    [Command("farmer")]
    public async Task FarmerRole()
    {
        if (!Context.Guild.Id.Equals(965739860033941534) || !Context.Channel.Id.Equals(965752962498584688))
            return;

        var hasRole = false;

        if (Context.User is SocketGuildUser user)
        {
            if (user.Roles.Any(role => role.Id.Equals(965763469204926504)))
                hasRole = true;

            if (hasRole)
            {
                await Context.Guild.GetUser(Context.User.Id).RemoveRoleAsync(965763469204926504);
                await Context.Message.ReplyAsync($"Removed farmer role from <@{user.Id}>");
            }
            else
            {
                await Context.Guild.GetUser(Context.User.Id).AddRoleAsync(965763469204926504);
                await Context.Message.ReplyAsync($"Gave farmer role to <@{user.Id}>");
            }
        }
    }
}