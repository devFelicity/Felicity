using DotNetBungieAPI;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Applications;
using Felicity.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDiscord(
        discordClient =>
        {
            // configure your client here
        },
        interactionService =>
        {
            // configure your interaction service here
        },
        textCommandsService =>
        {
            // configure your text commands service here
        },
        builder.Configuration)
    .UseBungieApiClient(bungieClient =>
    {
        bungieClient.ApiKey = "ApiKey";
        bungieClient.ApplicationScopes = ApplicationScopes.ReadBasicUserProfile;
        bungieClient.CacheDefinitions = true;
        bungieClient.ClientId = 123;
        bungieClient.ClientSecret = "secret_here";
        bungieClient.UsedLocales.Add(BungieLocales.EN);
        bungieClient
            .UseDefaultDefinitionProvider(definitionProvider =>
            {
                definitionProvider.ManifestFolderPath = "path where all manifests would be stored";
                definitionProvider.AutoUpdateManifestOnStartup = true;
                definitionProvider.FetchLatestManifestOnInitialize = true;
                definitionProvider.DeleteOldManifestDataAfterUpdates = false;
            })
            .UseDefaultHttpClient(httpClient =>
            {
                httpClient.SetRatelimitSettings(200, TimeSpan.FromSeconds(10));
            });
    });

var app = builder.Build();
await app.RunAsync();