using Discord;
using Discord.WebSocket;
using Felicity.Util;
using Serilog;

namespace Felicity.Services.Hosted;

public class StatusService : BackgroundService
{
    private static readonly List<Game> GameList = new()
    {
        new Game("Destiny 3"),
        new Game("Spire of Stars"),
        new Game("you 👀", ActivityType.Watching),
        new Game("Leaf break stuff 🔨", ActivityType.Watching),
        new Game("with fire"),
        new Game("you break the rules", ActivityType.Watching),
        new Game("Juice WRLD", ActivityType.Listening),
        new Game("Google Chrome"),
        new Game("$10k qp tourney", ActivityType.Competing),
        new Game("ttv/fake_positivity", ActivityType.Watching),
        new Game("sweet bird sounds", ActivityType.Listening),
        new Game("Felicity ... wait", ActivityType.Watching),
        new Game($"v.{BotVariables.Version}"),
        new Game("/lookup", ActivityType.Watching),
        new Game("/metrics", ActivityType.Watching),
        new Game("/vendor", ActivityType.Watching),
        new Game("/loot-table", ActivityType.Watching),
        new Game("/recipes", ActivityType.Watching),
        new Game("/memento", ActivityType.Watching)
    };

    private readonly TimeSpan _delay = TimeSpan.FromMinutes(15);
    private readonly DiscordShardedClient _discordClient;

    public StatusService(DiscordShardedClient discordClient)
    {
        _discordClient = discordClient;
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
                    Log.Information($"Set game to: {newGame.Name}");
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
            Console.WriteLine($"Exception in StatusService\n{e.GetType()}: {e.Message}");
        }
    }
}