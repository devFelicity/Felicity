using Discord;
using Discord.WebSocket;

namespace FelicityOne.Services;

internal static class StatusService
{
    private static readonly Random rnd = new();

    private static readonly List<Game> gameList = new()
    {
        new Game("Destiny 3"),
        new Game("you 👀", ActivityType.Watching),
        new Game("Leaf break stuff 🔨", ActivityType.Watching),
        new Game("what does air taste like?", ActivityType.CustomStatus),
        new Game("you break the rules", ActivityType.Watching),
        new Game("Juice WRLD", ActivityType.Listening),
        new Game("Google Chrome"),
        new Game("$10k qp tourney", ActivityType.Competing),
        new Game("Pornhub VR"),
        new Game("ttv/purechill", ActivityType.Watching),
        new Game("sweet bird sounds", ActivityType.Listening),
        new Game("Felicity ... wait", ActivityType.Watching),
        new Game($"v{ConfigService.GetBotSettings().Version}")
    };

    private static Game LastGame { get; set; } = null!;
    public static DiscordSocketClient DiscordClient { get; set; } = null!;

    public static async void ChangeGame()
    {
        Game newGame;
        do
        {
            newGame = gameList[rnd.Next(gameList.Count)];
        } while (newGame == LastGame);

        try
        {
            await Task.Delay(1000);
            if (!Felicity.IsDebug())
                await DiscordClient.SetActivityAsync(newGame);
            LastGame = newGame;
        }
        catch
        {
            // ignored
        }
    }
}