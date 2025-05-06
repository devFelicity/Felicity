using Discord;
using Discord.WebSocket;
using Felicity.Util;

namespace Felicity.Services.Hosted;

public class StatusService : BackgroundService
{
    private static readonly List<Game> GameList = new()
    {
        new Game("Shutting down May 9th, use Levante.")
    };

    private readonly TimeSpan _delay = TimeSpan.FromMinutes(15);
    private readonly DiscordShardedClient _discordClient;
    private readonly ILogger<StatusService> _logger;

    public StatusService(
        DiscordShardedClient discordClient,
        ILogger<StatusService> logger)
    {
        _discordClient = discordClient;
        _logger = logger;
    }

    private static Game LastGame { get; set; } = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                Game newGame;
                do
                {
                    newGame = GameList[Random.Shared.Next(GameList.Count)];
                } while (newGame == LastGame);

                try
                {
                    await _discordClient.SetActivityAsync(newGame);
                    _logger.LogInformation("Set game to: {Name}", newGame.Name);
                    LastGame = newGame;
                }
                catch
                {
                    // ignored
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in StatusService");
        }
    }
}
