using Discord;
using Discord.WebSocket;
using DotNetBungieAPI;
using DotNetBungieAPI.AspNet.Security.OAuth.Providers;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Applications;
using Felicity.Extensions;
using Felicity.Models;
using Felicity.Options;
using Felicity.Services;
using Felicity.Services.Hosted;
using Felicity.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();

await BotVariables.Initialize();

try
{
    var builder = WebApplication.CreateBuilder(args);

    Console.Title = $"Felicity v.{BotVariables.Version}";

    var bungieApiOptions = new BungieApiOptions();
    builder.Configuration.GetSection("Bungie").Bind(bungieApiOptions);

    EnsureDirectoryExists(bungieApiOptions.ManifestPath!);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        var serilogConfig = configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day);

        if (builder.Environment.IsProduction())
            serilogConfig
                .WriteTo.Sentry(o =>
                {
                    o.AttachStacktrace = true;
                    o.Dsn = builder.Configuration.GetSection("SentryDsn").Value;
                    o.MinimumBreadcrumbLevel = LogEventLevel.Information;
                    o.MinimumEventLevel = LogEventLevel.Warning;
                    o.Release = $"FelicityOne@{BotVariables.Version}";
                });
    });

    builder.Host.UseDefaultServiceProvider(o => o.ValidateScopes = false);

    builder.Services.AddDbContext<UserDb>();
    builder.Services.AddDbContext<ServerDb>();
    builder.Services.AddDbContext<TwitchStreamDb>();

    builder.Services
        .AddDiscord(
            discordClient =>
            {
                discordClient.GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers |
                                               GatewayIntents.GuildPresences;
                discordClient.AlwaysDownloadUsers = true;
            },
            _ => { },
            textCommandsService => { textCommandsService.CaseSensitiveCommands = false; },
            builder.Configuration)
        .UseBungieApiClient(bungieClient =>
        {
            bungieClient.ApiKey = bungieApiOptions.ApiKey;
            bungieClient.ApplicationScopes = ApplicationScopes.ReadBasicUserProfile |
                                             ApplicationScopes.ReadDestinyInventoryAndVault |
                                             ApplicationScopes.MoveEquipDestinyItems;
            bungieClient.CacheDefinitions = true;
            bungieClient.ClientId = bungieApiOptions.ClientId;
            bungieClient.ClientSecret = bungieApiOptions.ClientSecret;
            bungieClient.UsedLocales.Add(BungieLocales.EN);
            bungieClient
                .UseDefaultDefinitionProvider(definitionProvider =>
                {
                    definitionProvider.ManifestFolderPath = bungieApiOptions.ManifestPath;
                    definitionProvider.AutoUpdateManifestOnStartup = true;
                    definitionProvider.FetchLatestManifestOnInitialize = true;
                    definitionProvider.DeleteOldManifestDataAfterUpdates = true;
                })
                .UseDefaultHttpClient(httpClient =>
                {
                    httpClient.SetRatelimitSettings(200, TimeSpan.FromSeconds(10));
                });
        })
        .AddHostedService<BungieClientStartupService>()
        .AddSingleton<LogAdapter<BaseSocketClient>>();

    builder.Services.AddHostedService<TwitchClientStartupService>();

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = BungieNetAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = BungieNetAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddBungieNet(options =>
        {
            options.ClientId = bungieApiOptions.ClientId.ToString();
            options.ApiKey = bungieApiOptions.ApiKey!;
            options.ClientSecret = bungieApiOptions.ClientSecret!;
            options.Events = new OAuthEvents
            {
                OnCreatingTicket = oAuthCreatingTicketContext =>
                {
                    BungieAuthCacheService.TryAddContext(oAuthCreatingTicketContext);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddMvc();
    builder.Services
        .AddControllers(options => { options.EnableEndpointRouting = false; })
        .AddJsonOptions(x => { BungieAuthCacheService.Initialize(x.JsonSerializerOptions); });
    builder.Services.AddCors(c =>
    {
        c.AddPolicy("AllowOrigin",
            options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });

    var app = builder.Build();

    app.UseRouting();

    app.UseCookiePolicy(new CookiePolicyOptions
    {
        Secure = CookieSecurePolicy.Always
    });
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseMvc();

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

static void EnsureDirectoryExists(string path)
{
    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
}