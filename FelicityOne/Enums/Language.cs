// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace FelicityOne.Enums;

public enum Lang
{
    En,
    De,
    Es,
    Fr,
    It,
    Ja,
    Ko,
    Nl,
    Pl,
    PtBr,
    Ru,
    ZhChs,
    ZhCht
}

public static class EnumConverter
{
    public static string LangToString(Lang lang)
    {
        var language = lang switch
        {
            Lang.En => "en",
            Lang.De => "de",
            Lang.Es => "es",
            Lang.Fr => "fr",
            Lang.It => "it",
            Lang.Ja => "ja",
            Lang.Ko => "ko",
            Lang.Pl => "pl",
            Lang.PtBr => "pt-br",
            Lang.Ru => "ru",
            Lang.ZhChs => "zh-chs",
            Lang.ZhCht => "zh-cht",
            _ => "en"
        };

        return language;
    }
}