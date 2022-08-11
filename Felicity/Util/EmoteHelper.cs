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

        if (emoteConfig.Settings.Emotes.ContainsKey((uint)valuePerkId))
            emoteConfig.Settings.Emotes.Remove((uint)valuePerkId);

        name = name.Replace(" ", "").Replace("-", "").Replace("'", "");

        var result = AddEmote(discordClient, imageUrl, name);

        if (result == null)
            return result;

        emoteConfig.Settings.Emotes.Add((uint)valuePerkId, new Emote { Id = result.Id, Name = result.Name });
        WriteEmoteCache(emoteConfig);
        return result;
    }

    public static string StaticEmote(string input)
    {
        var output = input.ToLower() switch
        {
            "primary" => "<:primary:1006946858318434364>",
            "special" => "<:special:1006946861078290523>",
            "heavy" => "<:heavy:1006946859610296510>",
            "pve" => "<:pve:1006939958013079643>",
            "pvp" => "<:pvp:1006939951843246120>",
            "arc" => "<:arc:1006939955718783098>",
            "solar" => "<:solar:1006939954364039228>",
            "void" => "<:void:1006939953139294269>",
            "stasis" => "<:stasis:1006939956901580894>",
            _ => ""
        };

        return output;
    }

    public static string GetItemType(DestinyInventoryItemDefinition manifestItem)
    {
        var result = manifestItem.ItemSubType switch
        {
            DestinyItemSubType.None => manifestItem.ItemType switch
            {
                DestinyItemType.Vehicle => "<:SW:996727310805893181> ",
                DestinyItemType.Ship => "<:SP:996727309069471815> ",
                _ => string.Empty
            },
            _ => GetItemType(manifestItem.ItemSubType)
        };

        return result != string.Empty ? result : "<:CS:996724235634491523> ";
    }

    public static string GetItemType(DestinyItemSubType? weaponDestinyItemType)
    {
        var result = weaponDestinyItemType switch
        {
            DestinyItemSubType.AutoRifle => "<:AR:996495566521520208> ",
            DestinyItemSubType.Shotgun => "<:SG:996495567825948672> ",
            DestinyItemSubType.Machinegun => "<:LMG:996495568887087196> ",
            DestinyItemSubType.HandCannon => "<:HC:996492277373476906> ",
            DestinyItemSubType.RocketLauncher => "<:RL:996493601083244695> ",
            DestinyItemSubType.FusionRifle => "<:FR:996495565082873976> ",
            DestinyItemSubType.SniperRifle => "<:SR:996492271212040243> ",
            DestinyItemSubType.PulseRifle => "<:PR:996493599871078491> ",
            DestinyItemSubType.ScoutRifle => "<:ScR:996492274953371769> ",
            DestinyItemSubType.Sidearm => "<:SA:996492272411619470> ",
            DestinyItemSubType.Sword => "<:SRD:996492273795727361> ",
            DestinyItemSubType.FusionRifleLine => "<:LFR:996497905865195540> ",
            DestinyItemSubType.GrenadeLauncher => "<:GL:996492276228436000> ",
            DestinyItemSubType.SubmachineGun => "<:SMG:996493598495359096> ",
            DestinyItemSubType.TraceRifle => "<:TR:996495569650466929> ",
            DestinyItemSubType.Bow => "<:BW:996493602354114640> ",
            DestinyItemSubType.Glaive => "<:GV:996495571126845491> ",
            DestinyItemSubType.HelmetArmor => "<:helmet:996490149728899122> ",
            DestinyItemSubType.GauntletsArmor => "<:gloves:996490148025995385> ",
            DestinyItemSubType.ChestArmor => "<:chest:996490146922901655> ",
            DestinyItemSubType.LegArmor => "<:boots:996490145224200292> ",
            DestinyItemSubType.ClassArmor => "<:class:996490144066572288> ",
            _ => string.Empty
        };

        return result != string.Empty ? result : "<:CS:996724235634491523> ";
    }
}