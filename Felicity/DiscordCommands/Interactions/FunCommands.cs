using Discord;
using Discord.Interactions;
using Felicity.Util;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

public class FunCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("fristy", "Have you ever heard of the cute FristyFox?")]
    public async Task FunFristy()
    {
        await DeferAsync();

        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            IconUrl = "https://cdn.discordapp.com/avatars/139428768669237248/0fe07c14088b6c5f222283f44f34b09b.png",
            Name = "<= this is FristyFox",
            Url = "https://twitch.tv/fristyfox"
        };
        embed.Description = "FristyFox of the House Mittens, the first of His Name, King of the Pyramids, " +
                            "and the first Foxes, Lord of the seven discords and the Protector of the Chat, " +
                            "Lord of Foxstone, King of Snaccville, Khal of the Great Hangout Sea, the Unburnt, " +
                            "Breaker of Bongobutt Chains, and Father of Cuties <:mittensAWW:594996439508320297> Cutest there is, " +
                            "Cutest there was, and the Cutest there ever will be!!! NO TAKE BACKS! <:mittensAWW:594996439508320297>";

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("jess", "Have you ever heard of the cute Jess?")]
    public async Task FunJess()
    {
        await DeferAsync();

        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            IconUrl = "https://cdn.discordapp.com/avatars/894420816542974024/f8a8b6f2cc0619fb01962c9ea225e6ac.png",
            Name = "<= this is Jess",
            Url = "https://twitch.tv/tattmeupjess"
        };
        embed.Description = "better than a blueberry, is an apple <a:catapplerun:968303617402630184>";

        await FollowupAsync(embed: embed.Build());
    }
}