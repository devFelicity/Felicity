using Discord;
using Discord.WebSocket;
using DotNetBungieAPI;
using DotNetBungieAPI.AspNet.Security.OAuth.Providers;
using DotNetBungieAPI.DefinitionProvider.Sqlite;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Applications;
using DotNetBungieAPI.Models.Destiny;
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
    .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

try
{
    await BotVariables.Initialize();
    var builder = WebApplication.CreateBuilder(args);
    var title = $"Starting Felicity v.{BotVariables.Version} on {Environment.OSVersion}...";
    Console.Title = title;
    Log.Information(title);
    
    if (!BotVariables.IsDebug)
        builder.WebHost.UseSentry(options =>
        {
            options.AttachStacktrace = true;
            options.Dsn = builder.Configuration.GetSection("SentryDsn").Value;
            options.MinimumBreadcrumbLevel = LogLevel.Information;
            options.MinimumEventLevel = LogLevel.Warning;
            options.Release = $"FelicityOne@{BotVariables.Version}";
        });

    var bungieApiOptions = new BungieApiOptions();
    builder.Configuration.GetSection("Bungie").Bind(bungieApiOptions);

    EnsureDirectoryExists(bungieApiOptions.ManifestPath!);
    EnsureDirectoryExists("Data");

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        var serilogConfig = configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day);

        if (!BotVariables.IsDebug)
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

    builder.Services.AddDbContext<MetricDb>();
    builder.Services.AddDbContext<UserDb>();
    builder.Services.AddDbContext<ServerDb>();
    builder.Services.AddDbContext<TwitchStreamDb>();

    builder.Services
        .AddDiscord(
            discordClient =>
            {
                discordClient.GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildInvites &
                                               ~GatewayIntents.GuildScheduledEvents;
                discordClient.AlwaysDownloadUsers = false;
            },
            _ => { },
            textCommandsService => { textCommandsService.CaseSensitiveCommands = false; },
            builder.Configuration)
        .AddLogging(options => options.AddSerilog(dispose: true))
        .UseBungieApiClient(bungieClient =>
        {
            if (bungieApiOptions.ApiKey != null)
                bungieClient.ClientConfiguration.ApiKey = bungieApiOptions.ApiKey;

            bungieClient.ClientConfiguration.ApplicationScopes = ApplicationScopes.ReadUserData |
                                                                 ApplicationScopes.ReadBasicUserProfile |
                                                                 ApplicationScopes.ReadDestinyInventoryAndVault |
                                                                 ApplicationScopes.MoveEquipDestinyItems;

            bungieClient.ClientConfiguration.CacheDefinitions = true;
            bungieClient.ClientConfiguration.ClientId = bungieApiOptions.ClientId;

            if (bungieApiOptions.ClientSecret != null)
                bungieClient.ClientConfiguration.ClientSecret = bungieApiOptions.ClientSecret;

            bungieClient.ClientConfiguration.UsedLocales.Add(BungieLocales.EN);
            bungieClient
                .DefinitionProvider.UseSqliteDefinitionProvider(definitionProvider =>
                {
                    definitionProvider.ManifestFolderPath = bungieApiOptions.ManifestPath;
                    definitionProvider.AutoUpdateManifestOnStartup = true;
                    definitionProvider.FetchLatestManifestOnInitialize = true;
                    definitionProvider.DeleteOldManifestDataAfterUpdates = true;
                });
            bungieClient.DotNetBungieApiHttpClient.ConfigureDefaultHttpClient(options =>
                options.SetRateLimitSettings(190, TimeSpan.FromSeconds(10)));
            bungieClient.DefinitionRepository.ConfigureDefaultRepository(x =>
            {
                var defToIgnore = Enum.GetValues<DefinitionsEnum>()
                    .FirstOrDefault(y => y == DefinitionsEnum.DestinyTraitCategoryDefinition);

                x.IgnoreDefinitionType(defToIgnore);
            });
        })
        .AddHostedService<BungieClientStartupService>()
        .AddSingleton<LogAdapter<BaseSocketClient>>();

    builder.Services.Configure<TwitchOptions>(builder.Configuration.GetSection("Twitch")).AddSingleton<TwitchService>();
    builder.Services.AddHostedService<TwitchStartupService>();
    builder.Services.AddHostedService<ResetService>();
    builder.Services.AddHostedService<StatusService>();

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

    if (!BotVariables.IsDebug)
        app.UseSentryTracing();

    app.UseCookiePolicy(new CookiePolicyOptions
    {
        Secure = CookieSecurePolicy.Always
    });
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseMvc();
    // app.UseHttpsRedirection();
    if (!app.Environment.IsDevelopment())
        app.UseHsts();

    app.MapGet("/health", () => Results.Ok());

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

return;

static void EnsureDirectoryExists(string path)
{
    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
}