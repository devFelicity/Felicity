using Discord;
using Discord.Interactions;
using Felicity.Models;

// ReSharper disable NotAccessedField.Local
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

[RequireBotModerator]
public class StaffCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly UserDb _userDb;
    private readonly ServerDb _serverDb;
    private readonly TwitchStreamDb _streamDb;

    public StaffCommands(UserDb userContext, TwitchStreamDb streamDb, ServerDb serverDb)
    {
        _userDb = userContext;
        _streamDb = streamDb;
        _serverDb = serverDb;
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
    }
}

public class RequireBotModerator : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo commandInfo, IServiceProvider services)
    {
        if(context.User.Id == 684854397871849482)
            return Task.FromResult(PreconditionResult.FromSuccess());

        if (((IGuildUser) context.User).GuildPermissions.ManageGuild)
            return Task.FromResult(PreconditionResult.FromSuccess());

        var guildUser = context.Guild.GetUserAsync(context.User.Id).Result;

        if (context.Guild.OwnerId == guildUser.Id)
            return Task.FromResult(PreconditionResult.FromSuccess());

        const string msg =
            "You are not a bot moderator for this server, your server owner should not have this command enabled for roles other than the designated moderator role.";

        return Task.FromResult(PreconditionResult.FromError(msg));
    }
}