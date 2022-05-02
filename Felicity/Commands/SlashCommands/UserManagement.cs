using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Interactions;
using Felicity.Services;
using static Felicity.Services.OAuthService;

// ReSharper disable UnusedType.Global
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
                    "Please visit this link to complete your registration, the bot will DM you when your registration is complete.\n\n" +
                    APIService.GetApiClient().OAuth.GetOAuthAuthorizationUrl(newUser.State));
                break;
            case UserLinkStatus.Incomplete:
                var user = GetUser(Context.User.Id).Result;
                await FollowupAsync(
                    "You have an incomplete registration in progress, please visit this link to complete your registration:\n\n" + 
                    APIService.GetApiClient().OAuth.GetOAuthAuthorizationUrl(user.State));
                break;
            case UserLinkStatus.Registered:
                await FollowupAsync("You are already registered.", ephemeral: true);
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [SlashCommand("unregister", "Remove your profile link from the bot.")]
    public async Task Unregister()
    {
        await DeferAsync(true);

        var path = $"Users/{Context.User.Id}.json";

        if (File.Exists(path))
        {
            File.Delete(path);
            await FollowupAsync("Deleted profile successfully.");
        }
        else
        {
            await FollowupAsync("You are not registered.");
        }
    }
}