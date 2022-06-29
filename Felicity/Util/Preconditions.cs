using Discord;
using Discord.Interactions;
using Felicity.Models;

namespace Felicity.Util;

// ReSharper disable once ClassNeverInstantiated.Global
public class Preconditions
{
    public class RequireBotModerator : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.User.Id == BotVariables.BotOwnerId)
                return Task.FromResult(PreconditionResult.FromSuccess());

            if (((IGuildUser)context.User).GuildPermissions.ManageGuild)
                return Task.FromResult(PreconditionResult.FromSuccess());

            if (context.Guild.OwnerId == context.User.Id)
                return Task.FromResult(PreconditionResult.FromSuccess());

            const string msg =
                "You are not a bot moderator for this server, your server owner should not have this command enabled for roles other than the designated moderator role.";

            return Task.FromResult(PreconditionResult.FromError(msg));
        }
    }

    public class RequireOAuth : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            var dbSet = services.GetService<UserDb>()?.Users;
            var user = dbSet?.FirstOrDefault(x => x.DiscordId == context.User.Id);

            if (user != null) return Task.FromResult(PreconditionResult.FromSuccess());

            const string msg =
                "This command requires you to be registered to provide user information to the API.\nPlease `/user register` and try again.";
            context.Interaction.RespondAsync(msg);

            return Task.FromResult(PreconditionResult.FromError(msg));
        }
    }
}