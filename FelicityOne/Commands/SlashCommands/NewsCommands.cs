using Discord;
using Discord.Interactions;
using FelicityOne.Helpers;
using FelicityOne.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands.SlashCommands;

public class NewsCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("twab", "Where TWAB?")]
    public async Task Twab()
    {
        await DeferAsync();

        var lg = Context.Language().ToString().ToLower();

        var twab = BungieAPI.GetApiClient().Api
            .Content_GetContentByTagAndType(lg, "twab", "news").Result;

        var url = $"https://www.bungie.net/{lg}/Explore/Detail/News/{twab.ContentId}";

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = twab.Author.DisplayName,
                IconUrl = BungieAPI.BaseUrl + twab.Author.ProfilePicturePath
            },
            Color = ConfigService.GetEmbedColor(),
            Description = twab.Properties["Subtitle"].ToString(),
            Fields = new List<EmbedFieldBuilder>
            {
                new()
                {
                    Name = "Created:",
                    IsInline = true,
                    Value = $"<t:{twab.CreationDate.GetTimestamp()}>"
                },
                new()
                {
                    Name = "Modified:",
                    IsInline = true,
                    Value = $"<t:{twab.ModifyDate.GetTimestamp()}>"
                }
            },
            Footer = Extensions.GenerateEmbedFooter(),
            ImageUrl = BungieAPI.BaseUrl + twab.Properties["ArticleBanner"],
            Title = twab.Properties["Title"].ToString(),
            Url = url
        };

        await FollowupAsync(embed: embed.Build());
    }
}