using System.IO;
using Discord;
using Felicity.Configs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Felicity.Helpers
{
    internal class ConfigHelper
    {
        private const string configBasePath = "Configs/";
        private static IConfiguration _activeConfig;
        private static IConfiguration _botConfig;
        private static IConfiguration _dataConfig;
        private static IConfiguration _emoteConfig;
        public static string ActiveConfigPath => configBasePath + "activeConfig.json";
        public static string BotConfigPath => configBasePath + "botConfig.json";
        public static string DataConfigPath => configBasePath + "dataConfig.json";
        public static string EmoteConfigPath => configBasePath + "emoteConfig.json";

        public static bool LoadConfigFiles()
        {
            var closeProgram = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            if (File.Exists(BotConfigPath))
            {
                _botConfig = new ConfigurationBuilder()
                    .AddJsonFile(BotConfigPath, false, true)
                    .Build();
            }
            else
            {
                File.WriteAllText(BotConfigPath, JsonConvert.SerializeObject(new BotConfig(), Formatting.Indented));
                Log.Error("<ConfigHelper> No botConfig.json file detected, creating new file.");
                closeProgram = true;
            }

            /*if (File.Exists(DataConfigPath))
            {
                dataConfig = new ConfigurationBuilder().AddJsonFile(DataConfigPath).Build();
            }
            else
            {
                File.WriteAllText(DataConfigPath,
                    JsonConvert.SerializeObject(new DataConfig(), Formatting.Indented));
                Log.Error("<ConfigHelper> No dataConfig.json file detected, creating new file.");
                closeProgram = true;
            }

            if (File.Exists(ActiveConfigPath))
            {
                activeConfig = new ConfigurationBuilder().AddJsonFile(ActiveConfigPath).Build();
            }
            else
            {
                File.WriteAllText(ActiveConfigPath,
                    JsonConvert.SerializeObject(new ActiveConfig(), Formatting.Indented));
                Log.Error("<ConfigHelper> No activeConfig.json file detected, creating new file.");
                closeProgram = true;
            }

            if (File.Exists(EmoteConfigPath))
            {
                emoteConfig = new ConfigurationBuilder().AddJsonFile(EmoteConfigPath).Build();
            }
            else
            {
                File.WriteAllText(EmoteConfigPath,
                    JsonConvert.SerializeObject(new EmoteConfig(), Formatting.Indented));
                Log.Error("<ConfigHelper> No emoteConfig.json file detected, creating new file.");
                closeProgram = true;
            }*/

            return closeProgram;
        }

        public static Color GetEmbedColor()
        {
            var settings = GetBotSettings();
            return new Color(settings.EmbedColor.R, settings.EmbedColor.G, settings.EmbedColor.B);
        }

        public static BotSettings GetBotSettings()
        {
            return _botConfig.GetRequiredSection("Settings").Get<BotSettings>();
        }
    }
}