using DotNetBungieAPI.HashReferences;

namespace Felicity.Util.Enums;

internal static class Craftables
{
    public static readonly Dictionary<string, List<uint>> CraftableList = new()
    {
        {
            "Haunted", new List<uint>
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
            "Leviathan", new List<uint>
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
            "Risen", new List<uint>
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
            "Vow of the Disciple", new List<uint>
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
            "Wellspring", new List<uint>
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
            "Witch Queen", new List<uint>
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
}