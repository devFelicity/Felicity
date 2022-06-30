using System.Text.Json;
using Discord;
using Discord.WebSocket;
using Felicity.Models.Caches;
using Emote = Felicity.Models.Caches.Emote;

namespace Felicity.Util;

internal static class EmoteHelper
{
    private const string FilePath = "Data/emoteCache.json";

    private static EmoteCache GetEmoteCache()
    {
        var result = JsonSerializer.Deserialize<EmoteCache>(File.ReadAllText(FilePath)) ?? new EmoteCache();

        return result;
    }

    private static void WriteEmoteCache(EmoteCache cache)
    {
        File.WriteAllText(FilePath, JsonSerializer.Serialize(cache));
    }

    private static GuildEmote? AddEmote(BaseSocketClient discordClient, string imageUrl, string name)
    {
        var emoteSettings = GetEmoteCache();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var serverId in emoteSettings.Settings.ServerIDs!)
        {
            var server = discordClient.GetGuild(serverId);

            if (server != null && server.Emotes.Count >= 50)
                continue;

            var imageBytes = new HttpClient().GetByteArrayAsync($"{BotVariables.BungieBaseUrl}{imageUrl}").Result;
            var emote = server?.CreateEmoteAsync(name, new Image(new MemoryStream(imageBytes))).Result;
            return emote;
        }

        return null;
    }

    public static GuildEmote? GetEmote(BaseSocketClient discordClient, string imageUrl, string name, uint? valuePerkId)
    {
        var emoteConfig = GetEmoteCache();
        
        if (valuePerkId == 0)
        {
            foreach (var settingsServerId in emoteConfig.Settings.ServerIDs!)
            {
                var serverMotes = discordClient.GetGuild(settingsServerId).Emotes;
                foreach (var serverMote in serverMotes)
                {
                    if (serverMote.Name == name)
                        return serverMote;
                }
            }
        }

        emoteConfig.Settings.Emotes ??= new Dictionary<uint, Emote>();

        if (valuePerkId == null)
            return null;

        if (emoteConfig.Settings.Emotes.ContainsKey((uint) valuePerkId))
        {
            foreach (var settingsServerId in emoteConfig.Settings.ServerIDs!)
            {
                var serverMotes = discordClient.GetGuild(settingsServerId).Emotes;
                foreach (var serverMote in serverMotes)
                {
                    if (serverMote.Id == emoteConfig.Settings.Emotes[(uint) valuePerkId].Id)
                        return serverMote;
                }
            }
        }
            
        name = name.Replace(" ", "").Replace("-", "").Replace("'", "");

        var result = AddEmote(discordClient, imageUrl, name);

        if (result == null)
            return result;

        emoteConfig.Settings.Emotes.Add((uint) valuePerkId, new Emote {Id = result.Id, Name = result.Name});
        WriteEmoteCache(emoteConfig);
        return result;
    }
}