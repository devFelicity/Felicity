using System;
using System.Linq;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny.Entities.Characters;
using BungieSharper.Entities.Destiny.Responses;
using BungieSharper.Entities.User;
using Discord;
using Discord.WebSocket;
// ReSharper disable UnusedMember.Global

namespace Felicity.Helpers;

internal static class Extensions
{
    public static bool IsStaff(this SocketUser user)
    {
        var staff = ConfigHelper.GetBotSettings();
        return staff.BotStaff.Any(staffId => staffId == user.Id);
    }

    public static Embed GenerateLookupEmbed(this DestinyProfileResponse destinyProfile, UserInfoCard userInfoCard)
    {
        DestinyCharacterComponent goodChar = null;

        var lastPlayed = new DateTime();
        foreach (var (_, value) in destinyProfile.Characters.Data.Where(destinyCharacterComponent =>
                     destinyCharacterComponent.Value.DateLastPlayed > lastPlayed))
        {
            lastPlayed = value.DateLastPlayed;
            goodChar = value;
        }

        var fullName = $"{userInfoCard.BungieGlobalDisplayName}#{userInfoCard.BungieGlobalDisplayNameCode}";

        var embed = new EmbedBuilder
        {
            Color = ConfigHelper.GetEmbedColor(),
            Title = fullName,
            Footer = new EmbedFooterBuilder
            {
                Text = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0}",
                IconUrl = "https://whaskell.pw/images/felicity_circle.jpg"
            },
            Description =
                $"{Format.Code($"/addfriend {fullName}")} | " +
                $"{Format.Code($"/invite {fullName}")} | " +
                $"{Format.Code($"/join {fullName}")}",
            ThumbnailUrl = "https://bungie.net" + goodChar?.EmblemPath
        };

        var memTypeAndId = $"{(int) userInfoCard.MembershipType}/{userInfoCard.MembershipId}";

        embed.AddField("General",
            $"[Braytech](https://bray.tech/{memTypeAndId})\n" +
            $"[D2Timeline](https://mijago.github.io/D2Timeline/#/display/{memTypeAndId})\n" +
            $"[Guardian.Report](https://guardian.report/?view=PVE&guardians={userInfoCard.MembershipId})\n", true);
        embed.AddField("PvE",
            $"[Dungeons]({GetReportLink(userInfoCard, "dungeon")})\n" +
            // $"[GM Nightfalls](https://grandmaster.report/user/{memTypeAndId})\n" +
            $"[Nightfalls](https://nightfall.report/guardian/{memTypeAndId})\n" +
            $"[Raids]({GetReportLink(userInfoCard, "raid")})", true);
        embed.AddField("PvP",
            $"[Crucible](https://crucible.report/report/{memTypeAndId})\n" +
            $"[DestinyTracker](https://destinytracker.com/destiny-2/profile/bungie/{userInfoCard.MembershipId}/overview?perspective=pvp)\n" +
            $"[Trials](https://trials.report/report/{memTypeAndId})", true);

        return embed.Build();
    }

    private static string GetReportLink(UserInfoCard userInfoCard, string reportType)
    {
        string platform;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (userInfoCard.MembershipType)
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

        return $"https://{reportType}.report/{platform}/{userInfoCard.MembershipId}";
    }
}