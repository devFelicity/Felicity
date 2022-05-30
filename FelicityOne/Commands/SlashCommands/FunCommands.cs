using Discord;
using Discord.Interactions;
using FelicityOne.Enums;
using FelicityOne.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands.SlashCommands;

public class FunCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("fristy", "Have you ever heard of the cute FristyFox?")]
    public async Task Fristy()
    {
        await DeferAsync();

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                IconUrl = "https://cdn.discordapp.com/avatars/139428768669237248/0fe07c14088b6c5f222283f44f34b09b.png",
                Name = "<= this is FristyFox",
                Url = "https://twitch.tv/fristyfox"
            },
            Description = "FristyFox of the House Mittens, the first of His Name, King of the Pyramids, " +
                          "and the first Foxes, Lord of the seven discords and the Protector of the Chat, " +
                          "Lord of Foxstone, King of Snaccville, Khal of the Great Hangout Sea, the Unburnt, " +
                          "Breaker of Bongobutt Chains, and Father of Cuties <:mittensAWW:594996439508320297> Cutest there is, " +
                          "Cutest there was, and the Cutest there ever will be!!! NO TAKE BACKS! <:mittensAWW:594996439508320297>",
            Color = ConfigService.GetEmbedColor(),
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            }
        };

        await FollowupAsync(embed: embed.Build());
    }
}