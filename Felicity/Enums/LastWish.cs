// ReSharper disable UnusedMember.Global

using System;

namespace Felicity.Enums;

public enum LastWishEncounter
{
    Kalli,
    ShuroChi,
    Morgeth,
    Vault,
    Riven
}

internal class LastWish
{
    public string Title => "Last Wish";

    public string Description =>
        "Put an end to the Taken curse within the Dreaming City through killing Riven of a Thousand Voices.";

    public string Image => "https://www.bungie.net/img/destiny_content/pgcr/raid_beanstalk.jpg";

    public string Icon =>
        "https://www.bungie.net/common/destiny2_content/icons/fc5791eb2406bf5e6b361f3d16596693.png";

    public static string GetEncounterName(LastWishEncounter Encounter) 
        => Enum.GetName(typeof(LastWishEncounter), Encounter);

    public static string GetChallengeName(LastWishEncounter Encounter)
    {
        return Encounter switch
        {
            LastWishEncounter.Kalli => "Summoning Ritual",
            LastWishEncounter.ShuroChi => "Which Witch",
            LastWishEncounter.Morgeth => "Forever Fight",
            LastWishEncounter.Vault => "Keep Out",
            LastWishEncounter.Riven => "Strength of Memory",
            _ => ""
        };
    }

    public static string GetChallengeDescription(LastWishEncounter Encounter)
    {
        return Encounter switch
        {
            LastWishEncounter.Kalli =>
                "During the Kalli Fight, players must capture all of nine plates and kill the Taken Ogres that spawn.",
            LastWishEncounter.ShuroChi =>
                "During the Shuro Chi fight, players must avoid Shuro Chi's ranged attack.",
            LastWishEncounter.Morgeth =>
                "During the Morgeth fight, players must not defeat any ogres, besides Morgeth.",
            LastWishEncounter.Vault =>
                "During the Vault encounter, players must prevent all \"Might of Riven\" enemies from entering the center room.",
            LastWishEncounter.Riven =>
                "During the Riven fight, players must not shoot the same eye more than once.",
            _ => ""
        };
    }
}