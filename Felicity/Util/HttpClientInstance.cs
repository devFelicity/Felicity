namespace Felicity.Util;

public static class HttpClientInstance
{
    private static readonly HttpClient? _httpClient;

    public static HttpClient Instance => _httpClient ?? new HttpClient();
}
