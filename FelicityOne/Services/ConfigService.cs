using Discord;
using FelicityOne.Configs;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace FelicityOne.Services;

internal static class ConfigService
{
    private const string configBasePath = "Configs/";
    private static IConfiguration? _botConfig;
    private static IConfiguration? _emoteConfig;
    private static IConfiguration? _serverConfig;
    private static string BotConfigPath => configBasePath + "botConfig.json";
    public static string ServerConfigPath => configBasePath + "serverConfig.json";
    public static string EmoteConfigPath => configBasePath + "emoteConfig.json";

    public static bool LoadConfigFiles()
    {
        var closeProgram = false;

        if (!Directory.Exists("Configs"))
            Directory.CreateDirectory("Configs");

        if (!Directory.Exists("Users"))
            Directory.CreateDirectory("Users");

        if (File.Exists(BotConfigPath))
        {
            _botConfig = new ConfigurationBuilder()
                .AddJsonFile(BotConfigPath, false, true)
                .Build();
        }
        else
        {
            File.WriteAllText(BotConfigPath,
                JsonConvert.SerializeObject(new BotConfig(), Formatting.Indented));
            Console.WriteLine($"No {BotConfigPath} found.");
            closeProgram = true;
        }

        if (File.Exists(ServerConfigPath))
        {
            _serverConfig = new ConfigurationBuilder()
                .AddJsonFile(ServerConfigPath, false, true)
                .Build();
        }
        else
        {
            File.WriteAllText(ServerConfigPath, 
                JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented));
            Log.Information("No {ConfigFile} found.", ServerConfigPath);
            closeProgram = true;
        }

        if (File.Exists(EmoteConfigPath))
        {
            _emoteConfig = new ConfigurationBuilder().AddJsonFile(EmoteConfigPath, false, true).Build();
        }
        else
        {
            File.WriteAllText(EmoteConfigPath,
                JsonConvert.SerializeObject(new EmoteConfig(), Formatting.Indented));
            Console.WriteLine($"No {EmoteConfigPath} found.");
            closeProgram = true;
        }

        KnownMementos.PopulateMementos();

        return closeProgram;
    }

    public static Color GetEmbedColor()
    {
        var settings = GetBotSettings().EmbedColor;
        return new Color(settings.R, settings.G, settings.B);
    }

    public static BotSettings GetBotSettings()
    {
        return _botConfig.GetRequiredSection("Settings").Get<BotSettings>();
    }

    public static EmoteSettings GetEmoteSettings()
    {
        return _emoteConfig.GetRequiredSection("Settings").Get<EmoteSettings>();
    }

    public static ServerSetting? GetServerSettings(ulong serverID)
    {
        try
        {
            var result = _serverConfig.GetRequiredSection("Settings").GetRequiredSection(serverID.ToString())
                .Get<ServerSetting>();

            return result;
        }
        catch
        {
            return null;
        }
    }

    public static OAuthConfig? GetUserSettings(ulong userID)
    {
        var path = $"Users/{userID}.json";
        return File.Exists(path) ? ConfigHelper.FromJson<OAuthConfig>(File.ReadAllText(path)) : null;
    }
}