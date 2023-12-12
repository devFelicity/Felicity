namespace Felicity.Util.Enums;

internal enum EmblemCat : uint
{
    Seasonal = 2451657441,
    Account = 24961706,
    General = 1166184619,
    Competitive = 1801524334,
    Gambit = 4111024827,
    Strikes = 3958514834,
    World = 631010939,
    Trials = 2220993106,
    Raids = 329982304
}

internal static class EmblemCats
{
    public static readonly List<EmblemCat> EmblemCatList = new()
    {
        EmblemCat.Seasonal,
        EmblemCat.Account,
        EmblemCat.General,
        EmblemCat.Competitive,
        EmblemCat.Gambit,
        EmblemCat.Strikes,
        EmblemCat.World,
        EmblemCat.Trials,
        EmblemCat.Raids
    };
}
