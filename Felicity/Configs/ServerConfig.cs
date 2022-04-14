using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;

// ReSharper disable UnusedMember.Global
// ReSharper disable PartialTypeWithSinglePart

namespace Felicity.Configs;

public partial class ServerConfig
{
    [J("Settings")] public Dictionary<string, ServerSetting> Settings { get; set; }
}

public partial class ServerSetting
{
    [J("moderatorRole")] public ulong ModeratorRole { get; set; }
    [J("announcementChannel")] public ulong AnnouncementChannel { get; set; }
    [J("staffChannel")] public ulong StaffChannel { get; set; }
    [J("memberCountChannel")] public ulong MemberCountChannel { get; set; }
    [J("boostCountChannel")] public ulong BoostCountChannel { get; set; }
    [J("memberEvents")] public MemberEvents MemberEvents { get; set; } = new();
    [J("subscriptions")] public Dictionary<string, Subscription> Subscriptions { get; set; } = new();
    [J("destiny")] public Destiny Destiny { get; set; } = new();
}

public partial class Destiny
{
    [J("dailyReset")] public Vendor DailyReset { get; set; } = new();
    [J("weeklyReset")] public Vendor WeeklyReset { get; set; } = new();
    [J("xur")] public Vendor Xur { get; set; } = new();
    [J("ada")] public Vendor Ada { get; set; } = new();
    [J("gunsmith")] public Vendor Gunsmith { get; set; } = new();
}

public partial class Vendor
{
    [J("channelId")] public ulong ChannelId { get; set; }
    [J("enabled")] public bool Enabled { get; set; }
}

public partial class MemberEvents
{
    [J("logChannel")] public ulong LogChannel { get; set; }
    [J("memberJoined")] public bool MemberJoined { get; set; }
    [J("memberLeft")] public bool MemberLeft { get; set; }
    [J("memberKicked")] public bool MemberKicked { get; set; }
    [J("memberBanned")] public bool MemberBanned { get; set; }
}

public partial class Subscription
{
    [J("type")] public string Type { get; set; }
    [J("url")] public string Url { get; set; }
    [J("channelId")] public ulong ChannelId { get; set; }
}

public partial class ServerConfig
{
    public static ServerConfig FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ServerConfig>(json, Converter.Settings);
    }

    public static string ToJson(ServerConfig self)
    {
        return JsonConvert.SerializeObject(self, Converter.Settings);
    }
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Formatting = Formatting.Indented,
        Converters =
        {
            new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
        }
    };
}