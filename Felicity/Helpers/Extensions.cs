using System;
using System.Linq;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny.Entities.Characters;
using BungieSharper.Entities.Destiny.Responses;
using Discord;
using Discord.WebSocket;
using Felicity.Configs;
using Felicity.Enums;
using Felicity.Services;

// ReSharper disable UnusedMember.Global

namespace Felicity.Helpers;

internal static class Extensions
{
    public static OAuthConfig OAuth(this SocketUser user)
    {
        return OAuthService.GetUser(user.Id).Result;
    }

    public static bool IsStaff(this SocketUser user)
    {
        var staff = ConfigHelper.GetBotSettings();
        return staff.BotStaff.Any(staffId => staffId == user.Id);
    }

    public static EmbedBuilder GenerateMessageEmbed(string authorName, string authorIcon, string description, string authorUrl = "")
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = authorIcon,
                Name = authorName,
                Url = authorUrl
            },
            Description = description,
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            }
        };

        return embed;
    }

    public static Embed GenerateLookupEmbed(this DestinyProfileResponse destinyProfile, string bungieName,
        long membershipId, BungieMembershipType membershipType)
    {
        DestinyCharacterComponent goodChar = null;

        var lastPlayed = new DateTime();
        foreach (var (_, value) in destinyProfile.Characters.Data.Where(destinyCharacterComponent =>
                     destinyCharacterComponent.Value.DateLastPlayed > lastPlayed))
        {
            lastPlayed = value.DateLastPlayed;
            goodChar = value;
        }

        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Title = bungieName,
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            },
            Description =
                $"{Format.Code($"/invite {bungieName}")} | " +
                $"{Format.Code($"/join {bungieName}")}",
            ThumbnailUrl = "https://bungie.net" + goodChar?.EmblemPath
        };

        var memTypeAndId = $"{(int) membershipType}/{membershipId}";

        embed.AddField("General",
            $"[Braytech](https://bray.tech/{memTypeAndId})\n" +
            $"[D2Timeline](https://mijago.github.io/D2Timeline/#/display/{memTypeAndId})\n" +
            $"[Guardian.Report](https://guardian.report/?view=PVE&guardians={membershipId})\n", true);
        embed.AddField("PvE",
            $"[Dungeons]({GetReportLink(membershipType, membershipId, "dungeon")})\n" +
            $"[Nightfalls](https://nightfall.report/guardian/{memTypeAndId})\n" +
            $"[Raids]({GetReportLink(membershipType, membershipId, "raid")})", true);
        embed.AddField("PvP",
            $"[Crucible](https://crucible.report/report/{memTypeAndId})\n" +
            $"[DestinyTracker](https://destinytracker.com/destiny-2/profile/bungie/{membershipId}/overview?perspective=pvp)\n" +
            $"[Trials](https://trials.report/report/{memTypeAndId})", true);

        return embed.Build();
    }

    private static string GetReportLink(BungieMembershipType membershipType, long membershipId, string reportType)
    {
        string platform;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (membershipType)
        {
            case BungieMembershipType.TigerXbox:
                platform = "xb";
                break;
            case BungieMembershipType.TigerPsn:
                platform = "ps";
                break;
            case BungieMembershipType.TigerSteam:
                platform = "pc";
                break;
            case BungieMembershipType.TigerStadia:
                platform = "stadia";
                break;
            default:
                return $"https://{reportType}.report";
        }

        return $"https://{reportType}.report/{platform}/{membershipId}";
    }

    public static EmbedBuilder GenerateVendorEmbed(string authorName, string thumbnailUrl, string description)
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                Name = authorName,
                IconUrl = thumbnailUrl
            },
            Description = description,
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            }
        };

        return embed;
    }
}