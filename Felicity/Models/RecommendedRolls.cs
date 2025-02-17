// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Felicity.Models;

public partial class NewWeaponRoll
{
    [JsonPropertyName("weaponRolls")]
    public List<WeaponRoll> WeaponRolls { get; set; }
}

public class WeaponRoll
{
    [JsonPropertyName("weaponHash")]
    public uint WeaponHash { get; set; }

    [JsonPropertyName("authorId")]
    public int AuthorId { get; set; }

    [JsonPropertyName("source")]
    public int Source { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("perks")]
    public List<uint> Perks { get; set; }

    [JsonPropertyName("canDrop")]
    public bool CanDrop { get; set; }
}

public partial class NewWeaponRoll
{
    public static NewWeaponRoll FromJson(string json)
    {
        return JsonSerializer.Deserialize<NewWeaponRoll>(json)!;
    }

    public static string ToJson(NewWeaponRoll self)
    {
        return JsonSerializer.Serialize(self);
    }
}

public class RecommendedRolls
{
    public List<Author>? Authors { get; set; }
    public List<Roll>? PvE { get; set; }
    public List<Roll>? PvP { get; set; }
}

public class Roll
{
    public int AuthorId { get; set; }
    public bool CanDrop { get; set; }
    public WeaponSource Source { get; set; }
    public string? Reason { get; set; }
    public string? WeaponName { get; set; }
    public uint WeaponId { get; set; }
    public List<uint> WeaponPerks { get; set; } = new();
}

public class Author
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public string? Url { get; set; }
}

public enum WeaponSource
{
    WorldDrop,
    LastWish,
    GardenOfSalvation,
    DeepStoneCrypt,
    VaultOfGlass,
    VowOfTheDisciple,
    GrandmasterNightfall,
    ShatteredThrone,
    PitOfHeresy,
    Prophecy,
    GraspOfAvarice,
    Duality,
    TrialsOfOsiris,
    Strikes,
    IronBanner,
    Crucible,
    Gambit,
    Moon,
    Europa,
    XurEternity,
    ThroneWorld,
    Leviathan,
    Event,
    KingsFall,
    Lightfall,
    RootOfNightmares,
    GhostsOfTheDeep,
    CrotasEnd,
    SalvationsEdge,
    WarlordsRuin,
    VespersHost,
    SunderedDoctrine,
    PaleHeart,
    Seasonal = 100,
    Unknown = 999
}

public static class ProcessRollData
{
    private const string JsonFile = "Data/weaponRolls.json";

    public static async Task<RecommendedRolls?> FromJsonAsync()
    {
        if (!File.Exists(JsonFile))
            return null;

        await using var stream = File.OpenRead(JsonFile);
        return await JsonSerializer.DeserializeAsync<RecommendedRolls?>(stream);
    }
}
