using System.Collections.Concurrent;
using System.Text.Json;
using DotNetBungieAPI.Authorization;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Felicity.Services;

public static class BungieAuthCacheService
{
    private static ConcurrentDictionary<long, (OAuthCreatingTicketContext Context, AuthorizationTokenData Token)>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
#pragma warning disable IDE0044 // Add readonly modifier
        _authContexts = new();
#pragma warning restore IDE0044 // Add readonly modifier

    private static JsonSerializerOptions? _jsonSerializerOptions;

    public static void Initialize(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public static void TryAddContext(OAuthCreatingTicketContext authCreatingTicketContext)
    {
        var tokenData =
            authCreatingTicketContext.TokenResponse.Response!.Deserialize<AuthorizationTokenData>(
                _jsonSerializerOptions);
        _authContexts.TryAdd(tokenData!.MembershipId, (authCreatingTicketContext, tokenData));
    }

    public static bool GetByIdAndRemove(long id,
        out (OAuthCreatingTicketContext Context, AuthorizationTokenData Token) context)
    {
        return _authContexts.TryRemove(id, out context);
    }
}