using Discord.Interactions;
using Felicity.Util;

// ReSharper disable NotAccessedField.Local
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireBotModerator]
public class StaffCommands : InteractionModuleBase<ShardedInteractionContext>
{
    /*private readonly UserDb _userDb;
    private readonly ServerDb _serverDb;
    private readonly TwitchStreamDb _streamDb;
    private readonly IBungieClient _bungieClient;

    public StaffCommands(UserDb userContext, TwitchStreamDb streamDb, ServerDb serverDb, IBungieClient bungieClient)
    {
        _userDb = userContext;
        _streamDb = streamDb;
        _serverDb = serverDb;
        _bungieClient = bungieClient;
    }

    [SlashCommand("mod_refresh", "Forcefully refreshes all users tokens.")]
    public async Task ForceRefresh()
    {
        await RespondAsync("Triggering refresh...");
        await BungieApiUtils.ForceRefresh(_bungieClient, _userDb);
    }

    [SlashCommand("ping", "Pongs back")]
    public async Task PongAsync(
        [Summary("text", "text to pong back")] string text = "")
    {
        var eb = new EmbedBuilder()
            .WithTitle("Glorious embed title")
            .WithDescription("Description? Idk what you expected in ping command")
            .AddField("PONG", "PONG");
        
        if (string.IsNullOrEmpty(text))
        {
            eb.AddField("Some custom text", text);
        }

        await RespondAsync(embed: eb.Build(), ephemeral: true);
    }*/
}