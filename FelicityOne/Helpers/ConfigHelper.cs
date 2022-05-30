using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Helpers;

public static class ConfigHelper
{
    public static T? FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    public static string ToJson(object config)
    {
        return JsonConvert.SerializeObject(config, Settings);
    }

    private static readonly JsonSerializerSettings Settings = new()
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