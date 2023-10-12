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
    public Stat[]? Damage { get; set; }

    [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Range { get; set; }

    [JsonProperty("handling", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Handling { get; set; }

    [JsonProperty("reload", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Reload { get; set; }

    [JsonProperty("stability", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Stability { get; set; }

    [JsonProperty("aimAssist", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? AimAssist { get; set; }

    [JsonProperty("chargeDraw", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? ChargeDraw { get; set; }

    [JsonProperty("chargeDrawTime", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? ChargeDrawTime { get; set; }

    [JsonProperty("draw", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Draw { get; set; }

    [JsonProperty("zoom", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Zoom { get; set; }

    [JsonProperty("stow", NullValueHandling = NullValueHandling.Ignore)]
    public Stat[]? Stow { get; set; }
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
    ArmorModActivity,
    ArmorModGeneral,
    ArmorPerkExotic,
    SubclassClass,
    SubclassFragment,
    SubclassGrenade,
    SubclassMelee,
    SubclassMovement,
    SubclassSuper,
    TypeWeaponCatalystExotic,
    TypeWeaponFrame,
    TypeWeaponFrameExotic,
    TypeWeaponMod,
    TypeWeaponPerkEnhanced,
    WeaponCatalystExotic,
    WeaponFrame,
    WeaponFrameExotic,
    WeaponMod,
    WeaponOriginTrait,
    WeaponPerk,
    WeaponPerkEnhanced,
    WeaponPerkExotic
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
            "https://raw.githubusercontent.com/Database-Clarity/Live-Clarity-Database/live/descriptions/crayon.json");

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

    public override bool CanConvert(Type t)
    {
        return t == typeof(TypeEnum) || t == typeof(TypeEnum?);
    }

    public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var value = serializer.Deserialize<string>(reader);
        return value switch
        {
            "Armor Mod Activity" => TypeEnum.ArmorModActivity,
            "Armor Mod General" => TypeEnum.ArmorModGeneral,
            "Armor Perk Exotic" => TypeEnum.ArmorPerkExotic,
            "Subclass Class" => TypeEnum.SubclassClass,
            "Subclass Fragment" => TypeEnum.SubclassFragment,
            "Subclass Grenade" => TypeEnum.SubclassGrenade,
            "Subclass Melee" => TypeEnum.SubclassMelee,
            "Subclass Movement" => TypeEnum.SubclassMovement,
            "Subclass Super" => TypeEnum.SubclassSuper,
            "Weapon Catalyst Exotic" => TypeEnum.WeaponCatalystExotic,
            "Weapon Frame" => TypeEnum.WeaponFrame,
            "Weapon Frame Exotic" => TypeEnum.WeaponFrameExotic,
            "Weapon Mod" => TypeEnum.WeaponMod,
            "Weapon Origin Trait" => TypeEnum.WeaponOriginTrait,
            "Weapon Perk" => TypeEnum.WeaponPerk,
            "Weapon Perk Enhanced" => TypeEnum.WeaponPerkEnhanced,
            "Weapon Perk Exotic" => TypeEnum.WeaponPerkExotic,
            "weaponCatalystExotic" => TypeEnum.TypeWeaponCatalystExotic,
            "weaponFrame" => TypeEnum.TypeWeaponFrame,
            "weaponFrameExotic" => TypeEnum.TypeWeaponFrameExotic,
            "weaponMod" => TypeEnum.TypeWeaponMod,
            "weaponPerkEnhanced" => TypeEnum.TypeWeaponPerkEnhanced,
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
            case TypeEnum.ArmorModActivity:
                serializer.Serialize(writer, "Armor Mod Activity");
                return;
            case TypeEnum.ArmorModGeneral:
                serializer.Serialize(writer, "Armor Mod General");
                return;
            case TypeEnum.ArmorPerkExotic:
                serializer.Serialize(writer, "Armor Perk Exotic");
                return;
            case TypeEnum.SubclassClass:
                serializer.Serialize(writer, "Subclass Class");
                return;
            case TypeEnum.SubclassFragment:
                serializer.Serialize(writer, "Subclass Fragment");
                return;
            case TypeEnum.SubclassGrenade:
                serializer.Serialize(writer, "Subclass Grenade");
                return;
            case TypeEnum.SubclassMelee:
                serializer.Serialize(writer, "Subclass Melee");
                return;
            case TypeEnum.SubclassMovement:
                serializer.Serialize(writer, "Subclass Movement");
                return;
            case TypeEnum.SubclassSuper:
                serializer.Serialize(writer, "Subclass Super");
                return;
            case TypeEnum.WeaponCatalystExotic:
                serializer.Serialize(writer, "Weapon Catalyst Exotic");
                return;
            case TypeEnum.WeaponFrame:
                serializer.Serialize(writer, "Weapon Frame");
                return;
            case TypeEnum.WeaponFrameExotic:
                serializer.Serialize(writer, "Weapon Frame Exotic");
                return;
            case TypeEnum.WeaponMod:
                serializer.Serialize(writer, "Weapon Mod");
                return;
            case TypeEnum.WeaponOriginTrait:
                serializer.Serialize(writer, "Weapon Origin Trait");
                return;
            case TypeEnum.WeaponPerk:
                serializer.Serialize(writer, "Weapon Perk");
                return;
            case TypeEnum.WeaponPerkEnhanced:
                serializer.Serialize(writer, "Weapon Perk Enhanced");
                return;
            case TypeEnum.WeaponPerkExotic:
                serializer.Serialize(writer, "Weapon Perk Exotic");
                return;
            case TypeEnum.TypeWeaponCatalystExotic:
                serializer.Serialize(writer, "weaponCatalystExotic");
                return;
            case TypeEnum.TypeWeaponFrame:
                serializer.Serialize(writer, "weaponFrame");
                return;
            case TypeEnum.TypeWeaponFrameExotic:
                serializer.Serialize(writer, "weaponFrameExotic");
                return;
            case TypeEnum.TypeWeaponMod:
                serializer.Serialize(writer, "weaponMod");
                return;
            case TypeEnum.TypeWeaponPerkEnhanced:
                serializer.Serialize(writer, "weaponPerkEnhanced");
                return;
            default:
                throw new Exception("Cannot marshal type TypeEnum");
        }
    }
}