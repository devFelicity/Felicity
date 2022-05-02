using System;
using APIHelper;
using BungieSharper.Client;
using Felicity.Helpers;

namespace Felicity.Services;

internal static class APIService
{
    internal static BungieApiClient GetApiClient()
    {
        var config = ConfigHelper.GetBotSettings();

        return API.GetApiClient(config.BungieApiKey,
            Convert.ToInt32(config.BungieClientId),
            config.BungieClientSecret);
    }
}