using Discord;
using Discord.Interactions;
using Felicity.Models;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

[Group("user", "Manage user settings for the bot.")]
public class UserCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly UserDb _userDb;

    public UserCommands(UserDb userDb)
    {
        _userDb = userDb;
    }

    [SlashCommand("register", "Register your bungie profile to the bot.")]
    public async Task UserRegister()
    {
        await DeferAsync(true);

        var embed = Embeds.MakeBuilder();
        embed.Description = "Use the link below to register your Bungie profile with Felicity.\n"
        + "We securely store authentication keys to access your profile information, collections, records, and more.\n"
        + "**If you don't want us to store your data, please refrain from proceeding.**\n\n"
        + $"[Click here to register.](https://auth.tryfelicity.one:8082/auth/bungie_net/{Context.User.Id})";

        await FollowupAsync(embed: embed.Build(), ephemeral: true);
    }

    [SlashCommand("remove", "Remove your profile link from the bot.")]
    public async Task UserRemove()
    {
        await DeferAsync(true);

        var embed = Embeds.MakeBuilder();

        var user = _userDb.Users.FirstOrDefault(x => x.DiscordId == Context.User.Id);

        if (user != null)
        {
            _userDb.Users.Remove(user);

            embed.Color = Color.Green;
            embed.Description = "Your profile has been removed from Felicity.";

            await _userDb.SaveChangesAsync();
        }
        else
        {
            embed.Color = Color.Red;
            embed.Description = "You are not currently registered with Felicity.";
        }

        await FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}