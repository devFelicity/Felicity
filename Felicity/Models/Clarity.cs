using System.Globalization;
using Felicity.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable ClassNeverInstantiated.Global

namespace Felicity.Models;

public class Clarity
{
    [JsonProperty("hash")] public long Hash { get; set; }

    [JsonProperty("name")] public string? Name { get; set; }

    [JsonProperty("lastUpdate")] public long LastUpdate { get; set; }

    [JsonProperty("updatedBy")] public string? UpdatedBy { get; set; }

    [JsonProperty("type")] public TypeEnum Type { get; set; }

    [JsonProperty("description")] public string? Description { get; set; }

    [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
    public Stats? Stats { get; set; }

    [JsonProperty("itemHash", NullValueHandling = NullValueHandling.Ignore)]
    public long? ItemHash { get; set; }

    [JsonProperty("itemName", NullValueHandling = NullValueHandling.Ignore)]
    public string? ItemName { get; set; }

    [JsonProperty("investmentStatOnly", NullValueHandling = NullValueHandling.Ignore)]
    public bool? InvestmentStatOnly { get; set; }
}

public class Stats
{
    [JsonProperty("damage", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Damage { get; set; }

    [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Range { get; set; }

    [JsonProperty("handling", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Handling { get; set; }

    [JsonProperty("reload", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Reload { get; set; }

    [JsonProperty("stability", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Stability { get; set; }

    [JsonProperty("aimAssist", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? AimAssist { get; set; }

    [JsonProperty("chargeDraw", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? ChargeDraw { get; set; }

    [JsonProperty("chargeDrawTime", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? ChargeDrawTime { get; set; }

    [JsonProperty("draw", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Draw { get; set; }

    [JsonProperty("zoom", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Zoom { get; set; }

    [JsonProperty("stow", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Stow { get; set; }
}

public class StatType
{
    [JsonProperty("stat")] public long[]? Stat { get; set; }

    [JsonProperty("multiplier", NullValueHandling = NullValueHandling.Ignore)]
    public double[]? Multiplier { get; set; }
}

public class Stat
{
    [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Active { get; set; }

    [JsonProperty("passive", NullValueHandling = NullValueHandling.Ignore)]
    public StatType? Passive { get; set; }
}

public enum TypeEnum
{
    ArmorExotic,
    WeaponMod,
    WeaponOriginTrait,
    WeaponPerk,
    WeaponPerkEnhanced
}

public static class ClarityParser
{
    private static Dictionary<string, Clarity>? _clarityDb;

    public static string ToJson(this Dictionary<string, Clarity> self)
    {
        return JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public static async Task<Dictionary<string, Clarity>?> Fetch()
    {
        if (_clarityDb != null)
            return _clarityDb;

        using var httpClient = new HttpClient();
        var json = await httpClient.GetStringAsync(
            "https://raw.githubusercontent.com/Ice-mourne/database-clarity/main/descriptions/crayon.json");

        _clarityDb = JsonConvert.DeserializeObject<Dictionary<string, Clarity>>(json, Converter.Settings);

        return _clarityDb;
    }

    public static string ClarityClean(this string inputString)
    {
        var outputString = inputString
            .Replace("export description (\n", "")
            .Replace("[this sheet ](https://d2clarity.page.link/combatantundefined",
                "[this sheet](https://d2clarity.page.link/combatant)")
            .Replace("->1", "-> 1")
            .Replace("\r", "")
            .Replace("**", "")
            .Replace("<:primary:968793055677251604>", EmoteHelper.StaticEmote("primary"))
            .Replace("<:special:968793055631114330>", EmoteHelper.StaticEmote("special"))
            .Replace("<:heavy:968793055652106320>", EmoteHelper.StaticEmote("heavy"))
            .Replace("<:stasis:915198000727461909>", EmoteHelper.StaticEmote("stasis"))
            .Replace("<:arc:720178925317128243>", EmoteHelper.StaticEmote("arc"))
            .Replace("<:solar:720178909361995786>", EmoteHelper.StaticEmote("solar"))
            .Replace("<:void:720178940240461864>", EmoteHelper.StaticEmote("void"))
            .Replace("<:pve:922884406073507930>", EmoteHelper.StaticEmote("pve"))
            .Replace("<:pvp:922884468275019856>", EmoteHelper.StaticEmote("pvp"));

        return outputString;
    }
}

internal static class Converter
{
    public static readonly JsonSerializerSettings? Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            TypeEnumConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        }
    };
}

internal class TypeEnumConverter : JsonConverter
{
    public static readonly TypeEnumConverter Singleton = new();

    public override bool CanConvert(Type t) =>
        t == typeof(TypeEnum) || t == typeof(TypeEnum?);

    public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var value = serializer.Deserialize<string>(reader);
        return value switch
        {
            "armorExotic" => TypeEnum.ArmorExotic,
            "weaponMod" => TypeEnum.WeaponMod,
            "weaponOriginTrait" => TypeEnum.WeaponOriginTrait,
            "weaponPerk" => TypeEnum.WeaponPerk,
            "weaponPerkEnhanced" => TypeEnum.WeaponPerkEnhanced,
            _ => throw new Exception("Cannot un-marshal type TypeEnum")
        };
    }

    public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }

        var value = (TypeEnum)untypedValue;
        switch (value)
        {
            case TypeEnum.ArmorExotic:
                serializer.Serialize(writer, "armorExotic");
                return;
            case TypeEnum.WeaponMod:
                serializer.Serialize(writer, "weaponMod");
                return;
            case TypeEnum.WeaponOriginTrait:
                serializer.Serialize(writer, "weaponOriginTrait");
                return;
            case TypeEnum.WeaponPerk:
                serializer.Serialize(writer, "weaponPerk");
                return;
            case TypeEnum.WeaponPerkEnhanced:
                serializer.Serialize(writer, "weaponPerkEnhanced");
                return;
            default:
                throw new Exception("Cannot marshal type TypeEnum");
        }
    }
}