using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace Felicity.Services;

internal static class StatusService
{
    private static readonly Random rnd = new();

    private static readonly List<Game> gameList = new()
    {
        new Game("Clan Graphic on top", ActivityType.Watching),
        new Game("Destiny 3"),
        new Game("you 👀", ActivityType.Watching),
        new Game("Leaf break stuff 🔨", ActivityType.Watching),
        new Game("you break the rules", ActivityType.Watching),
        new Game("Juice WRLD", ActivityType.Listening),
        new Game("Google Chrome"),
        new Game("Pornhub VR"),
        new Game("ttv/purechill", ActivityType.Watching),
        new Game("🍂 ranting", ActivityType.Listening)
    };

    public static DiscordSocketClient _client;

    private static Game LastGame { get; set; }

    public static async void ChangeGame()
    {
        Game newGame;
        do
        {
            newGame = gameList[rnd.Next(gameList.Count)];
        } while (newGame == LastGame);

        await _client.SetActivityAsync(newGame);

        LastGame = newGame;
    }
}