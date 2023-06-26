using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Serilog;

namespace Felicity.Util;

// ReSharper disable once ClassNeverInstantiated.Global
public class Preconditions
{
    public class RequireBotModerator : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(
            IInteractionContext context,
            ICommandInfo commandInfo, 
            IServiceProvider services)
        {
            if (context.User.Id == BotVariables.BotOwnerId)
                return PreconditionResult.FromSuccess();

            if (context.User is IGuildUser guildUser && guildUser.GuildPermissions.ManageGuild)
                return PreconditionResult.FromSuccess();

            if (context.Guild.OwnerId == context.User.Id)
                return PreconditionResult.FromSuccess();

            const string msg = "You are not a bot moderator for this server.";

            await context.Interaction.RespondAsync(msg);

            return PreconditionResult.FromError(msg);
        }
    }

    public class RequireOAuth : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(
            IInteractionContext context,
            ICommandInfo commandInfo, 
            IServiceProvider services)
        {
            await context.Interaction.DeferAsync();

            var dbSet = services.GetService<UserDb>();
            var user = dbSet?.Users.FirstOrDefault(x => x.DiscordId == context.User.Id);
            var nowTime = DateTime.UtcNow;

            var errorEmbed = Embeds.MakeErrorEmbed();

            if (user is not null)
            {
                if (user.OAuthRefreshExpires < nowTime)
                    errorEmbed.Description =
                        "Your information has expired and needs to be refreshed.\n" +
                        "Please run `/user register` and follow the instructions.";

                if (user.OAuthTokenExpires < nowTime)
                {
                    user = await user.RefreshToken(services.GetService<IBungieClient>()!, nowTime);

                    Log.Information($"Refreshed token for {user.BungieName}.");

                    await dbSet?.SaveChangesAsync()!;
                }

                if (user.BungieMembershipId == 0)
                    errorEmbed.Description =
                        $"Your registration data is invalid, please run `/user register` again.\nIf the issue persists, {BotVariables.ErrorMessage}";
            }
            else
            {
                errorEmbed.Description =
                    "This command requires you to be registered to provide user information to the API.\n" +
                    "Please use `/user register` and try again.";
            }

            if (string.IsNullOrEmpty(errorEmbed.Description))
                return PreconditionResult.FromSuccess();

            await context.Interaction.FollowupAsync(embed: errorEmbed.Build());
            return PreconditionResult.FromError(errorEmbed.Description);
        }
    }
}