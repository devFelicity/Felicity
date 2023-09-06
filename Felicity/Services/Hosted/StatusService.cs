using Discord;
using Discord.WebSocket;
using Felicity.Util;

namespace Felicity.Services.Hosted;

public class StatusService : BackgroundService
{
    private static readonly List<Game> GameList = new()
    {
        new Game("Destiny 3"),
        new Game("Spire of Stars"),
        new Game("you 👀", ActivityType.Watching),
        new Game("Moons break stuff 🔨", ActivityType.Watching),
        new Game("with fire"),
        new Game("you break the rules", ActivityType.Watching),
        new Game("Juice WRLD", ActivityType.Listening),
        new Game("Google Chrome"),
        new Game("$10k qp tourney", ActivityType.Competing),
        new Game("sweet bird sounds", ActivityType.Listening),
        new Game("Felicity ... wait", ActivityType.Watching),
        new Game($"v.{BotVariables.Version}"),
        new Game("/lookup", ActivityType.Watching),
        new Game("hide & seek in the Tower"),
        new Game("/metrics", ActivityType.Watching),
        new Game("/vendor", ActivityType.Watching),
        new Game("/loot-table", ActivityType.Watching),
        new Game("/recipes", ActivityType.Watching),
        new Game("/memento", ActivityType.Watching),
        new Game("big /checkpoint hurting mom & pop LFG", ActivityType.Watching)
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