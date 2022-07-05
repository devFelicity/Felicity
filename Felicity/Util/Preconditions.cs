using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Clients;
using Felicity.Models;
using Serilog;

namespace Felicity.Util;

// ReSharper disable once ClassNeverInstantiated.Global
public class Preconditions
{
    public class RequireBotModerator : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.User.Id == BotVariables.BotOwnerId)
                return PreconditionResult.FromSuccess();

            if (((IGuildUser)context.User).GuildPermissions.ManageGuild)
                return PreconditionResult.FromSuccess();

            if (context.Guild.OwnerId == context.User.Id)
                return PreconditionResult.FromSuccess();

            const string msg =
                "You are not a bot moderator for this server.";

            await context.Interaction.RespondAsync(msg);

            return PreconditionResult.FromError(msg);
        }
    }

    public class RequireOAuth : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            await context.Interaction.DeferAsync();

            var dbSet = services.GetService<UserDb>();
            var user = dbSet?.Users.FirstOrDefault(x => x.DiscordId == context.User.Id);
            var nowTime = DateTime.UtcNow;
            string msg;

            if (user != null)
            {
                if (user.OAuthRefreshExpires < nowTime)
                {
                    msg =
                        "Your information has expired and needs to be refreshed.\n" +
                        "Please run `/user register` and follow the instructions.";

                    await context.Interaction.FollowupAsync(msg);

                    return PreconditionResult.FromError(msg);
                }

                // ReSharper disable once InvertIf
                if (user.OAuthTokenExpires < nowTime)
                {
                    user = await user.RefreshToken(services.GetService<IBungieClient>()!, nowTime);

                    Log.Information($"Refreshed token for {user.BungieName}.");

                    await dbSet?.SaveChangesAsync()!;
                }

                return PreconditionResult.FromSuccess();
            }

            msg = "This command requires you to be registered to provide user information to the API.\n" +
                  "Please use `/user register` and try again.";
            await context.Interaction.FollowupAsync(msg);

            return PreconditionResult.FromError(msg);
        }
    }
}