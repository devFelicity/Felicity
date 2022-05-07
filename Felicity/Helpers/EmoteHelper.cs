using System.IO;
using System.Net.Http;
using APIHelper;
using Discord;
using Discord.WebSocket;

namespace Felicity.Helpers;

internal static class EmoteHelper
{
    public static DiscordSocketClient _client;

    private static GuildEmote AddEmote(string imageUrl, string name)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var serverID in ConfigHelper.GetEmoteSettings().ServerIDs)
        {
            var server = _client.GetGuild(serverID);

            if (server.Emotes.Count >= 50)
                continue;

            var imageBytes = new HttpClient().GetByteArrayAsync($"{RemoteAPI.apiBaseUrl}{imageUrl}").Result;
            var emote = server.CreateEmoteAsync(name, new Image(new MemoryStream(imageBytes))).Result;
            return emote;
        }

        return null;
    }

    public static GuildEmote GetEmote(string imageUrl, string name)
    {
        name = name.Replace(" ", "").Replace("-", "").Replace("'", "");

        foreach (var serverID in ConfigHelper.GetEmoteSettings().ServerIDs)
        {
            var guild = _client.GetGuild(serverID);

            foreach (var guildEmote in guild.Emotes)
            {
                if (guildEmote.Animated)
                    continue;

                if (guildEmote.Name == name)
                    return guildEmote;
            }
        }

        return imageUrl == null ? null : AddEmote(imageUrl, name);
    }
}