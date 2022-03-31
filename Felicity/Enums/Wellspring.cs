// ReSharper disable UnusedMember.Global

using Serilog;

namespace Felicity.Enums
{
    public enum Wellspring
    {
        Golmag,
        Vezuul,
        Borgong,
        Zeerik
    }

    internal class WellspringInfo
    {
        public string GetBossName(Wellspring boss)
        {
            return boss switch
            {
                Wellspring.Golmag => "Golmag, Warden of the Spring",
                Wellspring.Vezuul => "Vezuul, Lightflayer",
                Wellspring.Borgong => "Bor'gong, Warden of the Spring",
                Wellspring.Zeerik => "Zeerik, Lightflayer",
                _ => ""
            };
        }

        public string GetActivityType(Wellspring boss)
        {
            return boss switch
            {
                Wellspring.Golmag => "Attack",
                Wellspring.Vezuul => "Defend",
                Wellspring.Borgong => "Attack",
                Wellspring.Zeerik => "Defend",
                _ => ""
            };
        }

        public string GetWeaponName(Wellspring boss)
        {
            return boss switch
            {
                Wellspring.Golmag => "Come to Pass",
                Wellspring.Vezuul => "Tarnation",
                Wellspring.Borgong => "Fel Taradiddle",
                Wellspring.Zeerik => "Father's Sins",
                _ => ""
            };
        }

        public static string GetWeaponTypeString(Wellspring boss)
        {
            return boss switch
            {
                Wellspring.Golmag => "Auto Rifle",
                Wellspring.Vezuul => "Grenade Launcher",
                Wellspring.Borgong => "Bow",
                Wellspring.Zeerik => "Sniper",
                _ => ""
            };
        }
    }
}