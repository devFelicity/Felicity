using Felicity.Helpers;

namespace Felicity.Enums;

internal static class Strings
{
    public static readonly string FelicityVersion = $"Felicity {ConfigHelper.GetBotSettings().Version:##.0}";

    public static string GetMementoImage(MementoType mementoName)
    {
        return mementoName switch
        {
            MementoType.Gambit => "https://bungie.net/common/destiny2_content/icons/045e66a538f70024c194b01a5cf8652a.jpg",
            MementoType.Trials => "https://bungie.net/common/destiny2_content/icons/c2e0148851bd8aec5d04d413b897dcbd.jpg",
            MementoType.Nightfall => "https://bungie.net/common/destiny2_content/icons/bf21c13f03a29aa0067f85c84593a594.jpg",
            _ => ""
        };
    }
}

internal static class Images
{
    public const string FelicityLogo = "https://whaskell.pw/images/felicity_circle.jpg";

    public const string ModVendorIcon = "https://bungie.net/common/destiny2_content/icons/23599621d4c63076c647384028d96ca4.png";

    public const string XurVendorLogo = "https://www.bungie.net/img/destiny_content/vendor/icons/xur_large_icon.png";

    public const string SaintVendorLogo = "https://bungie.net/common/destiny2_content/icons/c3cb40c2b36cccd2f6cf462f14c89736.png";
}