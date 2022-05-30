using Discord;
using Discord.WebSocket;
using FelicityOne.Configs;
using FelicityOne.Services;
using Emote = FelicityOne.Configs.Emote;

namespace FelicityOne.Helpers;

internal static class EmoteHelper
{
    public static DiscordSocketClient? DiscordClient;

    private static GuildEmote? AddEmote(string imageUrl, string name)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var serverID in ConfigService.GetEmoteSettings().ServerIDs)
        {
            var server = DiscordClient?.GetGuild(serverID);

            if (server != null && server.Emotes.Count >= 50)
                continue;

            var imageBytes = new HttpClient().GetByteArrayAsync($"https://bungie.net/{imageUrl}").Result;
            var emote = server?.CreateEmoteAsync(name, new Image(new MemoryStream(imageBytes))).Result;
            return emote;
        }

        return null;
    }

    public static GuildEmote? GetEmote(string imageUrl, string name, uint? valuePerkId)
    {
        var emoteConfig = ConfigHelper.FromJson<EmoteConfig>(File.ReadAllText(ConfigService.EmoteConfigPath));
        if (emoteConfig == null) return null;

        if (valuePerkId == 0)
        {
            foreach (var settingsServerID in emoteConfig.Settings.ServerIDs)
            {
                var serverMotes = DiscordClient!.GetGuild(settingsServerID).Emotes;
                foreach (var serverMote in serverMotes)
                {
                    if (serverMote.Name == name)
                        return serverMote;
                }
            }
        }

        emoteConfig.Settings.Emotes ??= new Dictionary<uint, Emote>();

        if (emoteConfig.Settings.Emotes.ContainsKey((uint) valuePerkId))
        {
            foreach (var settingsServerID in emoteConfig.Settings.ServerIDs)
            {
                var serverMotes = DiscordClient!.GetGuild(settingsServerID).Emotes;
                foreach (var serverMote in serverMotes)
                {
                    if (serverMote.Id == emoteConfig.Settings.Emotes[(uint) valuePerkId].Id)
                        return serverMote;
                }
            }
        }
            
        name = name.Replace(" ", "").Replace("-", "").Replace("'", "");

        var result = AddEmote(imageUrl, name);

        if (result == null) return result;

        emoteConfig.Settings.Emotes.Add((uint) valuePerkId, new Emote {Id = result.Id, Name = result.Name});
        File.WriteAllText(ConfigService.EmoteConfigPath, ConfigHelper.ToJson(emoteConfig));
        return result;
    }
}