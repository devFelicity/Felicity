using BungieSharper.Entities;
using BungieSharper.Entities.Destiny.Entities.Characters;
using BungieSharper.Entities.Destiny.Responses;
using Discord;
using Discord.WebSocket;
using FelicityOne.Configs;
using FelicityOne.Enums;
using FelicityOne.Services;

namespace FelicityOne.Helpers;

internal static class Extensions
{
    public static OAuthConfig OAuth(this SocketUser user)
    {
        return OAuthService.GetUser(user.Id).Result;
    }

    public static bool IsStaff(this SocketUser user)
    {
        var staff = ConfigService.GetBotSettings();
        return staff.BotStaff.Any(staffId => staffId.Id == user.Id);
    }

    public static ServerSetting? Config(this SocketGuild guild)
    {
        return ConfigService.GetServerSettings(guild.Id);
    }

    public static EmbedBuilder GenerateMessageEmbed(string authorName, string authorIcon, string description,
        string authorUrl = "")
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigService.GetEmbedColor(),
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

    public static EmbedBuilder GenerateUserEmbed(SocketUser guildUser)
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigService.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = guildUser.GetDefaultAvatarUrl(),
                Name = guildUser.Username
            },
            Description = $"{Format.Bold(guildUser.Username)} left the server!",
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new()
                {
                    IsInline = true,
                    Name = "Account Created",
                    Value = guildUser.CreatedAt.ToString("d")
                },
                new()
                {
                    IsInline = true,
                    Name = "User ID",
                    Value = guildUser.Id
                },
                new()
                {
                    IsInline = true,
                    Name = "Bungie Name",
                    Value = GetBungieName(guildUser)
                }
            }
        };

        return embed;
    }

    public static EmbedBuilder GenerateUserEmbed(SocketGuildUser guildUser)
    {
        var embed = new EmbedBuilder
        {
            Color = ConfigService.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = guildUser.GetDefaultAvatarUrl(),
                Name = guildUser.DisplayName
            },
            Description = $"{Format.Bold(guildUser.Username)} joined the server!",
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            },
            Fields = new List<EmbedFieldBuilder>
            {
                new()
                {
                    IsInline = true,
                    Name = "Account Created",
                    Value = guildUser.CreatedAt.ToString("d")
                },
                new()
                {
                    IsInline = true,
                    Name = "User ID",
                    Value = guildUser.Id
                },
                new()
                {
                    IsInline = true,
                    Name = "Bungie Name",
                    Value = GetBungieName(guildUser)
                }
            }
        };

        return embed;
    }

    private static string GetBungieName(SocketUser user)
    {
        var oauth = user.OAuth();
        return oauth.DestinyMembership.BungieName;
    }

    public static Embed GenerateLookupEmbed(this DestinyProfileResponse destinyProfile, string bungieName,
        long membershipId, BungieMembershipType membershipType)
    {
        DestinyCharacterComponent goodChar = null!;

        var lastPlayed = new DateTime();
        foreach (var (_, value) in destinyProfile.Characters.Data.Where(destinyCharacterComponent =>
                     destinyCharacterComponent.Value.DateLastPlayed > lastPlayed))
        {
            lastPlayed = value.DateLastPlayed;
            goodChar = value;
        }

        var embed = new EmbedBuilder
        {
            Color = ConfigService.GetEmbedColor(),
            Title = bungieName,
            Footer = new EmbedFooterBuilder
            {
                Text = ConfigService.GetBotSettings().Version,
                IconUrl = Images.FelicityLogo
            },
            Description =
                $"{Format.Code($"/invite {bungieName}")} | " +
                $"{Format.Code($"/join {bungieName}")}",
            ThumbnailUrl = "https://bungie.net" + goodChar.EmblemPath
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
            Color = ConfigService.GetEmbedColor(),
            Author = new EmbedAuthorBuilder
            {
                Name = authorName,
                IconUrl = thumbnailUrl
            },
            Description = description,
            Footer = new EmbedFooterBuilder
            {
                Text = ConfigService.GetBotSettings().Version,
                IconUrl = Images.FelicityLogo
            }
        };

        return embed;
    }
}