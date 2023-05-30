using Discord;
using Discord.Interactions;
using Felicity.Util;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

public class SupportCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("support", "Get helpful links for the Felicity project.")]
    public async Task Support()
    {
        await DeferAsync();

        var embed = Embeds.MakeBuilder();
        embed.Title = "Thank you for your interest in Felicity.";
        embed.Color = Color.Green;
        embed.ThumbnailUrl = BotVariables.Images.FelicitySquare;
        embed.Description = Format.Bold("--- Useful links:") +
                            $"\n<:discord:994211332301791283> [Support Server]({BotVariables.DiscordInvite})" +
                            "\n<:twitter:994216171110932510> [Twitter](https://twitter.com/devFelicity)" +
                            "\n🌐 [Website](https://tryfelicity.one)" +
                            "\n\n" + Format.Bold("--- Contribute to upkeep:") +
                            "\n• <:kofi:994212063041835098> Donate one-time or monthly on [Ko-Fi](https://ko-fi.com/axsleaf)" +
                            "\n• <:streamelements:994215192554635285> Donate any amount through [StreamElements](https://streamelements.com/blossomleafy/tip)" +
                            "\n• <:paypal:994215375141097493> Donate any amount through [PayPal](https://paypal.me/leafyleaf)" +
                            "\n• <:github:994212386204549160> Become a sponsor on [GitHub](https://github.com/sponsors/MoonieGZ)" +
                            "\n• <:twitch:994214014055895040> Subscribe on [Twitch](https://twitch.tv/subs/MoonieGZ) *(free once per month with Amazon Prime)*";

        await FollowupAsync(embed: embed.Build());
    }
}