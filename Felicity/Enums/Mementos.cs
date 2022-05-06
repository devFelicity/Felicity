using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace Felicity.Enums;

public class Mementos
{
    [J("MementoList")] public MementoList MementoList { get; set; }
}

public class MementoList
{
    [J("gambit")] public MementoCat[] Gambit { get; set; }
    [J("trials")] public MementoCat[] Trials { get; set; }
    [J("nightfall")] public MementoCat[] Nightfall { get; set; }
}

public class MementoCat
{
    [J("name")] public string Name { get; set; }
    [J("url")] public string Url { get; set; }
    [J("author")] public string Author { get; set; }
    [J("source")] public MementoSource Source { get; set; }
}

public enum MementoType
{
    Gambit,
    Nightfall,
    Trials
}

public enum MementoSource
{
    OpenWorld,
    RaidVotD,
    SeasonRisen,
    ThroneWorld
}

public static class KnownMementos
{
    private const string j = "/u/TheLastJoaquin";
    private const string k = "@KimberPrime";

    public static readonly Mementos KnownMementoList = new()
    {
        MementoList = new MementoList
        {
            Gambit = new[]
            {
                new MementoCat
                {
                    Author = j, Name = "Come to Pass", Url = "https://i.imgur.com/x2QrFor.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Fel Taradiddle", Url = "https://i.imgur.com/Rp0bPFX.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Pointed Inquiry", Url = "https://i.imgur.com/NKLVlfz.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Empirical Evidence", Url = "https://i.imgur.com/Nb183fW.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = k, Name = "Forensic Nightmare",
                    Url = "https://whaskell.pw/images/mementos/gambit-forensicnightmare.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Likely Suspect", Url = "https://i.imgur.com/eHhF9lG.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Father's Sins", Url = "https://i.imgur.com/3ErpRLH.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Tarnation", Url = "https://i.imgur.com/W3IXfuo.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = k, Name = "Red Herring", Url = "https://whaskell.pw/images/mementos/gambit-redherring.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "The Enigma", Url = "https://i.imgur.com/7NRVAFP.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = k, Name = "Sweet Sorrow",
                    Url = "https://whaskell.pw/images/mementos/gambit-sweetsorrow.jpg",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Under Your Skin", Url = "https://i.imgur.com/cdUF9NJ.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Piece of Mind", Url = "https://i.imgur.com/D60FmkO.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Explosive Personality", Url = "https://i.imgur.com/0Th1ku7.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Thoughtless", Url = "https://i.imgur.com/3EMhMbc.jpg",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Recurrent Impact", Url = "https://i.imgur.com/GA0DzYI.jpg",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Syncopation-53", Url = "https://i.imgur.com/yw3LIc6.png",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Palmyra-B", Url = "https://i.imgur.com/tfrZUgX.png",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Ragnhild-D", Url = "https://i.imgur.com/6qpL58g.jpg",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Insidious", Url = "https://i.imgur.com/IQvkq2K.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Submission", Url = "https://i.imgur.com/5szksQU.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = k, Name = "Deliverance",
                    Url = "https://whaskell.pw/images/mementos/gambit-deliverance.jpg", Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Lubrae's Ruin", Url = "https://i.imgur.com/0yFQLqy.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = k, Name = "Forbearance",
                    Url = "https://whaskell.pw/images/mementos/gambit-forbearance.jpg", Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Cataclysmic",
                    Url = "https://whaskell.pw/images/mementos/gambit-cataclysmic.jpg", Source = MementoSource.RaidVotD
                }
            },
            Trials = new[]
            {
                new MementoCat
                {
                    Author = j, Name = "Come to Pass", Url = "https://i.imgur.com/2ZFfjn1.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Fel Taradiddle", Url = "https://i.imgur.com/ZbJvxUE.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Pointed Inquiry", Url = "https://i.imgur.com/ZB66Fwf.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Empirical Evidence", Url = "https://i.imgur.com/2ZgjRBq.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Forensic Nightmare", Url = "https://i.imgur.com/kJ52jH3.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Likely Suspect", Url = "https://i.imgur.com/eQJPuzy.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Father's Sins", Url = "https://i.imgur.com/I7TZMxg.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Tarnation", Url = "https://i.imgur.com/H1zalBK.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = k, Name = "Red Herring", Url = "https://whaskell.pw/images/mementos/trials-redherring.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "The Enigma", Url = "https://whaskell.pw/images/mementos/trials-enigma.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = k, Name = "Sweet Sorrow",
                    Url = "https://whaskell.pw/images/mementos/trials-sweetsorrow.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Under Your Skin", Url = "https://i.imgur.com/e6Ro0MS.jpg",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Piece of Mind", Url = "https://i.imgur.com/Xx8crt0.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Explosive Personality", Url = "https://i.imgur.com/kZl9wrt.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Thoughtless", Url = "https://i.imgur.com/o35RbZL.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Recurrent Impact", Url = "https://i.imgur.com/LChULDI.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Syncopation-53", Url = "https://i.imgur.com/l2GlTYp.jpg",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Palmyra-B", Url = "https://i.imgur.com/NQgYYHJ.png",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Ragnhild-D", Url = "https://i.imgur.com/jECv7ub.png",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Insidious", Url = "https://i.imgur.com/5bUUOCb.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Submission", Url = "https://i.imgur.com/0T7Auh7.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Deliverance", Url = "https://i.imgur.com/y52ADs5.jpg",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Lubrae's Ruin", Url = "https://i.imgur.com/pZ4uJWX.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Forbearance", Url = "https://i.imgur.com/5nRKCxq.jpg",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Cataclysmic", Url = "https://i.imgur.com/VyTTWhp.png",
                    Source = MementoSource.RaidVotD
                }
            },
            Nightfall = new[]
            {
                new MementoCat
                {
                    Author = j, Name = "Come to Pass", Url = "https://i.imgur.com/EtG12bD.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Fel Taradiddle", Url = "https://i.imgur.com/VtP8rwj.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Pointed Inquiry", Url = "https://i.imgur.com/UddBSZo.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Empirical Evidence", Url = "https://i.imgur.com/KmNZGRK.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Forensic Nightmare", Url = "https://i.imgur.com/DP6wY2g.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Likely Suspect", Url = "https://i.imgur.com/aIOYy6a.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Father's Sins", Url = "https://i.imgur.com/8FT6oeE.jpg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Tarnation", Url = "https://i.imgur.com/yexddiy.jpeg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Red Herring", Url = "https://i.imgur.com/P8YZddt.png",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "The Enigma", Url = "https://i.imgur.com/2Nht3gk.jpeg",
                    Source = MementoSource.ThroneWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Sweet Sorrow", Url = "https://i.imgur.com/YlLXAbW.jpg",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Under Your Skin", Url = "https://i.imgur.com/JxizPFX.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Piece of Mind", Url = "https://whaskell.pw/images/mementos/nf-pieceofmind.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Explosive Personality",
                    Url = "https://whaskell.pw/images/mementos/nf-explosivepersonality.jpg",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Thoughtless", Url = "https://i.imgur.com/KvvYXDk.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Recurrent Impact", Url = "https://i.imgur.com/TIR2Nqj.png",
                    Source = MementoSource.SeasonRisen
                },
                new MementoCat
                {
                    Author = j, Name = "Syncopation-53", Url = "https://i.imgur.com/EqejQQo.jpeg",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Palmyra-B", Url = "https://i.imgur.com/nCvBsuz.jpeg",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Ragnhild-D", Url = "https://i.imgur.com/ooNyaxu.png",
                    Source = MementoSource.OpenWorld
                },
                new MementoCat
                {
                    Author = j, Name = "Insidious", Url = "https://whaskell.pw/images/mementos/nf-insidious.jpg",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Submission", Url = "https://i.imgur.com/kBWBM9P.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Deliverance", Url = "https://i.imgur.com/d4hl5HR.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Lubrae's Ruin", Url = "https://i.imgur.com/0w8je04.jpeg",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Forbearance", Url = "https://i.imgur.com/bMn02v9.png",
                    Source = MementoSource.RaidVotD
                },
                new MementoCat
                {
                    Author = j, Name = "Cataclysmic", Url = "https://i.imgur.com/qa0WHhw.png",
                    Source = MementoSource.RaidVotD
                }
            }
        }
    };
}