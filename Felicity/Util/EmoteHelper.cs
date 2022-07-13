using System.Text.Json;
using Discord;
using Discord.WebSocket;
using DotNetBungieAPI.Models.Destiny;
using DotNetBungieAPI.Models.Destiny.Definitions.InventoryItems;
using Felicity.Models.Caches;
using Emote = Felicity.Models.Caches.Emote;
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

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

    public static string GetWeaponType(DestinyInventoryItemDefinition manifestItem)
    {
        var result = manifestItem.ItemSubType switch
        {
            DestinyItemSubType.AutoRifle => "<:autorifle:996495566521520208> ",
            DestinyItemSubType.Shotgun => "<:shotgun:996495567825948672> ",
            DestinyItemSubType.Machinegun => "<:machinegun:996495568887087196> ",
            DestinyItemSubType.HandCannon => "<:handcannon:996492277373476906> ",
            DestinyItemSubType.RocketLauncher => "<:rocketlauncher:996493601083244695> ",
            DestinyItemSubType.FusionRifle => "<:fusionrifle:996495565082873976> ",
            DestinyItemSubType.SniperRifle => "<:sniperrifle:996492271212040243> ",
            DestinyItemSubType.PulseRifle => "<:pulserifle:996493599871078491> ",
            DestinyItemSubType.ScoutRifle => "<:scoutrifle:996492274953371769> ",
            DestinyItemSubType.Sidearm => "<:sidearm:996492272411619470> ",
            DestinyItemSubType.Sword => "<:sword:996492273795727361> ",
            DestinyItemSubType.FusionRifleLine => "<:linearfusion:996497905865195540> ",
            DestinyItemSubType.GrenadeLauncher => "<:grenadelauncher:996492276228436000> ",
            DestinyItemSubType.SubmachineGun => "<:submachinegun:996493598495359096> ",
            DestinyItemSubType.TraceRifle => "<:tracerifle:996495569650466929> ",
            DestinyItemSubType.Bow => "<:bow:996493602354114640> ",
            DestinyItemSubType.Glaive => "<:glaive:996495571126845491> ",
            DestinyItemSubType.None => manifestItem.ItemType switch
            {
                DestinyItemType.Vehicle => "<:sparrow:996727310805893181> ",
                DestinyItemType.Ship => "<:ship:996727309069471815> ",
                _ => string.Empty
            },
            _ => string.Empty
        };

        return result != string.Empty ? result : "<:consumables:996724235634491523> ";
    }
}