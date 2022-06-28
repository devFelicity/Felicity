using Discord;
// ReSharper disable StringLiteralTypo

namespace Felicity.Util;

public static class Embeds
{
    public static EmbedBuilder MakeBuilder()
    {
        var builder = new EmbedBuilder
        {
            Color = Color.Orange,
            Footer = MakeFooter()
        };

        return builder;
    }

    public static EmbedFooterBuilder MakeFooter()
    {
        return new EmbedFooterBuilder
        {
            Text = $"Felicity v.{BotVariables.Version} | tryfelicity.one",
            IconUrl = "https://cdn.tryfelicity.one/images/felicity_circle.jpg"
        };
    }
}