﻿using Discord;
using DotNetBungieAPI;
using DotNetBungieAPI.AspNet.Security.OAuth.Providers;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Applications;
using Felicity.Extensions;
using Felicity.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;

string[] directoryList = {"Data", "Data/Manifest"};

foreach (var d in directoryList)
    if (!Directory.Exists(d))
        Directory.CreateDirectory(d);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    var bungieApiOptions = new BungieApiOptions();
    builder.Configuration.GetSection("Bungie").Bind(bungieApiOptions);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        var serilogConfig = configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day);

        if (builder.Environment.IsProduction())
        {
            serilogConfig
                .WriteTo.Sentry(o =>
                {
                    o.AttachStacktrace = true;
                    o.Dsn = builder.Configuration.GetSection("SentryDsn").Value;
                    o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                    o.MinimumEventLevel = LogEventLevel.Warning;
                    o.Release = "FelicityOne@6.0.0";
                });
        }
    });

    builder.Services
        .AddDiscord(
            discordClient =>
            {
                discordClient.GatewayIntents = (GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents) |
                                               GatewayIntents.GuildMembers | GatewayIntents.GuildPresences;
                discordClient.AlwaysDownloadUsers = true;
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
                    definitionProvider.ManifestFolderPath = "Data/Manifest";
                    definitionProvider.AutoUpdateManifestOnStartup = true;
                    definitionProvider.FetchLatestManifestOnInitialize = true;
                    definitionProvider.DeleteOldManifestDataAfterUpdates = true;
                })
                .UseDefaultHttpClient(httpClient =>
                {
                    httpClient.SetRatelimitSettings(200, TimeSpan.FromSeconds(10));
                });
        });
    
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
            options.ApiKey = bungieApiOptions.ApiKey;
            options.ClientSecret = bungieApiOptions.ClientSecret;
        });
    
    builder.Services.AddMvc();
    builder.Services
        .AddControllers(options => { options.EnableEndpointRouting = false; });
    builder.Services.AddCors(c =>
    {
        c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
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