using Discord.Commands;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Text;

public class BasicTextCommands : ModuleBase<ShardedCommandContext>
{
    [Command("ping")]
    public async Task Pong()
    {
        await ReplyAsync("<:NOOOOOOOOOOOOOT:855149582177533983>");
    }
}