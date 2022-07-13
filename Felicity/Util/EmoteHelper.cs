using System.Text.Json;
using Discord;
using Discord.WebSocket;
using DotNetBungieAPI.Models.Destiny;
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
            foreach (var settingsServerId in emoteConfig.Settings.ServerIDs!)
            {
                var serverMotes = discordClient.GetGuild(settingsServerId).Emotes;
                foreach (var serverMote in serverMotes)
                    if (serverMote.Name == name)
                        return serverMote;
            }

        emoteConfig.Settings.Emotes ??= new Dictionary<uint, Emote>();

        if (valuePerkId == null)
            return null;

        if (emoteConfig.Settings.Emotes.ContainsKey((uint)valuePerkId))
            foreach (var settingsServerId in emoteConfig.Settings.ServerIDs!)
            {
                var serverMotes = discordClient.GetGuild(settingsServerId).Emotes;
                var emotes = serverMotes.FirstOrDefault(x => x.Id == emoteConfig.Settings.Emotes[(uint)valuePerkId].Id);
                if (emotes != null)
                    return emotes;
            }

        name = name.Replace(" ", "").Replace("-", "").Replace("'", "");

        var result = AddEmote(discordClient, imageUrl, name);

        if (result == null)
            return result;

        emoteConfig.Settings.Emotes.Add((uint)valuePerkId, new Emote { Id = result.Id, Name = result.Name });
        WriteEmoteCache(emoteConfig);
        return result;
    }

    public static string GetWeaponType(DestinyItemSubType manifestItemItemSubType)
    {
        var result = string.Empty;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (manifestItemItemSubType)
        {
            case DestinyItemSubType.AutoRifle:
                result += "<:autorifle:996495566521520208> ";
                break;
            case DestinyItemSubType.Shotgun:
                result += "<:shotgun:996495567825948672> ";
                break;
            case DestinyItemSubType.Machinegun:
                result += "<:machinegun:996495568887087196> ";
                break;
            case DestinyItemSubType.HandCannon:
                result += "<:handcannon:996492277373476906> ";
                break;
            case DestinyItemSubType.RocketLauncher:
                result += "<:rocketlauncher:996493601083244695> ";
                break;
            case DestinyItemSubType.FusionRifle:
                result += "<:fusionrifle:996495565082873976> ";
                break;
            case DestinyItemSubType.SniperRifle:
                result += "<:sniperrifle:996492271212040243> ";
                break;
            case DestinyItemSubType.PulseRifle:
                result += "<:pulserifle:996493599871078491> ";
                break;
            case DestinyItemSubType.ScoutRifle:
                result += "<:scoutrifle:996492274953371769> ";
                break;
            case DestinyItemSubType.Sidearm:
                result += "<:sidearm:996492272411619470> ";
                break;
            case DestinyItemSubType.Sword:
                result += "<:sword:996492273795727361> ";
                break;
            case DestinyItemSubType.FusionRifleLine:
                result += "<:linearfusion:996497905865195540> ";
                break;
            case DestinyItemSubType.GrenadeLauncher:
                result += "<:grenadelauncher:996492276228436000> ";
                break;
            case DestinyItemSubType.SubmachineGun:
                result += "<:submachinegun:996493598495359096> ";
                break;
            case DestinyItemSubType.TraceRifle:
                result += "<:tracerifle:996495569650466929> ";
                break;
            case DestinyItemSubType.Bow:
                result += "<:bow:996493602354114640> ";
                break;
            case DestinyItemSubType.Glaive:
                result += "<:glaive:996495571126845491> ";
                break;
        }

        return result;
    }
}