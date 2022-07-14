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
    Class
}

public enum Encounter
{
    First,
    Second,
    Third,
    Fourth,
    Boss
}

public static class LootTables
{
    public static readonly List<LootTableDefinition> KnownTables = new()
    {
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
                        DefinitionHashes.InventoryItems.Forbearance_613334176,
                        DefinitionHashes.InventoryItems.Insidious_3428521585,
                        DefinitionHashes.InventoryItems.LubraesRuin_2534546147
                    }
                }
            }
        },
        new LootTableDefinition
        {
            ActivityType = ActivityType.Raid, Name = "Vault of Glass", Description = "Beneath Venus, evil stirs...\n\n" +
                "Timelost weapons require challenge completion on that encounter while VoG is the active rotator raid.",
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
                    LootIds = new List<uint> { DefinitionHashes.InventoryItems.AgeOldBond }
                },
                new()
                {
                    EncounterType = Encounter.Second, EncounterName = "Shuro Chi, The Corrupted",
                    LootIds = new List<uint> { DefinitionHashes.InventoryItems.Transfiguration }
                },
                new()
                {
                    EncounterType = Encounter.Third, EncounterName = "Morgeth, The Spirekeeper",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.NationofBeasts,
                        DefinitionHashes.InventoryItems.CleansingKnife
                    }
                },
                new()
                {
                    EncounterType = Encounter.Fourth, EncounterName = "The Vault",
                    LootIds = new List<uint> { DefinitionHashes.InventoryItems.TyrannyofHeaven }
                },
                new()
                {
                    EncounterType = Encounter.Boss, EncounterName = "Riven of a Thousand Voices",
                    LootIds = new List<uint>
                    {
                        DefinitionHashes.InventoryItems.ChatteringBone,
                        DefinitionHashes.InventoryItems.GlitteringKey
                    }
                }
            }
        }
    };
}