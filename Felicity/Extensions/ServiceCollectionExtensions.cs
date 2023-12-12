using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Felicity.Options;
using Felicity.Services.Hosted;
using Fergun.Interactive;

namespace Felicity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscord(
        this IServiceCollection serviceCollection,
        Action<DiscordSocketConfig> configureClient,
        Action<InteractionServiceConfig> configureInteractionService,
        Action<CommandServiceConfig> configureTextCommands,
        IConfiguration configuration)
    {
        var discordSocketConfig = new DiscordSocketConfig();
        configureClient(discordSocketConfig);
        var discordClient = new DiscordShardedClient(discordSocketConfig);

        var interactionServiceConfig = new InteractionServiceConfig();
        configureInteractionService(interactionServiceConfig);
        var interactionService = new InteractionService(discordClient, interactionServiceConfig);

        var commandServiceConfig = new CommandServiceConfig();
        configureTextCommands(commandServiceConfig);
        var textCommandService = new CommandService(commandServiceConfig);

        return serviceCollection
            .Configure<DiscordBotOptions>(configuration.GetSection("DiscordBot"))
            .AddHostedService<DiscordStartupService>()
            .AddSingleton(discordClient)
            .AddSingleton(interactionService)
            .AddSingleton(new InteractiveConfig { DefaultTimeout = TimeSpan.FromMinutes(2) })
            .AddSingleton<InteractiveService>()
            .AddSingleton(textCommandService);
    }
}
