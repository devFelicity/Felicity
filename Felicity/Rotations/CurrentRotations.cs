using System;
using Felicity.Enums;
using Newtonsoft.Json;

namespace Felicity.Rotations;

internal class CurrentRotations
{
    [JsonProperty("DailyResetTimestamp")]
    public static DateTime DailyResetTimestamp = DateTime.Now;

    [JsonProperty("WeeklyResetTimestamp")]
    public static DateTime WeeklyResetTimestamp = DateTime.Now;

    // Dailies

    [JsonProperty("LostSector")]
    public static LostSector LostSector;

    [JsonProperty("LostSectorArmorDrop")]
    public static LostSectorReward LostSectorArmorDrop;

    [JsonProperty("AltarWeapon")]
    public static AltarsOfSorrow AltarWeapon;

    [JsonProperty("Wellspring")]
    public static Wellspring Wellspring;

    // Weeklies

    [JsonProperty("LWChallengeEncounter")]
    public static LastWish LWChallengeEncounter;

    [JsonProperty("DSCChallengeEncounter")]
    public static DeepStoneCrypt DSCChallengeEncounter;

    /*
    [JsonProperty("GoSChallengeEncounter")]
    public static GardenOfSalvationEncounter GoSChallengeEncounter;

    [JsonProperty("VoGChallengeEncounter")]
    public static VaultOfGlassEncounter VoGChallengeEncounter;

    [JsonProperty("VowChallengeEncounter")]
    public static VowOfTheDiscipleEncounter VotDChallengeEncounter;

    [JsonProperty("CurseWeek")]
    public static CurseWeek CurseWeek;

    [JsonProperty("AscendantChallenge")]
    public static AscendantChallenge AscendantChallenge;

    [JsonProperty("Nightfall")]
    public static Nightfall Nightfall;

    [JsonProperty("NightfallWeaponDrops")]
    public static NightfallWeapon NightfallWeaponDrops;

    [JsonProperty("EmpireHunt")]
    public static EmpireHunt EmpireHunt;

    [JsonProperty("NightmareHunts")]
    public static NightmareHunt[] NightmareHunts;
    */
}