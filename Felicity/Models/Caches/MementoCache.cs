using System.Text.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Models.Caches;

public class MementoCache
{
    public MementoInventoryElement[]? MementoInventory { get; set; }

    public class MementoInventoryElement
    {
        public MementoSource Source { get; set; }
        public MementoWeaponList[]? WeaponList { get; set; }
    }

    public class MementoWeaponList
    {
        public string? WeaponName { get; set; }
        public MementoTypeList[]? TypeList { get; set; }
    }

    public class MementoTypeList
    {
        public MementoType Type { get; set; }
        public Memento? Memento { get; set; }
    }

    public class Memento
    {
        public string? Credit { get; set; }
        public string? ImageUrl { get; set; }
    }
}

public static class ProcessMementoData
{
    private const string FilePath = "Data/mementoCache.json";

    public static async Task<MementoCache?> ReadJsonAsync()
    {
        await using var stream = File.OpenRead(FilePath);
        return await JsonSerializer.DeserializeAsync<MementoCache>(stream);
    }
}

public enum MementoType
{
    Gambit,
    Nightfall,
    Trials
}

public enum MementoSource
{
    OpenWorld,
    RaidVotD,
    SeasonRisen,
    SeasonHaunted,
    ThroneWorld
}