// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Options;

public class BungieApiOptions
{
    public string? ApiKey { get; set; }
    public int ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ManifestPath { get; set; }
    public string? EmblemReportApiKey { get; set; }
}
