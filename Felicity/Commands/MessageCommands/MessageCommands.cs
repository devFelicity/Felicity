using Discord;
using Felicity.Enums;
using Felicity.Helpers;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.MessageCommands;

public static class MessageCommands
{
    public static Embed ResourceHub()
    {
        var embed = new EmbedBuilder
        {
            Title = "Destiny 2 Resource Hub",
            Color = ConfigHelper.GetEmbedColor(),
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            },
            Description = "**Braytech**\nFor character progression, clans, triumphs, collections, and more: [here.](https://bray.tech)\n\n"+
                          "**Destiny Recipes**\nPre-seasonal checklist, Vault Cleaner, Power Checker and Loot Companion: [here.](https://destinyrecipes.com/)\n\n"+
                          "**Destiny Item Manager**\nSwap items, check stats, and build loadouts: [here.](https://destinyitemmanager.com/)\n\n"+
                          "**.Report Sites**\nGet various statistics from your activity history: [raids](https://raid.report/), " +
                          "[nightfalls](https://nightfall.report/), [trials](https://trials.report/), [guardian](https://guardian.report/)."
        };

        return embed.Build();
    }
}