using System.Text;
using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Models.Content;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Util;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

public class NewsCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IBungieClient _bungieClient;

    public NewsCommands(IBungieClient bungieClient)
    {
        _bungieClient = bungieClient;
    }

    [SlashCommand("twab", "Where TWAB?")]
    public async Task Twab(
        [Summary("query", "Search TWABs for a phrase or word. If null, latest TWAB will be returned.")]
        string search =
            "")
    {
        if (!await BungieApiUtils.CheckApi(_bungieClient))
            throw new Exception("Bungie API is down or unresponsive.");

        const string lg = "en";

        if (!string.IsNullOrEmpty(search))
        {
            var searchEmbed = new EmbedBuilder
            {
                Title = $"TWAB search for `{search}`.",
                Color = Color.Teal,
                Footer = Embeds.MakeFooter()
            };

            var results = await SearchTwab(search);

            if (results.Count != 0)
                searchEmbed.Fields = results;

            await FollowupAsync(embed: searchEmbed.Build());

            return;
        }

        var twabTask =
            await _bungieClient.ApiAccess.Content.SearchContentWithText(lg, new[] { "news" }, "this week at bungie", "",
                "");

        var twab = twabTask.Response.Results.FirstOrDefault();

        if (twab == null)
        {
            var errorEmbed = new EmbedBuilder
            {
                Description = "Failed to find latest TWAB.",
                Color = Color.Teal,
                Footer = Embeds.MakeFooter()
            };

            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var url = $"{BotVariables.BungieBaseUrl}{lg}/Explore/Detail/News/{twab.ContentId}";

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = twab.Author.DisplayName,
                IconUrl = $"{BotVariables.BungieBaseUrl}{twab.Author.ProfilePicturePath}"
            },
            Color = Color.Teal,
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
            Footer = Embeds.MakeFooter(),
            ImageUrl = $"{BotVariables.BungieBaseUrl}{twab.Properties["ArticleBanner"]}",
            Title = twab.Properties["Title"].ToString(),
            Url = url
        };

        await FollowupAsync(embed: embed.Build());
    }

    private async Task<List<EmbedFieldBuilder>> SearchTwab(string search)
    {
        var twabList = new List<ContentItemPublicContract>();

        twabList.AddRange(await FillTwabList("this week at bungie"));
        twabList.AddRange(await FillTwabList("bungie weekly update"));

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
                $"> {counter}. [{title}]({BotVariables.BungieBaseUrl}en/Explore/Detail/News/{result.ContentId})";

            if (currentLength + newResult.Length >= 1024)
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

    private async Task<IEnumerable<ContentItemPublicContract>> FillTwabList(string query)
    {
        var twabList = new List<ContentItemPublicContract>();

        var done = false;
        var i = 1;

        do
        {
            var twabPage =
                await _bungieClient.ApiAccess.Content.SearchContentWithText("en", new[] { "news" }, query, "", "", i);

            foreach (var page in twabPage.Response.Results)
            {
                var alreadyPresent = false;

                foreach (var unused in twabList.Where(contentItemPublicContract =>
                             contentItemPublicContract.ContentId == page.ContentId))
                    alreadyPresent = true;

                if (alreadyPresent) continue;

                if (!page.Properties["Title"].ToString()!.ToLower().Contains("this week"))
                    continue;

                if (!twabList.Contains(page))
                    twabList.Add(page);
            }

            if (twabPage.Response.HasMore)
            {
                i++;
                continue;
            }

            done = true;
        } while (!done);

        return twabList;
    }
}