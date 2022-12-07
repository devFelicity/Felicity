using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models.Destiny.Responses;

namespace Felicity.Util.Enums;

internal static class Craftables
{
    public static readonly Dictionary<string, List<uint>> CraftableList = new()
    {
        {
            CraftSource.SeasonSeraph, new List<uint>
            {
                DefinitionHashes.Records.Disparity,
                DefinitionHashes.Records.FireandForget,
                DefinitionHashes.Records.IKELOS_HC_v103,
                DefinitionHashes.Records.IKELOS_SG_v103,
                DefinitionHashes.Records.IKELOS_SMG_v103,
                DefinitionHashes.Records.IKELOS_SR_v103,
                DefinitionHashes.Records.JudgmentofKelgorath,
                DefinitionHashes.Records.PathofLeastResistance,
                DefinitionHashes.Records.RetrofitEscapade,
                DefinitionHashes.Records.TripwireCanary
            }
        },
        {
            CraftSource.DeepStoneCrypt, new List<uint>
            {
                DefinitionHashes.Records.Bequest,
                DefinitionHashes.Records.Commemoration,
                DefinitionHashes.Records.Heritage,
                DefinitionHashes.Records.Posterity,
                DefinitionHashes.Records.Succession,
                DefinitionHashes.Records.Trustee
            }
        },
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
                DefinitionHashes.Records.Taipan4fr
                // DefinitionHashes.Records.ImperialDecree,
                // DefinitionHashes.Records.Goldtusk,
                // DefinitionHashes.Records.DeathsRazor,
                // DefinitionHashes.Records.ThroneCleaver
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

    public static readonly Dictionary<string, List<uint>> CraftedList = new()
    {
        {
            CraftSource.SeasonSeraph, new List<uint>
            {
                DefinitionHashes.InventoryItems.Disparity_1751893422,
                DefinitionHashes.InventoryItems.FireandForget_2272041093,
                DefinitionHashes.InventoryItems.IKELOS_HC_v103_1731355324,
                DefinitionHashes.InventoryItems.IKELOS_SG_v103_1168625549,
                DefinitionHashes.InventoryItems.IKELOS_SMG_v103,
                DefinitionHashes.InventoryItems.IKELOS_SR_v103_2302346155,
                DefinitionHashes.InventoryItems.JudgmentofKelgorath_2978226043,
                DefinitionHashes.InventoryItems.PathofLeastResistance_2827764482,
                DefinitionHashes.InventoryItems.RetrofitEscapade_3103325054,
                DefinitionHashes.InventoryItems.TripwireCanary_3849444474
            }
        },
        {
            CraftSource.DeepStoneCrypt, new List<uint>
            {
                DefinitionHashes.InventoryItems.Bequest_3366545721,
                DefinitionHashes.InventoryItems.Commemoration_4230965989,
                DefinitionHashes.InventoryItems.Heritage_4248569242,
                DefinitionHashes.InventoryItems.Posterity_3281285075,
                DefinitionHashes.InventoryItems.Succession_2990047042,
                DefinitionHashes.InventoryItems.Trustee_1392919471
            }
        },
        {
            CraftSource.KingsFall, new List<uint>
            {
                DefinitionHashes.InventoryItems.DefianceofYasmin_3228096719,
                DefinitionHashes.InventoryItems.DoomofChelchis_1937552980,
                DefinitionHashes.InventoryItems.MidhasReckoning_3969066556,
                DefinitionHashes.InventoryItems.QullimsTerminus_1321506184,
                DefinitionHashes.InventoryItems.SmiteofMerain_2221264583,
                DefinitionHashes.InventoryItems.ZaoulisBane_431721920
            }
        },
        {
            CraftSource.SeasonPlunder, new List<uint>
            {
                DefinitionHashes.InventoryItems.BloodFeud_1509167284,
                DefinitionHashes.InventoryItems.BrigandsLaw_1298815317,
                DefinitionHashes.InventoryItems.NoReprieve_2531963421,
                DefinitionHashes.InventoryItems.PlancksStride_820890091,
                DefinitionHashes.InventoryItems.SailspyPitchglass_1184309824,
                DefinitionHashes.InventoryItems.TarnishedMettle_2218569744
            }
        },
        {
            CraftSource.Anniversary, new List<uint>
            {
                DefinitionHashes.InventoryItems.BxR55Battler_2708806099,
                DefinitionHashes.InventoryItems.HalfTruths_3257091166,
                DefinitionHashes.InventoryItems.PardonOurDust_3849810018,
                DefinitionHashes.InventoryItems.RetracedPath_548958835,
                DefinitionHashes.InventoryItems.TheOtherHalf_3257091167,
                DefinitionHashes.InventoryItems.WastelanderM5_1679868061
            }
        },
        {
            CraftSource.Unknown, new List<uint>
            {
                DefinitionHashes.InventoryItems.AmmitAR2_2119346509,
                DefinitionHashes.InventoryItems.Taipan4fr_1911060537
            }
        },
        {
            CraftSource.SeasonHaunted, new List<uint>
            {
                DefinitionHashes.InventoryItems.BumpintheNight_1959650777,
                DefinitionHashes.InventoryItems.Firefright_2778013407,
                DefinitionHashes.InventoryItems.HollowDenial_2323544076,
                DefinitionHashes.InventoryItems.NezarecsWhisper_254636484,
                DefinitionHashes.InventoryItems.TearsofContrition_1366394399,
                DefinitionHashes.InventoryItems.WithoutRemorse_1478986057
            }
        },
        {
            CraftSource.Opulent, new List<uint>
            {
                DefinitionHashes.InventoryItems.Austringer_3055790362,
                DefinitionHashes.InventoryItems.Beloved_3107853529,
                DefinitionHashes.InventoryItems.CALUSMiniTool_2490988246,
                DefinitionHashes.InventoryItems.DrangBaroque_502356570,
                DefinitionHashes.InventoryItems.FixedOdds_2194955522,
                DefinitionHashes.InventoryItems.TheEpicurean_2263839058
            }
        },
        {
            CraftSource.SeasonRisen, new List<uint>
            {
                DefinitionHashes.InventoryItems.ExplosivePersonality_4096943616,
                DefinitionHashes.InventoryItems.PieceofMind_2097055732,
                DefinitionHashes.InventoryItems.RecurrentImpact_1572896086,
                DefinitionHashes.InventoryItems.SweetSorrow_1248372789,
                DefinitionHashes.InventoryItems.Thoughtless_4067556514,
                DefinitionHashes.InventoryItems.UnderYourSkin_232928045
            }
        },
        {
            CraftSource.RaidVotD, new List<uint>
            {
                DefinitionHashes.InventoryItems.Cataclysmic_999767358,
                DefinitionHashes.InventoryItems.Deliverance_768621510,
                DefinitionHashes.InventoryItems.Forbearance_613334176,
                DefinitionHashes.InventoryItems.Insidious_3428521585,
                DefinitionHashes.InventoryItems.LubraesRuin_2534546147,
                DefinitionHashes.InventoryItems.Submission_3886416794
            }
        },
        {
            CraftSource.WqWellspring, new List<uint>
            {
                DefinitionHashes.InventoryItems.CometoPass_927567426,
                DefinitionHashes.InventoryItems.EdgeofAction_2535142413,
                DefinitionHashes.InventoryItems.EdgeofConcurrence_542203595,
                DefinitionHashes.InventoryItems.EdgeofIntent_14194600,
                DefinitionHashes.InventoryItems.FathersSins_3865728990,
                DefinitionHashes.InventoryItems.FelTaradiddle_1399109800,
                DefinitionHashes.InventoryItems.Tarnation_2721157927
            }
        },
        {
            CraftSource.Wq, new List<uint>
            {
                DefinitionHashes.InventoryItems.EmpiricalEvidence_2607304614,
                DefinitionHashes.InventoryItems.ForensicNightmare_1526296434,
                DefinitionHashes.InventoryItems.LikelySuspect_1994645182,
                DefinitionHashes.InventoryItems.OsteoStriga_46524085,
                DefinitionHashes.InventoryItems.PalmyraB_3489657138,
                DefinitionHashes.InventoryItems.PointedInquiry_297296830,
                DefinitionHashes.InventoryItems.RagnhildD_4225322581,
                DefinitionHashes.InventoryItems.RedHerring_3175851496,
                DefinitionHashes.InventoryItems.Syncopation53_2856514843,
                DefinitionHashes.InventoryItems.TheEnigma_2595497736
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

    public static bool GetWeaponLevel(DestinyItemResponse response, out string weaponLevel)
    {
        weaponLevel = string.Empty;

        var weaponObjective = response.PlugObjectives.Data.ObjectivesPerPlug
            .First(x => x.Key.Hash
                is DefinitionHashes.InventoryItems.ShapedWeapon_1922808508
                or DefinitionHashes.InventoryItems.ShapedWeapon_659359923
                or DefinitionHashes.InventoryItems.ShapedWeapon_4029346515).Value;

        var obj = weaponObjective.FirstOrDefault(x => x.Objective.Hash == 3077315735);
        if (obj == null)
            return false;

        weaponLevel = obj.Progress.ToString() ?? string.Empty;
        return true;
    }

    private static class CraftSource
    {
        public const string Anniversary = "30th Anniversary";
        public const string DeepStoneCrypt = "Deep Stone Crypt";
        public const string KingsFall = "Kings Fall";
        public const string Opulent = "Opulent";
        public const string RaidVotD = "Vow of the Disciple";
        public const string SeasonHaunted = "Haunted";
        public const string SeasonPlunder = "Plunder";
        public const string SeasonRisen = "Risen";
        public const string SeasonSeraph = "Seraph";
        public const string Unknown = "Quest / Unknown";
        public const string Wq = "Witch Queen";
        public const string WqWellspring = "Wellspring";
    }
}