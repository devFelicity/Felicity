using System.Text;
using BungieSharper.Entities.Content;
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
    public async Task Twab(
        [Summary("query", "Search TWABs for a phrase or word. If null, latest TWAB will be returned.")]
        string search =
            "")
    {
        await DeferAsync();

        const string lg = "en";
        // var lg = Context.Language().ToString().ToLower();

        if (!string.IsNullOrEmpty(search))
        {
            var searchEmbed = new EmbedBuilder
            {
                Title = $"TWAB search for `{search}`.",
                Color = ConfigService.GetEmbedColor(),
                Footer = Extensions.GenerateEmbedFooter()
            };

            var results = SearchTWAB(search);
            if (results.Count != 0)
                searchEmbed.Fields = results;

            await FollowupAsync(embed: searchEmbed.Build());

            return;
        }

        var twab = BungieAPI.GetApiClient().Api
            .Content_SearchContentWithText(lg, "news", searchtext: "this week at bungie")
            .Result.Results.First();

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

    private static List<EmbedFieldBuilder> SearchTWAB(string search)
    {
        var twabList = new List<ContentItemPublicContract>();

        twabList.AddRange(FillTwabList("this week at bungie"));
        twabList.AddRange(FillTwabList("bungie weekly update"));

        var foundTwabs = twabList.Where(twabLink =>
            twabLink.Properties["Content"].ToString()!.ToLower().Contains(search.ToLower()));

        var sb = new StringBuilder();
        var counter = 0;

        var embedFields = new List<EmbedFieldBuilder>();

        foreach (var result in foundTwabs)
        {
            counter++;
            var currentLength = sb.Length;

            var title = string.IsNullOrEmpty(result.Properties["Subtitle"].ToString())
                ? result.Properties["Title"].ToString()
                : result.Properties["Subtitle"].ToString();

            var newResult =
                $"> {counter}. [{title}](https://bungie.net/en/Explore/Detail/News/{result.ContentId})";

            if (currentLength + newResult.Length > 1024)
            {
                embedFields.Add(new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Results",
                    Value = sb.ToString()
                });
                sb.Clear();
            }

            sb.AppendLine(newResult);
        }

        if (sb.Length > 0)
            embedFields.Add(new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "Results",
                Value = sb.ToString()
            });

        return embedFields;
    }

    private static IEnumerable<ContentItemPublicContract> FillTwabList(string query)
    {
        var twabList = new List<ContentItemPublicContract>();

        var done = false;
        var i = 1;

        do
        {
            var twabPage = BungieAPI.GetApiClient().Api
                .Content_SearchContentWithText("en", "news", i, searchtext: query).Result;

            foreach (var page in twabPage.Results)
            {
                var alreadyPresent = false;

                foreach (var unused in twabList.Where(contentItemPublicContract =>
                             contentItemPublicContract.ContentId == page.ContentId))
                    alreadyPresent = true;

                if (alreadyPresent) continue;

                if (!page.Properties["Title"].ToString()!.ToLower().Contains("week")) continue;

                if (!twabList.Contains(page))
                    twabList.Add(page);
            }

            if (twabPage.HasMore)
            {
                i++;
                continue;
            }

            done = true;
        } while (!done);

        return twabList;
    }
}