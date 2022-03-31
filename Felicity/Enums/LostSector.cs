// ReSharper disable UnusedMember.Global

namespace Felicity.Enums
{
    public enum LostSector
    {
        VelesLabyrinth,
        ExodusGarden,
        AphelionsRest,
        BayDrownedWishes,
        ChamberStarlight,
        K1Revelation,
        K1CrewQuarters,
        K1Logistics,
        Metamorphosis,
        Sepulcher,
        Extraction
    }

    public enum LostSectorDifficulty
    {
        Legend,
        Master
    }

    public enum LostSectorReward
    {
        Helmet,
        Legs,
        Arms,
        Chest
    }

    public class LostSectorInfo
    {
        public string GetName(LostSector ls)
        {
            return ls switch
            {
                LostSector.VelesLabyrinth => "Veles Labyrinth",
                LostSector.ExodusGarden => "Exodus Garden 2A",
                LostSector.AphelionsRest => "Aphelion's Rest",
                LostSector.BayDrownedWishes => "Bay of Drowned Wishes",
                LostSector.ChamberStarlight => "Chamber of Starlight",
                LostSector.K1Revelation => "K1 Revelation",
                LostSector.K1CrewQuarters => "K1 Crew Quarters",
                LostSector.K1Logistics => "K1 Logistics",
                LostSector.Metamorphosis => "Metamorphosis",
                LostSector.Sepulcher => "Sepulcher",
                LostSector.Extraction => "Extraction",
                _ => ""
            };
        }

        public string GetLocation(LostSector ls)
        {
            return ls switch
            {
                LostSector.VelesLabyrinth => "Forgotten Shore, Cosmodrome",
                LostSector.ExodusGarden => "The Divide, Cosmodrome",
                LostSector.AphelionsRest => "The Strand, The Dreaming City",
                LostSector.BayDrownedWishes => "Divalian Mists, The Dreaming City",
                LostSector.ChamberStarlight => "Rheasilvia, The Dreaming City",
                LostSector.K1Revelation => "Sorrow's Harbor, The Moon",
                LostSector.K1CrewQuarters => "Hellmouth, The Moon",
                LostSector.K1Logistics => "Archer's Line, The Moon",
                LostSector.Metamorphosis => "Miasma, Savathûn's Throne World",
                LostSector.Sepulcher => "Florescent Canal, Savathûn's Throne World",
                LostSector.Extraction => "Quagmire, Savathûn's Throne World",
                _ => ""
            };
        }

        public static string GetBossName(LostSector ls)
        {
            return ls switch
            {
                LostSector.VelesLabyrinth => "Deksis-5, Taskmaster",
                LostSector.ExodusGarden => "",
                LostSector.BayDrownedWishes => "",
                LostSector.ChamberStarlight => "",
                LostSector.AphelionsRest => "Ur Haraak, Disciple of Quiria",
                LostSector.K1Revelation => "Nightmare of Arguth, The Tormented",
                LostSector.K1CrewQuarters => "Nightmare of Reyiks, Actuator",
                LostSector.K1Logistics => "Nightmare of Kelniks Reborn",
                LostSector.Metamorphosis => "",
                LostSector.Sepulcher => "",
                LostSector.Extraction => "",
                _ => ""
            };
        }

        public static string GetImageURL(LostSector ls)
        {
            return ls switch
            {
                LostSector.VelesLabyrinth => "",
                LostSector.ExodusGarden => "",
                LostSector.BayDrownedWishes => "",
                LostSector.ChamberStarlight => "",
                LostSector.AphelionsRest => "https://www.bungie.net/img/destiny_content/pgcr/dreaming_city_aphelions_rest.jpg",
                LostSector.K1Revelation => "https://www.bungie.net/img/destiny_content/pgcr/moon_k1_revelation.jpg",
                LostSector.K1CrewQuarters => "https://www.bungie.net/img/destiny_content/pgcr/moon_k1_crew_quarters.jpg",
                LostSector.K1Logistics => "https://www.bungie.net/img/destiny_content/pgcr/moon_k1_logistics.jpg",
                LostSector.Metamorphosis => "",
                LostSector.Sepulcher => "",
                LostSector.Extraction => "",
                _ => ""
            };
        }

        public string GetChampions(LostSector ls, LostSectorDifficulty lsd)
        {
            return lsd switch
            {
                LostSectorDifficulty.Legend => ls switch
                {
                    LostSector.VelesLabyrinth => "",
                    LostSector.ExodusGarden => "",
                    LostSector.AphelionsRest => "",
                    LostSector.BayDrownedWishes => "",
                    LostSector.ChamberStarlight => "",
                    LostSector.K1Revelation => "",
                    LostSector.K1CrewQuarters => "",
                    LostSector.K1Logistics => "",
                    LostSector.Metamorphosis => "",
                    LostSector.Sepulcher => "",
                    LostSector.Extraction => "",
                    _ => ""
                },
                LostSectorDifficulty.Master => ls switch
                {
                    LostSector.VelesLabyrinth => "",
                    LostSector.ExodusGarden => "",
                    LostSector.AphelionsRest => "",
                    LostSector.BayDrownedWishes => "",
                    LostSector.ChamberStarlight => "",
                    LostSector.K1Revelation => "",
                    LostSector.K1CrewQuarters => "",
                    LostSector.K1Logistics => "",
                    LostSector.Metamorphosis => "",
                    LostSector.Sepulcher => "",
                    LostSector.Extraction => "",
                    _ => ""
                },
                _ => ""
            };
        }

        public string GetShields(LostSector ls, LostSectorDifficulty lsd)
        {
            return lsd switch
            {
                LostSectorDifficulty.Legend => ls switch
                {
                    LostSector.VelesLabyrinth => "",
                    LostSector.ExodusGarden => "",
                    LostSector.AphelionsRest => "",
                    LostSector.BayDrownedWishes => "",
                    LostSector.ChamberStarlight => "",
                    LostSector.K1Revelation => "",
                    LostSector.K1CrewQuarters => "",
                    LostSector.K1Logistics => "",
                    LostSector.Metamorphosis => "",
                    LostSector.Sepulcher => "",
                    LostSector.Extraction => "",
                    _ => ""
                },
                LostSectorDifficulty.Master => ls switch
                {
                    LostSector.VelesLabyrinth => "",
                    LostSector.ExodusGarden => "",
                    LostSector.AphelionsRest => "",
                    LostSector.BayDrownedWishes => "",
                    LostSector.ChamberStarlight => "",
                    LostSector.K1Revelation => "",
                    LostSector.K1CrewQuarters => "",
                    LostSector.K1Logistics => "",
                    LostSector.Metamorphosis => "",
                    LostSector.Sepulcher => "",
                    LostSector.Extraction => "",
                    _ => ""
                },
                _ => ""
            };
        }

        public string GetModifiers(LostSector ls, LostSectorDifficulty lsd)
        {
            return lsd switch
            {
                LostSectorDifficulty.Legend => ls switch
                {
                    LostSector.VelesLabyrinth => "",
                    LostSector.ExodusGarden => "",
                    LostSector.AphelionsRest => "",
                    LostSector.BayDrownedWishes => "",
                    LostSector.ChamberStarlight => "",
                    LostSector.K1Revelation => "",
                    LostSector.K1CrewQuarters => "",
                    LostSector.K1Logistics => "",
                    LostSector.Metamorphosis => "",
                    LostSector.Sepulcher => "",
                    LostSector.Extraction => "",
                    _ => ""
                },
                LostSectorDifficulty.Master => ls switch
                {
                    LostSector.VelesLabyrinth => "",
                    LostSector.ExodusGarden => "",
                    LostSector.AphelionsRest => "",
                    LostSector.BayDrownedWishes => "",
                    LostSector.ChamberStarlight => "",
                    LostSector.K1Revelation => "",
                    LostSector.K1CrewQuarters => "",
                    LostSector.K1Logistics => "",
                    LostSector.Metamorphosis => "",
                    LostSector.Sepulcher => "",
                    LostSector.Extraction => "",
                    _ => ""
                },
                _ => ""
            };
        }
    }
}
