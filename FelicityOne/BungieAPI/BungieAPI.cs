using BungieSharper.Client;
using FelicityOne.Enums;
using FelicityOne.Services;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace

namespace FelicityOne;

internal static class BungieAPI
{
    public const string BaseUrl = "https://bungie.net";
    private static BungieApiClient? _apiClient;

    public static BungieApiClient GetApiClient()
    {
        if (_apiClient != null) return _apiClient;

        var config = ConfigService.GetBotSettings();

        var bConfig = new BungieClientConfig
        {
            ApiKey = config.BungieApiKey,
            OAuthClientId = uint.Parse(config.BungieClientId),
            OAuthClientSecret = config.BungieClientSecret,
            UserAgent = "Felicity/v5.0 (+will09600@gmail.com)"
        };

        _apiClient = new BungieApiClient(bConfig);

        return _apiClient;
    }

    public static List<T> GetManifestDefinition<T>(Lang lang, IEnumerable<uint> itemhash)
    {
        var result = new List<T>();

        var manifest = GetApiClient().Api.Destiny2_GetDestinyManifest().Result;

        var client = new HttpClient();

        var url = BaseUrl + manifest.JsonWorldComponentContentPaths[EnumConverter.LangToString(lang)][typeof(T).Name];

        using (var s = client.GetStreamAsync(url).Result)
        using (var sr = new StreamReader(s))
        using (JsonReader reader = new JsonTextReader(sr))
        {
            var dictionary = new JsonSerializer().Deserialize<Dictionary<string, T>>(reader);
            if (dictionary != null)
            {
                result.AddRange(itemhash.Select(u => dictionary[u.ToString()]));
            }
        }

        client.Dispose();

        return result;
    }
}