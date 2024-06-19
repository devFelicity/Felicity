using DotNetBungieAPI.HashReferences;
using DotNetBungieAPI.Models.Destiny.Responses;

namespace Felicity.Util.Enums;

internal static class Craftables
{
    public static readonly Dictionary<string, List<uint>> CraftableList = new()
    {
        {
            CraftSource.SeasonEchoes, [
                DefinitionHashes.Records.FaithKeeper,
                DefinitionHashes.Records.IllOmen,
                DefinitionHashes.Records.LostSignal,
                DefinitionHashes.Records.SightlineSurvey,
                DefinitionHashes.Records.TimewornWayfarer,
                DefinitionHashes.Records.VeiledThreat
            ]
        },
        {
            CraftSource.SeasonWish, [
                DefinitionHashes.Records.Appetence,
                DefinitionHashes.Records.DoomedPetitioner,
                DefinitionHashes.Records.Lethophobia,
                DefinitionHashes.Records.ScalarPotential,
                DefinitionHashes.Records.ScatterSignal,
                DefinitionHashes.Records.Supercluster
            ]
        },
        {
            CraftSource.SeasonUndying, [
                DefinitionHashes.Records.Adhortative,
                DefinitionHashes.Records.Imperative,
                DefinitionHashes.Records.Subjunctive,
                DefinitionHashes.Records.Optative
            ]
        },
        {
            CraftSource.RaidCe, [
                DefinitionHashes.Records.AbyssDefiant,
                DefinitionHashes.Records.FangofIrYût,
                DefinitionHashes.Records.OversoulEdict,
                DefinitionHashes.Records.SongofIrYût,
                DefinitionHashes.Records.Swordbreaker,
                DefinitionHashes.Records.WordofCrota
            ]
        },
        {
            CraftSource.SeasonWitch, [
                DefinitionHashes.Records.BryasLove,
                DefinitionHashes.Records.EleaticPrinciple,
                DefinitionHashes.Records.TheEremite,
                DefinitionHashes.Records.KeptConfidence,
                DefinitionHashes.Records.LocusLocutus,
                DefinitionHashes.Records.Semiotician
            ]
        },
        {
            CraftSource.SeasonDeep, [
                DefinitionHashes.Records.DifferentTimes,
                DefinitionHashes.Records.ADistantPull,
                DefinitionHashes.Records.RapaciousAppetite,
                DefinitionHashes.Records.TargetedRedaction,
                DefinitionHashes.Records.ThinPrecipice,
                DefinitionHashes.Records.UntilItsReturn
            ]
        },
        {
            CraftSource.RaidLw, [
                DefinitionHashes.Records.AgeOldBond,
                DefinitionHashes.Records.ApexPredator,
                DefinitionHashes.Records.ChatteringBone,
                DefinitionHashes.Records.NationofBeasts,
                DefinitionHashes.Records.TecheunForce,
                DefinitionHashes.Records.Transfiguration,
                DefinitionHashes.Records.TyrannyofHeaven,
                DefinitionHashes.Records.TheSupremacy
            ]
        },
        {
            CraftSource.RaidRoN, [
                DefinitionHashes.Records.AcasiasDejection,
                DefinitionHashes.Records.BriarsContempt,
                DefinitionHashes.Records.KoraxissDistress,
                DefinitionHashes.Records.MykelsReverence,
                DefinitionHashes.Records.NessasOblation,
                DefinitionHashes.Records.RufussFury
            ]
        },
        {
            CraftSource.SeasonDefiance, [
                DefinitionHashes.Records.Caretaker_3171877617,
                DefinitionHashes.Records.Goldtusk,
                DefinitionHashes.Records.ImperialDecree,
                DefinitionHashes.Records.Perpetualis,
                DefinitionHashes.Records.ProdigalReturn,
                DefinitionHashes.Records.Raconteur,
                DefinitionHashes.Records.DeathsRazor,
                DefinitionHashes.Records.Regnant,
                DefinitionHashes.Records.RoyalExecutioner,
                DefinitionHashes.Records.ThroneCleaver
            ]
        },
        {
            CraftSource.Lightfall, [
                DefinitionHashes.Records.DimensionalHypotrochoid,
                DefinitionHashes.Records.IterativeLoop,
                DefinitionHashes.Records.PhyllotacticSpiral,
                DefinitionHashes.Records.RoundRobin_2839479882,
                DefinitionHashes.Records.VoltaBracket
            ]
        },
        {
            CraftSource.SeasonSeraph, [
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
            ]
        },
        {
            CraftSource.RaidDsc, [
                DefinitionHashes.Records.Bequest,
                DefinitionHashes.Records.Commemoration,
                DefinitionHashes.Records.Heritage,
                DefinitionHashes.Records.Posterity,
                DefinitionHashes.Records.Succession,
                DefinitionHashes.Records.Trustee
            ]
        },
        {
            CraftSource.RaidKf, [
                DefinitionHashes.Records.DefianceofYasmin,
                DefinitionHashes.Records.DoomofChelchis,
                DefinitionHashes.Records.MidhasReckoning,
                DefinitionHashes.Records.QullimsTerminus,
                DefinitionHashes.Records.SmiteofMerain,
                DefinitionHashes.Records.ZaoulisBane
            ]
        },
        {
            CraftSource.SeasonPlunder, [
                DefinitionHashes.Records.BloodFeud,
                DefinitionHashes.Records.BrigandsLaw,
                DefinitionHashes.Records.NoReprieve,
                DefinitionHashes.Records.PlancksStride,
                DefinitionHashes.Records.SailspyPitchglass,
                DefinitionHashes.Records.TarnishedMettle
            ]
        },
        {
            CraftSource.Anniversary, [
                DefinitionHashes.Records.BxR55Battler,
                DefinitionHashes.Records.HalfTruths,
                DefinitionHashes.Records.PardonOurDust,
                DefinitionHashes.Records.RetracedPath,
                DefinitionHashes.Records.TheOtherHalf,
                DefinitionHashes.Records.WastelanderM5
            ]
        },
        {
            CraftSource.Unknown, [
                DefinitionHashes.Records.AmmitAR2,
                DefinitionHashes.Records.DeadMansTale,
                DefinitionHashes.Records.DeadMessenger,
                DefinitionHashes.Records.Taipan4fr,
                DefinitionHashes.Records.RevisionZero,
                DefinitionHashes.Records.Vexcalibur
            ]
        },
        {
            CraftSource.SeasonHaunted, [
                DefinitionHashes.Records.BumpintheNight,
                DefinitionHashes.Records.Firefright,
                DefinitionHashes.Records.HollowDenial,
                DefinitionHashes.Records.NezarecsWhisper,
                DefinitionHashes.Records.TearsofContrition,
                DefinitionHashes.Records.WithoutRemorse
            ]
        },
        {
            CraftSource.Opulent, [
                DefinitionHashes.Records.Austringer,
                DefinitionHashes.Records.Beloved_662547074,
                DefinitionHashes.Records.CALUSMiniTool,
                DefinitionHashes.Records.DrangBaroque,
                DefinitionHashes.Records.FixedOdds,
                DefinitionHashes.Records.TheEpicurean
            ]
        },
        {
            CraftSource.SeasonRisen, [
                DefinitionHashes.Records.ExplosivePersonality,
                DefinitionHashes.Records.PieceofMind,
                DefinitionHashes.Records.RecurrentImpact,
                DefinitionHashes.Records.SweetSorrow,
                DefinitionHashes.Records.Thoughtless,
                DefinitionHashes.Records.UnderYourSkin
            ]
        },
        {
            CraftSource.RaidVotD, [
                DefinitionHashes.Records.Cataclysmic,
                DefinitionHashes.Records.Deliverance,
                DefinitionHashes.Records.Forbearance,
                DefinitionHashes.Records.Insidious,
                DefinitionHashes.Records.LubraesRuin,
                DefinitionHashes.Records.Submission
            ]
        },
        {
            CraftSource.WqWellspring, [
                DefinitionHashes.Records.CometoPass,
                DefinitionHashes.Records.EdgeofAction,
                DefinitionHashes.Records.EdgeofConcurrence,
                DefinitionHashes.Records.EdgeofIntent,
                DefinitionHashes.Records.FathersSins,
                DefinitionHashes.Records.FelTaradiddle,
                DefinitionHashes.Records.Tarnation
            ]
        },
        {
            CraftSource.Wq, [
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
            ]
        }
    };

    public static readonly Dictionary<string, List<uint>> CraftedList = new()
    {
        {
            CraftSource.SeasonEchoes, [
                DefinitionHashes.InventoryItems.FaithKeeper_4195186942,
                DefinitionHashes.InventoryItems.IllOmen_3794274730,
                DefinitionHashes.InventoryItems.LostSignal_1197771438,
                DefinitionHashes.InventoryItems.SightlineSurvey_2350330520,
                DefinitionHashes.InventoryItems.TimewornWayfarer_1058098236,
                DefinitionHashes.InventoryItems.VeiledThreat_3685470415
            ]
        },
        {
            CraftSource.SeasonWish, [
                DefinitionHashes.InventoryItems.Appetence_4153087276,
                DefinitionHashes.InventoryItems.DoomedPetitioner_1501688142,
                DefinitionHashes.InventoryItems.Lethophobia_3710082365,
                DefinitionHashes.InventoryItems.ScalarPotential_2563668388,
                DefinitionHashes.InventoryItems.ScatterSignal_2558925366,
                DefinitionHashes.InventoryItems.Supercluster_92459755
            ]
        },
        {
            CraftSource.SeasonUndying, [
                DefinitionHashes.InventoryItems.Adhortative_2993554824,
                DefinitionHashes.InventoryItems.Imperative_2045811635,
                DefinitionHashes.InventoryItems.Subjunctive_1447836603,
                DefinitionHashes.InventoryItems.Optative_2817683783
            ]
        },
        {
            CraftSource.RaidCe, [
                DefinitionHashes.InventoryItems.AbyssDefiant_833898322,
                DefinitionHashes.InventoryItems.FangofIrYût_1432682459,
                DefinitionHashes.InventoryItems.OversoulEdict_1098171824,
                DefinitionHashes.InventoryItems.SongofIrYût_2828278545,
                DefinitionHashes.InventoryItems.Swordbreaker_3163900678,
                DefinitionHashes.InventoryItems.WordofCrota_120706239
            ]
        },
        {
            CraftSource.SeasonWitch, [
                DefinitionHashes.InventoryItems.BryasLove_2779821308,
                DefinitionHashes.InventoryItems.EleaticPrinciple_105306149,
                DefinitionHashes.InventoryItems.TheEremite_3347946548,
                DefinitionHashes.InventoryItems.LocusLocutus_4132072834,
                DefinitionHashes.InventoryItems.KeptConfidence_1875512595,
                DefinitionHashes.InventoryItems.Semiotician_2922749929
            ]
        },
        {
            CraftSource.SeasonDeep, [
                DefinitionHashes.InventoryItems.DifferentTimes_3016891299,
                DefinitionHashes.InventoryItems.ADistantPull_1769847435,
                DefinitionHashes.InventoryItems.RapaciousAppetite_1081724548,
                DefinitionHashes.InventoryItems.TargetedRedaction_3890055324,
                DefinitionHashes.InventoryItems.ThinPrecipice_4066778670,
                DefinitionHashes.InventoryItems.UntilItsReturn_2883484461
            ]
        },
        {
            CraftSource.RaidLw, [
                DefinitionHashes.InventoryItems.AgeOldBond_424291879,
                DefinitionHashes.InventoryItems.ApexPredator_1851777734,
                DefinitionHashes.InventoryItems.ChatteringBone_501329015,
                DefinitionHashes.InventoryItems.NationofBeasts_70083888,
                DefinitionHashes.InventoryItems.TecheunForce_3591141932,
                DefinitionHashes.InventoryItems.Transfiguration_3885259140,
                DefinitionHashes.InventoryItems.TyrannyofHeaven_3388655311,
                DefinitionHashes.InventoryItems.TheSupremacy_2884596447
            ]
        },
        {
            CraftSource.RaidRoN, [
                DefinitionHashes.InventoryItems.AcasiasDejection_1471212226,
                DefinitionHashes.InventoryItems.BriarsContempt_1491665733,
                DefinitionHashes.InventoryItems.KoraxissDistress_2972949637,
                DefinitionHashes.InventoryItems.MykelsReverence_231031173,
                DefinitionHashes.InventoryItems.NessasOblation_135029084,
                DefinitionHashes.InventoryItems.RufussFury_484515708
            ]
        },
        {
            CraftSource.SeasonDefiance, [
                DefinitionHashes.InventoryItems.Caretaker_2508948099,
                DefinitionHashes.InventoryItems.DeathsRazor_569799274,
                DefinitionHashes.InventoryItems.Goldtusk_569799275,
                DefinitionHashes.InventoryItems.ImperialDecree_2919334548,
                DefinitionHashes.InventoryItems.Perpetualis_392008588,
                DefinitionHashes.InventoryItems.ProdigalReturn_268260373,
                DefinitionHashes.InventoryItems.Raconteur_45643573,
                DefinitionHashes.InventoryItems.Regnant_268260372,
                DefinitionHashes.InventoryItems.RoyalExecutioner_1720503118,
                DefinitionHashes.InventoryItems.ThroneCleaver_569799273
            ]
        },
        {
            CraftSource.Lightfall, [
                DefinitionHashes.InventoryItems.DimensionalHypotrochoid_1311684613,
                DefinitionHashes.InventoryItems.RoundRobin_2034215657,
                DefinitionHashes.InventoryItems.PhyllotacticSpiral_3635821806,
                DefinitionHashes.InventoryItems.IterativeLoop_1289796511,
                DefinitionHashes.InventoryItems.VoltaBracket_3920310144
            ]
        },
        {
            CraftSource.SeasonSeraph, [
                DefinitionHashes.InventoryItems.Disparity_1751893422,
                DefinitionHashes.InventoryItems.FireandForget_2272041093,
                DefinitionHashes.InventoryItems.IKELOS_HC_v103,
                DefinitionHashes.InventoryItems.IKELOS_SG_v103,
                DefinitionHashes.InventoryItems.IKELOS_SMG_v103,
                DefinitionHashes.InventoryItems.IKELOS_SR_v103,
                DefinitionHashes.InventoryItems.JudgmentofKelgorath_2978226043,
                DefinitionHashes.InventoryItems.PathofLeastResistance_2827764482,
                DefinitionHashes.InventoryItems.RetrofitEscapade_3103325054,
                DefinitionHashes.InventoryItems.TripwireCanary_3849444474
            ]
        },
        {
            CraftSource.RaidDsc, [
                DefinitionHashes.InventoryItems.Bequest_3366545721,
                DefinitionHashes.InventoryItems.Commemoration_4230965989,
                DefinitionHashes.InventoryItems.Heritage_4248569242,
                DefinitionHashes.InventoryItems.Posterity_3281285075,
                DefinitionHashes.InventoryItems.Succession_2990047042,
                DefinitionHashes.InventoryItems.Trustee_1392919471
            ]
        },
        {
            CraftSource.RaidKf, [
                DefinitionHashes.InventoryItems.DefianceofYasmin_3228096719,
                DefinitionHashes.InventoryItems.DoomofChelchis_1937552980,
                DefinitionHashes.InventoryItems.MidhasReckoning_3969066556,
                DefinitionHashes.InventoryItems.QullimsTerminus_1321506184,
                DefinitionHashes.InventoryItems.SmiteofMerain_2221264583,
                DefinitionHashes.InventoryItems.ZaoulisBane_431721920
            ]
        },
        {
            CraftSource.SeasonPlunder, [
                DefinitionHashes.InventoryItems.BloodFeud_1509167284,
                DefinitionHashes.InventoryItems.BrigandsLaw_1298815317,
                DefinitionHashes.InventoryItems.NoReprieve_2531963421,
                DefinitionHashes.InventoryItems.PlancksStride_820890091,
                DefinitionHashes.InventoryItems.SailspyPitchglass_1184309824,
                DefinitionHashes.InventoryItems.TarnishedMettle_2218569744
            ]
        },
        {
            CraftSource.Anniversary, [
                DefinitionHashes.InventoryItems.BxR55Battler_2708806099,
                DefinitionHashes.InventoryItems.HalfTruths_3257091166,
                DefinitionHashes.InventoryItems.PardonOurDust_3849810018,
                DefinitionHashes.InventoryItems.RetracedPath_548958835,
                DefinitionHashes.InventoryItems.TheOtherHalf_3257091167,
                DefinitionHashes.InventoryItems.WastelanderM5_1679868061
            ]
        },
        {
            CraftSource.Unknown, [
                DefinitionHashes.InventoryItems.AmmitAR2_2119346509,
                2188764214,
                46125926,
                DefinitionHashes.InventoryItems.RevisionZero_1473821207,
                DefinitionHashes.InventoryItems.Taipan4fr_1911060537,
                DefinitionHashes.InventoryItems.Vexcalibur_3118061005
            ]
        },
        {
            CraftSource.SeasonHaunted, [
                DefinitionHashes.InventoryItems.BumpintheNight_1959650777,
                DefinitionHashes.InventoryItems.Firefright_2778013407,
                DefinitionHashes.InventoryItems.HollowDenial_2323544076,
                DefinitionHashes.InventoryItems.NezarecsWhisper_254636484,
                DefinitionHashes.InventoryItems.TearsofContrition_1366394399,
                DefinitionHashes.InventoryItems.WithoutRemorse_1478986057
            ]
        },
        {
            CraftSource.Opulent, [
                DefinitionHashes.InventoryItems.Austringer_3055790362,
                DefinitionHashes.InventoryItems.Beloved_3107853529,
                DefinitionHashes.InventoryItems.CALUSMiniTool_2490988246,
                DefinitionHashes.InventoryItems.DrangBaroque_502356570,
                DefinitionHashes.InventoryItems.FixedOdds_2194955522,
                DefinitionHashes.InventoryItems.TheEpicurean_2263839058
            ]
        },
        {
            CraftSource.SeasonRisen, [
                DefinitionHashes.InventoryItems.ExplosivePersonality_4096943616,
                DefinitionHashes.InventoryItems.PieceofMind_2097055732,
                DefinitionHashes.InventoryItems.RecurrentImpact_1572896086,
                DefinitionHashes.InventoryItems.SweetSorrow_1248372789,
                DefinitionHashes.InventoryItems.Thoughtless_4067556514,
                DefinitionHashes.InventoryItems.UnderYourSkin_232928045
            ]
        },
        {
            CraftSource.RaidVotD, [
                DefinitionHashes.InventoryItems.Cataclysmic_999767358,
                DefinitionHashes.InventoryItems.Deliverance_768621510,
                DefinitionHashes.InventoryItems.Forbearance_613334176,
                DefinitionHashes.InventoryItems.Insidious_3428521585,
                DefinitionHashes.InventoryItems.LubraesRuin_2534546147,
                DefinitionHashes.InventoryItems.Submission_3886416794
            ]
        },
        {
            CraftSource.WqWellspring, [
                DefinitionHashes.InventoryItems.CometoPass_927567426,
                DefinitionHashes.InventoryItems.EdgeofAction_2535142413,
                DefinitionHashes.InventoryItems.EdgeofConcurrence_542203595,
                DefinitionHashes.InventoryItems.EdgeofIntent_14194600,
                DefinitionHashes.InventoryItems.FathersSins_3865728990,
                DefinitionHashes.InventoryItems.FelTaradiddle_1399109800,
                DefinitionHashes.InventoryItems.Tarnation_2721157927
            ]
        },
        {
            CraftSource.Wq, [
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
            ]
        }
    };

    public static uint GetWeaponId(uint recordDefinitionHash)
    {
        return recordDefinitionHash switch
        {
            DefinitionHashes.Records.Appetence => DefinitionHashes.InventoryItems.Appetence_4153087276,
            DefinitionHashes.Records.DoomedPetitioner => DefinitionHashes.InventoryItems.DoomedPetitioner_1501688142,
            DefinitionHashes.Records.Lethophobia => DefinitionHashes.InventoryItems.Lethophobia_3710082365,
            DefinitionHashes.Records.ScalarPotential => DefinitionHashes.InventoryItems.ScalarPotential_2563668388,
            DefinitionHashes.Records.ScatterSignal => DefinitionHashes.InventoryItems.ScatterSignal_2558925366,
            DefinitionHashes.Records.Supercluster => DefinitionHashes.InventoryItems.Supercluster_92459755,
            DefinitionHashes.Records.Adhortative => DefinitionHashes.InventoryItems.Adhortative_2993554824,
            DefinitionHashes.Records.Imperative => DefinitionHashes.InventoryItems.Imperative_2045811635,
            DefinitionHashes.Records.Subjunctive => DefinitionHashes.InventoryItems.Subjunctive_1447836603,
            DefinitionHashes.Records.Optative => DefinitionHashes.InventoryItems.Optative_2817683783,
            DefinitionHashes.Records.AbyssDefiant => DefinitionHashes.InventoryItems.AbyssDefiant_833898322,
            DefinitionHashes.Records.FangofIrYût => DefinitionHashes.InventoryItems.FangofIrYût_1432682459,
            DefinitionHashes.Records.OversoulEdict => DefinitionHashes.InventoryItems.OversoulEdict_1098171824,
            DefinitionHashes.Records.SongofIrYût => DefinitionHashes.InventoryItems.SongofIrYût_2828278545,
            DefinitionHashes.Records.Swordbreaker => DefinitionHashes.InventoryItems.Swordbreaker_3163900678,
            DefinitionHashes.Records.WordofCrota => DefinitionHashes.InventoryItems.WordofCrota_120706239,
            DefinitionHashes.Records.BryasLove => DefinitionHashes.InventoryItems.BryasLove_2779821308,
            DefinitionHashes.Records.EleaticPrinciple => DefinitionHashes.InventoryItems.EleaticPrinciple_105306149,
            DefinitionHashes.Records.TheEremite => DefinitionHashes.InventoryItems.TheEremite_3347946548,
            DefinitionHashes.Records.KeptConfidence => DefinitionHashes.InventoryItems.LocusLocutus_4132072834,
            DefinitionHashes.Records.LocusLocutus => DefinitionHashes.InventoryItems.KeptConfidence_1875512595,
            DefinitionHashes.Records.Semiotician => DefinitionHashes.InventoryItems.Semiotician_2922749929,
            DefinitionHashes.Records.DifferentTimes => DefinitionHashes.InventoryItems.DifferentTimes_3016891299,
            DefinitionHashes.Records.ADistantPull => DefinitionHashes.InventoryItems.ADistantPull_1769847435,
            DefinitionHashes.Records.RapaciousAppetite => DefinitionHashes.InventoryItems.RapaciousAppetite_1081724548,
            DefinitionHashes.Records.TargetedRedaction => DefinitionHashes.InventoryItems.TargetedRedaction_3890055324,
            DefinitionHashes.Records.ThinPrecipice => DefinitionHashes.InventoryItems.ThinPrecipice_4066778670,
            DefinitionHashes.Records.UntilItsReturn => DefinitionHashes.InventoryItems.UntilItsReturn_2883484461,
            DefinitionHashes.Records.AgeOldBond => DefinitionHashes.InventoryItems.AgeOldBond_601592879,
            DefinitionHashes.Records.ApexPredator => DefinitionHashes.InventoryItems.ApexPredator_2545083870,
            DefinitionHashes.Records.ChatteringBone => DefinitionHashes.InventoryItems.ChatteringBone_568515759,
            DefinitionHashes.Records.NationofBeasts => DefinitionHashes.InventoryItems.NationofBeasts_654370424,
            DefinitionHashes.Records.TecheunForce => DefinitionHashes.InventoryItems.TecheunForce_4094657108,
            DefinitionHashes.Records.Transfiguration => DefinitionHashes.InventoryItems.Transfiguration_3799980700,
            DefinitionHashes.Records.TyrannyofHeaven => DefinitionHashes.InventoryItems.TyrannyofHeaven_2721249463,
            DefinitionHashes.Records.TheSupremacy => DefinitionHashes.InventoryItems.TheSupremacy_686951703,
            DefinitionHashes.Records.AcasiasDejection => DefinitionHashes.InventoryItems.AcasiasDejection_1471212226,
            DefinitionHashes.Records.BriarsContempt => DefinitionHashes.InventoryItems.BriarsContempt_1491665733,
            DefinitionHashes.Records.KoraxissDistress => DefinitionHashes.InventoryItems.KoraxissDistress_2972949637,
            DefinitionHashes.Records.MykelsReverence => DefinitionHashes.InventoryItems.MykelsReverence_231031173,
            DefinitionHashes.Records.NessasOblation => DefinitionHashes.InventoryItems.NessasOblation_135029084,
            DefinitionHashes.Records.RufussFury => DefinitionHashes.InventoryItems.RufussFury_484515708,
            DefinitionHashes.Records.BumpintheNight => DefinitionHashes.InventoryItems.BumpintheNight_1959650777,
            DefinitionHashes.Records.Firefright => DefinitionHashes.InventoryItems.Firefright_2778013407,
            DefinitionHashes.Records.HollowDenial => DefinitionHashes.InventoryItems.HollowDenial_2323544076,
            DefinitionHashes.Records.NezarecsWhisper => DefinitionHashes.InventoryItems.NezarecsWhisper_254636484,
            DefinitionHashes.Records.TearsofContrition => DefinitionHashes.InventoryItems.TearsofContrition_1366394399,
            DefinitionHashes.Records.WithoutRemorse => DefinitionHashes.InventoryItems.WithoutRemorse_1478986057,
            DefinitionHashes.Records.Austringer => DefinitionHashes.InventoryItems.Austringer_3055790362,
            DefinitionHashes.Records.Beloved_662547074 => DefinitionHashes.InventoryItems.Beloved_3107853529,
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
            DefinitionHashes.Records.DimensionalHypotrochoid => DefinitionHashes.InventoryItems
                .DimensionalHypotrochoid_1311684613,
            DefinitionHashes.Records.Perpetualis => DefinitionHashes.InventoryItems.Perpetualis_392008588,
            DefinitionHashes.Records.Raconteur => DefinitionHashes.InventoryItems.Raconteur_45643573,
            DefinitionHashes.Records.RoyalExecutioner => DefinitionHashes.InventoryItems.RoyalExecutioner_1720503118,
            DefinitionHashes.Records.ProdigalReturn => DefinitionHashes.InventoryItems.ProdigalReturn_268260373,
            DefinitionHashes.Records.Regnant => DefinitionHashes.InventoryItems.Regnant_268260372,
            DefinitionHashes.Records.Caretaker_3171877617 => DefinitionHashes.InventoryItems.Caretaker_2508948099,
            DefinitionHashes.Records.RoundRobin_2839479882 => DefinitionHashes.InventoryItems.RoundRobin_2034215657,
            DefinitionHashes.Records.PhyllotacticSpiral =>
                DefinitionHashes.InventoryItems.PhyllotacticSpiral_3635821806,
            DefinitionHashes.Records.IterativeLoop => DefinitionHashes.InventoryItems.IterativeLoop_1289796511,
            DefinitionHashes.Records.VoltaBracket => DefinitionHashes.InventoryItems.VoltaBracket_3920310144,
            DefinitionHashes.Records.Disparity => DefinitionHashes.InventoryItems.Disparity_1751893422,
            DefinitionHashes.Records.FireandForget => DefinitionHashes.InventoryItems.FireandForget_2272041093,
            DefinitionHashes.Records.IKELOS_HC_v103 => DefinitionHashes.InventoryItems.IKELOS_HC_v103,
            DefinitionHashes.Records.IKELOS_SG_v103 => DefinitionHashes.InventoryItems.IKELOS_SG_v103,
            DefinitionHashes.Records.IKELOS_SMG_v103 => DefinitionHashes.InventoryItems.IKELOS_SMG_v103,
            DefinitionHashes.Records.IKELOS_SR_v103 => DefinitionHashes.InventoryItems.IKELOS_SR_v103,
            DefinitionHashes.Records.JudgmentofKelgorath => DefinitionHashes.InventoryItems
                .JudgmentofKelgorath_2978226043,
            DefinitionHashes.Records.PathofLeastResistance => DefinitionHashes.InventoryItems
                .PathofLeastResistance_2827764482,
            DefinitionHashes.Records.RetrofitEscapade => DefinitionHashes.InventoryItems.RetrofitEscapade_3103325054,
            DefinitionHashes.Records.TripwireCanary => DefinitionHashes.InventoryItems.TripwireCanary_3849444474,
            DefinitionHashes.Records.Bequest => DefinitionHashes.InventoryItems.Bequest_3366545721,
            DefinitionHashes.Records.Commemoration => DefinitionHashes.InventoryItems.Commemoration_4230965989,
            DefinitionHashes.Records.Heritage => DefinitionHashes.InventoryItems.Heritage_4248569242,
            DefinitionHashes.Records.Posterity => DefinitionHashes.InventoryItems.Posterity_3281285075,
            DefinitionHashes.Records.Succession => DefinitionHashes.InventoryItems.Succession_2990047042,
            DefinitionHashes.Records.Trustee => DefinitionHashes.InventoryItems.Trustee_1392919471,
            DefinitionHashes.Records.DeathsRazor => DefinitionHashes.InventoryItems.DeathsRazor_569799274,
            DefinitionHashes.Records.Goldtusk => DefinitionHashes.InventoryItems.Goldtusk_569799275,
            DefinitionHashes.Records.ImperialDecree => DefinitionHashes.InventoryItems.ImperialDecree_2919334548,
            DefinitionHashes.Records.ThroneCleaver => DefinitionHashes.InventoryItems.ThroneCleaver_569799273,
            DefinitionHashes.Records.FaithKeeper => DefinitionHashes.InventoryItems.FaithKeeper_4195186942,
            DefinitionHashes.Records.IllOmen => DefinitionHashes.InventoryItems.IllOmen_3794274730,
            DefinitionHashes.Records.LostSignal => DefinitionHashes.InventoryItems.LostSignal_1197771438,
            DefinitionHashes.Records.SightlineSurvey => DefinitionHashes.InventoryItems.SightlineSurvey_2350330520,
            DefinitionHashes.Records.TimewornWayfarer => DefinitionHashes.InventoryItems.TimewornWayfarer_1058098236,
            DefinitionHashes.Records.VeiledThreat => DefinitionHashes.InventoryItems.VeiledThreat_3685470415,
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
        public const string Lightfall = "Lightfall";
        public const string Opulent = "Opulent";
        public const string RaidCe = "Crota's End";
        public const string RaidDsc = "Deep Stone Crypt";
        public const string RaidKf = "Kings Fall";
        public const string RaidLw = "Last Wish";
        public const string RaidRoN = "Root of Nightmares";
        public const string RaidVotD = "Vow of the Disciple";
        public const string SeasonDeep = "Deep";
        public const string SeasonDefiance = "Defiance";
        public const string SeasonEchoes = "Echoes";
        public const string SeasonHaunted = "Haunted";
        public const string SeasonPlunder = "Plunder";
        public const string SeasonRisen = "Risen";
        public const string SeasonSeraph = "Seraph";
        public const string SeasonUndying = "Undying";
        public const string SeasonWish = "Wish";
        public const string SeasonWitch = "Witch";
        public const string Unknown = "Quest / Unknown";
        public const string Wq = "Witch Queen";
        public const string WqWellspring = "Wellspring";
    }
}
