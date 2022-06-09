namespace FelicityOne.Enums;

internal static class Craftables
{
    public static readonly Dictionary<string, List<uint>> craftableList = new()
    {
        {
            "Haunted", new List<uint>
            {
                2330294643, // bump in the night
                2511032639, // firefright
                2177815995, // hollow denial
                1691359215, // nezarec's whisper
                3693097974, // tears of contrition
                2126241222  // without remorse
            }
        },
        {
            "Leviathan", new List<uint>
            {
                1444412985, // austringer
                662547074,  // beloved
                2258742229, // calus mini-tool
                826543773,  // drang (baroque)
                2547048326, // fixed odds
                1968204778  // epicurean
            }
        },
        {
            "Risen", new List<uint>
            {
                2115207174, // explosive personality
                3343946430, // piece of mind
                3357460834, // recurrent impact
                1701593813, // sweet sorrow
                3842442685, // thoughtless
                930979915   // under your skin
            }
        },
        {
            "Vow of the Disciple", new List<uint>
            {
                989023188,  // cataclysmic
                2896258222, // deliverance
                422252754,  // forbearance
                3868889639, // insidious
                876397380,  // lubrae's ruin
                1057921323  // submission
            }
        },
        {
            "Wellspring", new List<uint>
            {
                3863516258, // come to pass
                1955149226, // action
                3296489718, // concurrence
                96042291,   // intent
                311360599,  // father's sins
                3907981638, // fel taradiddle
                1507884969  // tarnation
            }
        },
        {
            "Witch Queen", new List<uint>
            {
                4205772441, // empirical evidence
                1615038969, // forensic nightmare
                2107474614, // likely suspect
                403175710,  // osteo striga
                292832740,  // palmyra
                689439417,  // pointed inquiry
                220342896,  // ragnhild
                3770251030, // red herring
                953028525,  // syncopation
                1446423643  // the enigma
            }
        }
    };
}