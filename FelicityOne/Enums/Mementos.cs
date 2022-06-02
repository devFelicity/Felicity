using FelicityOne.Helpers;
using J = Newtonsoft.Json.JsonPropertyAttribute;

#pragma warning disable CS8618

namespace FelicityOne.Enums;

public class Mementos
{
    [J("MementoList")] public MementoList MementoList { get; set; }
}

public class MementoList
{
    [J("gambit")] public MementoCat[] Gambit { get; set; }
    [J("trials")] public MementoCat[] Trials { get; set; }
    [J("nightfall")] public MementoCat[] Nightfall { get; set; }
}

public class MementoCat
{
    [J("name")] public string Name { get; set; }
    [J("url")] public string Url { get; set; }
    [J("author")] public string Author { get; set; }
    [J("source")] public MementoSource Source { get; set; }
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

public static class KnownMementos
{
    public static Mementos KnownMementoList { get; private set; }

    public static void PopulateMementos()
    {
        KnownMementoList = File.Exists("Data/mementoCache.json")
            ? ConfigHelper.FromJson<Mementos>(File.ReadAllText("Data/mementoCache.json"))!
            : new Mementos();
    }
}