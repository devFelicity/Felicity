using System.Text.Json.Serialization;
using Felicity.Util;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
#pragma warning disable CS8618

namespace Felicity.Models;

public class CheckpointParser
{
    public static async Task<Checkpoints?> FetchAsync()
    {
        try
        {
            return await HttpClientInstance.Instance.GetFromJsonAsync<Checkpoints>(
                "https://d2cp.io/platform/checkpoints?v=2");
        }
        catch
        {
            // ignored
        }

        return null;
    }
}

public class Checkpoints
{
    [JsonPropertyName("official")] public Official[]? Official { get; set; }

    [JsonPropertyName("community")] public object Community { get; set; }

    [JsonPropertyName("alert")] public Alert Alert { get; set; }
}

public class Alert
{
    [JsonPropertyName("alertActive")] public bool AlertActive { get; set; }

    [JsonPropertyName("alertText")] public string AlertText { get; set; }
}

public class Official
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("activity")] public string Activity { get; set; }

    [JsonPropertyName("activityHash")] public long ActivityHash { get; set; }

    [JsonPropertyName("encounter")] public string Encounter { get; set; }

    [JsonPropertyName("players")] public int Players { get; set; }

    [JsonPropertyName("maxPlayers")] public int MaxPlayers { get; set; }

    [JsonPropertyName("difficultyTier")] public Difficulty DifficultyTier { get; set; }

    [JsonPropertyName("imgURL")] public string ImgUrl { get; set; }

    [JsonPropertyName("iconURL")] public string IconUrl { get; set; }

    [JsonPropertyName("displayOrder")] public int DisplayOrder { get; set; }
}

public enum Difficulty
{
    Normal = 2,
    Master = 3
}