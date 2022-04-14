using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Felicity.Helpers;

using static Felicity.Services.OAuthService;

// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.SlashCommands;

public class UserManagement : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("register", "Register your bungie profile to the bot.")]
    public async Task Register()
    {
        await DeferAsync(true);

        var registered = UserIsLinked(Context.User.Id);
        switch (registered)
        {
            case UserLinkStatus.NotRegistered:
                var newUser = CreateUser(Context.User.Id);
                await FollowupAsync(
                    "Please visit this link to complete your registration, the bot will DM you when your registration is complete.\n\n"+
                    $"https://www.bungie.net/en/oauth/authorize?client_id={ConfigHelper.GetBotSettings().BungieClientId}&response_type=code&state={newUser.State}");
                break;
            case UserLinkStatus.Incomplete:
                var user = GetUser(Context.User.Id);
                await FollowupAsync(
                    "You have an incomplete registration in progress, please visit this link to complete your registration:\n\n"
                    + $"https://www.bungie.net/en/oauth/authorize?client_id={ConfigHelper.GetBotSettings().BungieClientId}&response_type=code&state={user.State}");
                break;
            case UserLinkStatus.Registered:
                await FollowupAsync("You are already registered.", ephemeral: true);
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}