using DotNetBungieAPI.HashReferences;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Util.Enums;

public class LootTableDefinition
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public ActivityType? ActivityType { get; init; }
    public List<LootTable>? Loot { get; init; }
}

public class LootTable
{
    public Encounter EncounterType { get; init; }
    public string? EncounterName { get; init; }
    public List<uint>? LootIds { get; init; }
}

public enum ActivityType
{
    Dungeon,
    Raid
}

public enum Armor
{
    Helmet,
    Gloves,
    Chest,
    Boots,
    Class,
    Everything,
    None
}

public enum Encounter
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Boss
}

public static class LootTables
{
    public static readonly List<LootTableDefinition> KnownTables = new()
    {
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Root of Nightmares",
            Description = "A sinister threat has taken root.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Survive the Onslaught",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.BriarsContempt_1491665733,
                        DefinitionHashes.InventoryItems.KoraxissDistress_2972949637,
                        DefinitionHashes.InventoryItems.NessasOblation_135029084
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Enter the Root",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.AcasiasDejection_1471212226,
                        DefinitionHashes.InventoryItems.KoraxissDistress_2972949637,
                        DefinitionHashes.InventoryItems.MykelsReverence_231031173,
                        DefinitionHashes.InventoryItems.NessasOblation_135029084
                    }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Zo'aurc, Explicator of Planets",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Chest, (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.AcasiasDejection_1471212226,
                        DefinitionHashes.InventoryItems.KoraxissDistress_2972949637,
                        DefinitionHashes.InventoryItems.MykelsReverence_231031173,
                        DefinitionHashes.InventoryItems.RufussFury_484515708
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Nezarec, Final God of Pain",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.ConditionalFinality,
                        DefinitionHashes.InventoryItems.AcasiasDejection_1471212226,
                        DefinitionHashes.InventoryItems.KoraxissDistress_2972949637,
                        DefinitionHashes.InventoryItems.MykelsReverence_231031173,
                        DefinitionHashes.InventoryItems.NessasOblation_135029084,
                        DefinitionHashes.InventoryItems.RufussFury_484515708
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "King's Fall", Description = "Long live the King...",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Gate",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.DoomofChelchis_1937552980
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Totems",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Chest, (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.DoomofChelchis_1937552980,
                        DefinitionHashes.InventoryItems.QullimsTerminus_1321506184
                    }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Warpriest",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.SmiteofMerain_2221264583,
                        DefinitionHashes.InventoryItems.DefianceofYasmin_3228096719
                    }
                },
                new()
                {
                    EncounterType = Encounter.Fourth, EncounterName = "Golgoroth",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.QullimsTerminus_1321506184,
                        DefinitionHashes.InventoryItems.ZaoulisBane_431721920,
                        DefinitionHashes.InventoryItems.MidhasReckoning_3969066556
                    }
                },
                new()
                {
                    EncounterType = Encounter.Fifth, EncounterName = "Daughters",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.QullimsTerminus_1321506184,
                        DefinitionHashes.InventoryItems.SmiteofMerain_2221264583,
                        DefinitionHashes.InventoryItems.ZaoulisBane_431721920
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Oryx, The Taken King",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet,
                        DefinitionHashes.InventoryItems.MidhasReckoning_3969066556,
                        DefinitionHashes.InventoryItems.TouchofMalice_1802135586
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Vow of the Disciple", Description = "The disciple beckons...",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Acquisition",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Chest, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.Deliverance_768621510,
                        DefinitionHashes.InventoryItems.Submission_3886416794,
                        DefinitionHashes.InventoryItems.Cataclysmic_999767358
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Collection",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Submission_3886416794,
                        DefinitionHashes.InventoryItems.Forbearance_613334176,
                        DefinitionHashes.InventoryItems.Insidious_3428521585,
                        DefinitionHashes.InventoryItems.Cataclysmic_999767358
                    }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Exhibition",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Chest, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.Deliverance_768621510,
                        DefinitionHashes.InventoryItems.Submission_3886416794
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Dominion",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.CollectiveObligation,
                        DefinitionHashes.InventoryItems.Forbearance_613334176,
                        DefinitionHashes.InventoryItems.Insidious_3428521585,
                        DefinitionHashes.InventoryItems.LubraesRuin_2534546147
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Vault of Glass", Description =
                "Beneath Venus, evil stirs...\n\n" +
                "Timelost weapons require **Master** challenge completion on that encounter while VoG is the active rotator raid.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Confluxes",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.FoundVerdict_3197270240,
                        DefinitionHashes.InventoryItems.CorrectiveMeasure_471518543,
                        DefinitionHashes.InventoryItems.VisionofConfluence_3186018373,
                        DefinitionHashes.InventoryItems.VisionofConfluenceTimelost_690668916
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Oracles",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.PraedythsRevenge_3653573172,
                        DefinitionHashes.InventoryItems.FoundVerdict_3197270240,
                        DefinitionHashes.InventoryItems.VisionofConfluence_3186018373,
                        DefinitionHashes.InventoryItems.PraedythsRevengeTimelost_1987769101
                    }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "The Templar",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.Fatebringer_2171478765,
                        DefinitionHashes.InventoryItems.VisionofConfluence_3186018373,
                        DefinitionHashes.InventoryItems.CorrectiveMeasure_471518543,
                        DefinitionHashes.InventoryItems.FatebringerTimelost_1216319404
                    }
                },
                new()
                {
                    EncounterType = Encounter.Fourth, EncounterName = "The Gatekeepers",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.Fatebringer_2171478765,
                        DefinitionHashes.InventoryItems.FoundVerdict_3197270240,
                        DefinitionHashes.InventoryItems.HezenVengeance_4050645223,
                        DefinitionHashes.InventoryItems.HezenVengeanceTimelost_1921159786
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Atheon, Time's Conflux",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.VexMythoclast,
                        DefinitionHashes.InventoryItems.PraedythsRevenge_3653573172,
                        DefinitionHashes.InventoryItems.CorrectiveMeasure_471518543,
                        DefinitionHashes.InventoryItems.HezenVengeance_4050645223,
                        DefinitionHashes.InventoryItems.CorrectiveMeasureTimelost_3796510434
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Deep Stone Crypt",
            Description = "The chains of legacy must be broken.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Crypt Security",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Trustee_1392919471
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Atraks-1",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Heritage_4248569242,
                        DefinitionHashes.InventoryItems.Succession_2990047042
                    }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Taniks, Reborn",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Heritage_4248569242,
                        DefinitionHashes.InventoryItems.Posterity_3281285075
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Taniks, The Abomination",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Chest, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.EyesofTomorrow,
                        DefinitionHashes.InventoryItems.Bequest_3366545721,
                        DefinitionHashes.InventoryItems.Commemoration_4230965989
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Garden of Salvation",
            Description = "The Garden calls out to you.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Evade the Consecrated Mind",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.AccruedRedemption,
                        DefinitionHashes.InventoryItems.ZealotsReward
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Summon the Consecrated Mind",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves,
                        DefinitionHashes.InventoryItems.ProphetofDoom,
                        DefinitionHashes.InventoryItems.RecklessOracle
                    }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Defeat the Consecrated Mind",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.SacredProvenance,
                        DefinitionHashes.InventoryItems.AncientGospel
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "The Sanctified Mind",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.OmniscientEye
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Last Wish", Description = "The opportunity of a lifetime.\n\n" +
                "Last Wish drops are unique in that anything can drop from anywhere except key items shown below.\n" +
                "Weapons indicated here are curated roll drops.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Kalli, The Corrupted",
                    LootIds = new List<uint> { DefinitionHashes.InventoryItems.AgeOldBond_601592879 }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Shuro Chi, The Corrupted",
                    LootIds = new List<uint> { DefinitionHashes.InventoryItems.Transfiguration_3799980700 }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Morgeth, The Spirekeeper",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.NationofBeasts_654370424,
                        DefinitionHashes.InventoryItems.CleansingKnife
                    }
                },
                new()
                {
                    EncounterType = Encounter.Fourth, EncounterName = "The Vault",
                    LootIds = new List<uint> { DefinitionHashes.InventoryItems.TyrannyofHeaven_2721249463 }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Riven of a Thousand Voices",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.ChatteringBone_568515759,
                        DefinitionHashes.InventoryItems.GlitteringKey
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Dungeon, Name = "Ghosts of the Deep",
            Description = "fish go brrr or something idk",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Hive Ritual",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.NoSurvivors_3262192268,
                        DefinitionHashes.InventoryItems.NewPacificEpitaph,
                        DefinitionHashes.InventoryItems.ColdComfort_839786290
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Ecthar, Shield of Savathûn",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.GreasyLuck,
                        DefinitionHashes.InventoryItems.ColdComfort_839786290
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Simmumah ur-Nokru",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.TheNavigator,
                        DefinitionHashes.InventoryItems.NoSurvivors_3262192268,
                        DefinitionHashes.InventoryItems.NewPacificEpitaph,
                        DefinitionHashes.InventoryItems.ColdComfort_839786290,
                        DefinitionHashes.InventoryItems.GreasyLuck,
                        (uint)Armor.Everything
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Dungeon, Name = "Spire of the Watcher",
            Description = "Machinations run wild in this dust-ridden ruin. Bring them to heel.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Ascend the Spire",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.LongArm,
                        DefinitionHashes.InventoryItems.SeventhSeraphCarbine_4070357005,
                        DefinitionHashes.InventoryItems.TerminusHorizon
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Silence the Spire",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.SeventhSeraphOfficerRevolver_1555959830,
                        DefinitionHashes.InventoryItems.TerminusHorizon
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Persys, Primordial Ruin",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.HierarchyofNeeds,
                        DefinitionHashes.InventoryItems.LiminalVigil,
                        DefinitionHashes.InventoryItems.LongArm,
                        DefinitionHashes.InventoryItems.SeventhSeraphCarbine_4070357005,
                        DefinitionHashes.InventoryItems.SeventhSeraphOfficerRevolver_1555959830,
                        DefinitionHashes.InventoryItems.TerminusHorizon,
                        DefinitionHashes.InventoryItems.Wilderflight,
                        (uint)Armor.Everything
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Dungeon, Name = "Duality",
            Description = "Dive into the depths of the exiled emperor's mind in search of dark secrets.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Sorrow Bearer",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Boots,
                        DefinitionHashes.InventoryItems.LingeringDread,
                        DefinitionHashes.InventoryItems.TheEpicurean_2263839058
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "The Vault",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Unforgiven,
                        DefinitionHashes.InventoryItems.Stormchaser
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Calus' Greatest Shame",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.Heartshadow,
                        DefinitionHashes.InventoryItems.NewPurpose,
                        DefinitionHashes.InventoryItems.FixedOdds_1642384931,
                        (uint)Armor.Everything
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Dungeon, Name = "Grasp of Avarice",
            Description = "A cautionary tale for adventurers willing to trade their humanity for riches.",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Phry'zia, The Insatiable",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Matador64
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Sunken Lair",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves, (uint)Armor.Chest,
                        DefinitionHashes.InventoryItems.HeroofAges
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Avarokk, The Covetous",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.Eyasluna,
                        DefinitionHashes.InventoryItems.H1000YardStare,
                        (uint)Armor.Everything
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Dungeon, Name = "Prophecy",
            Description = "Enter the realm of the Nine and ask the question: \"What is the nature of the Darkness?\"",
            Loot = new List<LootTable>
            {
                new()
                {
                    EncounterType = Encounter.First, EncounterName = "Phalanx Echo",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.Judgment_1476654960,
                        DefinitionHashes.InventoryItems.TheLongWalk_3326850591
                    }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "The Cube",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Gloves,
                        DefinitionHashes.InventoryItems.ASwiftVerdict_1626503676,
                        DefinitionHashes.InventoryItems.TheLastBreath_507038823
                    }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Kell Echo",
                    LootIds = new List<uint>
                    {
                        (uint)Armor.Helmet, (uint)Armor.Gloves, (uint)Armor.Chest, (uint)Armor.Boots, (uint)Armor.Class,
                        DefinitionHashes.InventoryItems.DarkestBefore_2481758391,
                        DefinitionHashes.InventoryItems.ASuddenDeath_2855157553
                    }
                }
            }
        }
    };
}