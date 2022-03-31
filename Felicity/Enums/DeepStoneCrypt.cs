// ReSharper disable UnusedMember.Global

namespace Felicity.Enums
{
    public enum DeepStoneCryptEncounter
    {
        Security,
        Atraks,
        Descent,
        Taniks
    }

    internal class DeepStoneCrypt
    {
        public string Title => "Deep Stone Crypt";

        public string Description =>
            "Purge the House of Salvation from the Deep Stone Crypt.";

        public string Image => "https://www.bungie.net/img/destiny_content/pgcr/europa-raid-deep-stone-crypt.jpg";

        public string Icon =>
            "https://www.bungie.net/common/destiny2_content/icons/f71c1a6ab05d2c287352c8ee0aae644e.png";

        public static string GetEncounterName(DeepStoneCryptEncounter Encounter)
        {
            return Encounter switch
            {
                DeepStoneCryptEncounter.Security => "Crypt Security",
                DeepStoneCryptEncounter.Atraks => "Atraks-1",
                DeepStoneCryptEncounter.Descent => "The Descent",
                DeepStoneCryptEncounter.Taniks => "Taniks",
                _ => ""
            };
        }

        public static string GetChallengeString(DeepStoneCryptEncounter Encounter)
        {
            return Encounter switch
            {
                DeepStoneCryptEncounter.Security => "Red Rover",
                DeepStoneCryptEncounter.Atraks => "Copies of Copies",
                DeepStoneCryptEncounter.Descent => "Of All Trades",
                DeepStoneCryptEncounter.Taniks => "The Core Four",
                _ => ""
            };
        }

        public static string GetChallengeDescriptionString(DeepStoneCryptEncounter Encounter)
        {
            return Encounter switch
            {
                DeepStoneCryptEncounter.Security =>
                    "During the Crypt Security encounter, while having the Operator Augment, each player must shoot two panels. This requires three phases.",
                DeepStoneCryptEncounter.Atraks =>
                    "During the Atraks-1 fight, players must not open any airlocks on the top level.",
                DeepStoneCryptEncounter.Descent =>
                    "During the Descent encounter, players must use each Augment once.",
                DeepStoneCryptEncounter.Taniks =>
                    "During the Taniks fight, players must deposit four Nuclear Cores in the same phase.",
                _ => ""
            };
        }
    }
}