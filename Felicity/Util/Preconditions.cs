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
                "You are not a bot moderator for this server.";

            return Task.FromResult(PreconditionResult.FromError(msg));
        }
    }

    public class RequireOAuth : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
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

                    await context.Interaction.RespondAsync(msg);

                    return PreconditionResult.FromError(msg);
                }

                // ReSharper disable once InvertIf
                if (user.OAuthTokenExpires < nowTime)
                {
                    await context.Interaction.DeferAsync();

                    user = await user.RefreshToken(services.GetService<IBungieClient>()!, nowTime);

                    Log.Information($"Refreshed token for {user.BungieName}.");

                    await dbSet?.SaveChangesAsync()!;
                }

                return PreconditionResult.FromSuccess();
            }

            msg = "This command requires you to be registered to provide user information to the API.\n" +
                  "Please user `/user register` and try again.";
            await context.Interaction.RespondAsync(msg);

            return PreconditionResult.FromError(msg);
        }
    }
}