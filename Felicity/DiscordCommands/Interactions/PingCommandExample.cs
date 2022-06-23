using Discord;
using Discord.Interactions;
using Discord.WebSocket;

// ReSharper disable EmptyConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

public class PingCommandExample : InteractionModuleBase<ShardedInteractionContext<SocketSlashCommand>>
{
    public PingCommandExample()
    {
    }

    [SlashCommand("ping", "Pongs back")]
    public async Task PongAsync(
        [Summary("text to pong back")] string? text = null)
    {
        var eb = new EmbedBuilder()
            .WithTitle("Glorious embed title")
            .WithDescription("Description? Idk what you expected in ping command")
            .AddField("PONG", "PONG");
        
        if (text is not null && text != "")
        {
            eb.AddField("Some custom text", text);
        }

        await Context.Interaction.RespondAsync(embed: eb.Build(), ephemeral: true);
    }
}