// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Models;

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
    SeasonalHunt,
    SeasonalChosen,
    SeasonalSplicer,
    SeasonalLost,
    SeasonalRisen
}