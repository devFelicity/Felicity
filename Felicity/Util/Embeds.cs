using Discord;
using Discord.WebSocket;

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

    public static EmbedBuilder GenerateGuildUser(SocketUser socketUser)
    {
        var embed = MakeBuilder();

        embed.Author = new EmbedAuthorBuilder
        {
            IconUrl = socketUser.GetAvatarUrl(),
            Name = socketUser.Username
        };
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                IsInline = true,
                Name = "Account Created",
                Value = socketUser.CreatedAt.ToString("d")
            },
            new()
            {
                IsInline = true,
                Name = "User ID",
                Value = socketUser.Id
            }
        };
        
        return embed;
    }
}