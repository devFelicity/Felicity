using DotNetBungieAPI.HashReferences;

namespace Felicity.Util.Enums;

internal static class Craftables
{
    public static readonly Dictionary<string, List<uint>> CraftableList = new()
    {
        {
            CraftSource.KingsFall, new List<uint>
            {
                DefinitionHashes.Records.DefianceofYasmin,
                DefinitionHashes.Records.DoomofChelchis,
                DefinitionHashes.Records.MidhasReckoning,
                DefinitionHashes.Records.QullimsTerminus,
                DefinitionHashes.Records.SmiteofMerain,
                DefinitionHashes.Records.ZaoulisBane
            }
        },
        {
            CraftSource.SeasonPlunder, new List<uint>
            {
                DefinitionHashes.Records.BloodFeud,
                DefinitionHashes.Records.BrigandsLaw,
                DefinitionHashes.Records.NoReprieve,
                DefinitionHashes.Records.PlancksStride,
                DefinitionHashes.Records.SailspyPitchglass,
                DefinitionHashes.Records.TarnishedMettle
            }
        },
        {
            CraftSource.Anniversary, new List<uint>
            {
                DefinitionHashes.Records.BxR55Battler,
                DefinitionHashes.Records.HalfTruths,
                DefinitionHashes.Records.PardonOurDust,
                DefinitionHashes.Records.RetracedPath,
                DefinitionHashes.Records.TheOtherHalf,
                DefinitionHashes.Records.WastelanderM5
            }
        },
        {
            CraftSource.Unknown, new List<uint>
            {
                DefinitionHashes.Records.AmmitAR2,
                DefinitionHashes.Records.ImperialDecree,
                DefinitionHashes.Records.Taipan4fr,
                DefinitionHashes.Records.Goldtusk,
                DefinitionHashes.Records.DeathsRazor,
                DefinitionHashes.Records.ThroneCleaver
            }
        },
        {
            CraftSource.SeasonHaunted, new List<uint>
            {
                DefinitionHashes.Records.BumpintheNight,
                DefinitionHashes.Records.Firefright,
                DefinitionHashes.Records.HollowDenial,
                DefinitionHashes.Records.NezarecsWhisper,
                DefinitionHashes.Records.TearsofContrition,
                DefinitionHashes.Records.WithoutRemorse
            }
        },
        {
            CraftSource.Opulent, new List<uint>
            {
                DefinitionHashes.Records.Austringer,
                DefinitionHashes.Records.Beloved,
                DefinitionHashes.Records.CALUSMiniTool,
                DefinitionHashes.Records.DrangBaroque,
                DefinitionHashes.Records.FixedOdds,
                DefinitionHashes.Records.TheEpicurean
            }
        },
        {
            CraftSource.SeasonRisen, new List<uint>
            {
                DefinitionHashes.Records.ExplosivePersonality,
                DefinitionHashes.Records.PieceofMind,
                DefinitionHashes.Records.RecurrentImpact,
                DefinitionHashes.Records.SweetSorrow,
                DefinitionHashes.Records.Thoughtless,
                DefinitionHashes.Records.UnderYourSkin
            }
        },
        {
            CraftSource.RaidVotD, new List<uint>
            {
                DefinitionHashes.Records.Cataclysmic,
                DefinitionHashes.Records.Deliverance,
                DefinitionHashes.Records.Forbearance,
                DefinitionHashes.Records.Insidious,
                DefinitionHashes.Records.LubraesRuin,
                DefinitionHashes.Records.Submission
            }
        },
        {
            CraftSource.WqWellspring, new List<uint>
            {
                DefinitionHashes.Records.CometoPass,
                DefinitionHashes.Records.EdgeofAction,
                DefinitionHashes.Records.EdgeofConcurrence,
                DefinitionHashes.Records.EdgeofIntent,
                DefinitionHashes.Records.FathersSins,
                DefinitionHashes.Records.FelTaradiddle,
                DefinitionHashes.Records.Tarnation
            }
        },
        {
            CraftSource.Wq, new List<uint>
            {
                DefinitionHashes.Records.EmpiricalEvidence,
                DefinitionHashes.Records.ForensicNightmare,
                DefinitionHashes.Records.LikelySuspect,
                DefinitionHashes.Records.OsteoStriga,
                DefinitionHashes.Records.PalmyraB,
                DefinitionHashes.Records.PointedInquiry,
                DefinitionHashes.Records.RagnhildD,
                DefinitionHashes.Records.RedHerring,
                DefinitionHashes.Records.Syncopation53,
                DefinitionHashes.Records.TheEnigma
            }
        }
    };

    public static uint GetWeaponId(uint recordDefinitionHash)
    {
        return recordDefinitionHash switch
        {
            DefinitionHashes.Records.BumpintheNight => DefinitionHashes.InventoryItems.BumpintheNight_1959650777,
            DefinitionHashes.Records.Firefright => DefinitionHashes.InventoryItems.Firefright_2778013407,
            DefinitionHashes.Records.HollowDenial => DefinitionHashes.InventoryItems.HollowDenial_2323544076,
            DefinitionHashes.Records.NezarecsWhisper => DefinitionHashes.InventoryItems.NezarecsWhisper_254636484,
            DefinitionHashes.Records.TearsofContrition => DefinitionHashes.InventoryItems.TearsofContrition_1366394399,
            DefinitionHashes.Records.WithoutRemorse => DefinitionHashes.InventoryItems.WithoutRemorse_1478986057,
            DefinitionHashes.Records.Austringer => DefinitionHashes.InventoryItems.Austringer_3055790362,
            DefinitionHashes.Records.Beloved => DefinitionHashes.InventoryItems.Beloved_3107853529,
            DefinitionHashes.Records.CALUSMiniTool => DefinitionHashes.InventoryItems.CALUSMiniTool_2490988246,
            DefinitionHashes.Records.DrangBaroque => DefinitionHashes.InventoryItems.DrangBaroque_502356570,
            DefinitionHashes.Records.FixedOdds => DefinitionHashes.InventoryItems.FixedOdds_2194955522,
            DefinitionHashes.Records.TheEpicurean => DefinitionHashes.InventoryItems.TheEpicurean_2263839058,
            DefinitionHashes.Records.ExplosivePersonality => DefinitionHashes.InventoryItems
                .ExplosivePersonality_4096943616,
            DefinitionHashes.Records.PieceofMind => DefinitionHashes.InventoryItems.PieceofMind_2097055732,
            DefinitionHashes.Records.RecurrentImpact => DefinitionHashes.InventoryItems.RecurrentImpact_1572896086,
            DefinitionHashes.Records.SweetSorrow => DefinitionHashes.InventoryItems.SweetSorrow_1248372789,
            DefinitionHashes.Records.Thoughtless => DefinitionHashes.InventoryItems.Thoughtless_4067556514,
            DefinitionHashes.Records.UnderYourSkin => DefinitionHashes.InventoryItems.UnderYourSkin_232928045,
            DefinitionHashes.Records.Cataclysmic => DefinitionHashes.InventoryItems.Cataclysmic_999767358,
            DefinitionHashes.Records.Deliverance => DefinitionHashes.InventoryItems.Deliverance_768621510,
            DefinitionHashes.Records.Forbearance => DefinitionHashes.InventoryItems.Forbearance_613334176,
            DefinitionHashes.Records.Insidious => DefinitionHashes.InventoryItems.Insidious_3428521585,
            DefinitionHashes.Records.LubraesRuin => DefinitionHashes.InventoryItems.LubraesRuin_2534546147,
            DefinitionHashes.Records.Submission => DefinitionHashes.InventoryItems.Submission_3886416794,
            DefinitionHashes.Records.CometoPass => DefinitionHashes.InventoryItems.CometoPass_927567426,
            DefinitionHashes.Records.EdgeofAction => DefinitionHashes.InventoryItems.EdgeofAction_2535142413,
            DefinitionHashes.Records.EdgeofConcurrence => DefinitionHashes.InventoryItems.EdgeofConcurrence_542203595,
            DefinitionHashes.Records.EdgeofIntent => DefinitionHashes.InventoryItems.EdgeofIntent_14194600,
            DefinitionHashes.Records.FathersSins => DefinitionHashes.InventoryItems.FathersSins_3865728990,
            DefinitionHashes.Records.FelTaradiddle => DefinitionHashes.InventoryItems.FelTaradiddle_1399109800,
            DefinitionHashes.Records.Tarnation => DefinitionHashes.InventoryItems.Tarnation_2721157927,
            DefinitionHashes.Records.EmpiricalEvidence => DefinitionHashes.InventoryItems.EmpiricalEvidence_2607304614,
            DefinitionHashes.Records.ForensicNightmare => DefinitionHashes.InventoryItems.ForensicNightmare_1526296434,
            DefinitionHashes.Records.LikelySuspect => DefinitionHashes.InventoryItems.LikelySuspect_1994645182,
            DefinitionHashes.Records.OsteoStriga => DefinitionHashes.InventoryItems.OsteoStriga_46524085,
            DefinitionHashes.Records.PalmyraB => DefinitionHashes.InventoryItems.PalmyraB_3489657138,
            DefinitionHashes.Records.PointedInquiry => DefinitionHashes.InventoryItems.PointedInquiry_297296830,
            DefinitionHashes.Records.RagnhildD => DefinitionHashes.InventoryItems.RagnhildD_4225322581,
            DefinitionHashes.Records.RedHerring => DefinitionHashes.InventoryItems.RedHerring_3175851496,
            DefinitionHashes.Records.Syncopation53 => DefinitionHashes.InventoryItems.Syncopation53_2856514843,
            DefinitionHashes.Records.TheEnigma => DefinitionHashes.InventoryItems.TheEnigma_2595497736,
            DefinitionHashes.Records.AmmitAR2 => DefinitionHashes.InventoryItems.AmmitAR2_2119346509,
            DefinitionHashes.Records.Taipan4fr => DefinitionHashes.InventoryItems.Taipan4fr_1911060537,
            DefinitionHashes.Records.ImperialDecree => DefinitionHashes.InventoryItems.ImperialDecree,
            DefinitionHashes.Records.Goldtusk => DefinitionHashes.InventoryItems.Goldtusk,
            DefinitionHashes.Records.DeathsRazor => DefinitionHashes.InventoryItems.DeathsRazor,
            DefinitionHashes.Records.ThroneCleaver => DefinitionHashes.InventoryItems.ThroneCleaver,
            DefinitionHashes.Records.BxR55Battler => DefinitionHashes.InventoryItems.BxR55Battler_2708806099,
            DefinitionHashes.Records.PardonOurDust => DefinitionHashes.InventoryItems.PardonOurDust_3849810018,
            DefinitionHashes.Records.WastelanderM5 => DefinitionHashes.InventoryItems.WastelanderM5_1679868061,
            DefinitionHashes.Records.RetracedPath => DefinitionHashes.InventoryItems.RetracedPath_548958835,
            DefinitionHashes.Records.HalfTruths => DefinitionHashes.InventoryItems.HalfTruths_3257091166,
            DefinitionHashes.Records.TheOtherHalf => DefinitionHashes.InventoryItems.TheOtherHalf_3257091167,
            DefinitionHashes.Records.TarnishedMettle => DefinitionHashes.InventoryItems.TarnishedMettle_2218569744,
            DefinitionHashes.Records.BrigandsLaw => DefinitionHashes.InventoryItems.BrigandsLaw_1298815317,
            DefinitionHashes.Records.BloodFeud => DefinitionHashes.InventoryItems.BloodFeud_1509167284,
            DefinitionHashes.Records.NoReprieve => DefinitionHashes.InventoryItems.NoReprieve_2531963421,
            DefinitionHashes.Records.SailspyPitchglass => DefinitionHashes.InventoryItems.SailspyPitchglass_1184309824,
            DefinitionHashes.Records.PlancksStride => DefinitionHashes.InventoryItems.PlancksStride_820890091,
            DefinitionHashes.Records.DefianceofYasmin => DefinitionHashes.InventoryItems.DefianceofYasmin_3228096719,
            DefinitionHashes.Records.DoomofChelchis => DefinitionHashes.InventoryItems.DoomofChelchis_1937552980,
            DefinitionHashes.Records.MidhasReckoning => DefinitionHashes.InventoryItems.MidhasReckoning_3969066556,
            DefinitionHashes.Records.QullimsTerminus => DefinitionHashes.InventoryItems.QullimsTerminus_1321506184,
            DefinitionHashes.Records.SmiteofMerain => DefinitionHashes.InventoryItems.SmiteofMerain_2221264583,
            DefinitionHashes.Records.ZaoulisBane => DefinitionHashes.InventoryItems.ZaoulisBane_431721920,
            _ => 0
        };
    }

    private static class CraftSource
    {
        public const string SeasonPlunder = "Plunder";
        public const string SeasonHaunted = "Haunted";
        public const string SeasonRisen = "Risen";
        public const string Opulent = "Opulent";
        public const string RaidVotD = "Vow of the Disciple";
        public const string WqWellspring = "Wellspring";
        public const string Wq = "Witch Queen";
        public const string Anniversary = "30th Anniversary";
        public const string Unknown = "Unknown";
        public const string KingsFall = "Kings Fall";
    }
}