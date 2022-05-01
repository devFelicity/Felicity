using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Felicity.Helpers;

public class RequireOAuthPrecondition : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var oauth = (context.User as SocketUser).OAuth();

        if (oauth != null) return Task.FromResult(PreconditionResult.FromSuccess());

        const string msg = "This command requires you to be registered to provide user information to the API.\nPlease /register and try again.";
        context.Interaction.RespondAsync(msg);

        return Task.FromResult(PreconditionResult.FromError(msg));
    }
}