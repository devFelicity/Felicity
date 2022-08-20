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
        embed.Description = "The link below will allow you to register your Bungie profile to Felicity.\n\n" +
                            "We store authentication keys to fetch information about your profile, collections, records and more." +
                            "If you do not consent to us storing your data, please do not continue.\n\n" +
                            $"[Click here to register.](https://api.tryfelicity.one:8082/auth/bungie_net/{Context.User.Id})";

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

        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}