using System.Text.Json;
using System.Text.Json.Serialization;
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Util;

public class EmblemReport
{
    public partial class EmblemResponse
    {
        [JsonPropertyName("data")]
#pragma warning disable CS8618
        public List<Datum> Data { get; set; }
#pragma warning restore CS8618
    }

    public partial class Datum
    {
        [JsonPropertyName("collectible_hash")]
        public uint CollectibleHash { get; set; }

        [JsonPropertyName("acquisition")]
        public long Acquisition { get; set; }

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
    }

    public partial class EmblemResponse
    {
        public static EmblemResponse? FromJson(string json) 
            => JsonSerializer.Deserialize<EmblemResponse>(json);
    }
}