using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Felicity.Helpers;

public class RequireOAuthPrecondition : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if ((context.User as SocketUser).OAuth() == null)
        {
            return Task.FromResult(PreconditionResult.FromError(
                "This command requires you to be registered to provide user information to the API. Please /register and try again."));
        }

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}