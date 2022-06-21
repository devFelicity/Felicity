using DotNetBungieAPI;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Applications;
using Felicity.Extensions;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Sentry(o =>
    {
        o.AttachStacktrace = true;
        o.Dsn = "Get sentry Dsn from config here";
        o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
        o.MinimumEventLevel = LogEventLevel.Warning;
        o.Release = $"FelicityOne@{"Get version here"}";
    })
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day)
            .WriteTo.Sentry(o =>
            {
                o.AttachStacktrace = true;
                o.Dsn = "Get sentry Dsn from config here";
                o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                o.MinimumEventLevel = LogEventLevel.Warning;
                o.Release = $"FelicityOne@{"Get version here"}";
            });
    });

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
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}