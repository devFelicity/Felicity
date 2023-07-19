using Discord;
using Discord.WebSocket;
using Felicity.Util;

namespace Felicity.Services.Hosted;

public class PepitoService : BackgroundService
{
    private const string FileName = "Data/pepito.txt";

    private readonly TimeSpan _delay = TimeSpan.FromMinutes(1);
    private readonly DiscordShardedClient _discordClient;
    private readonly ILogger<StatusService> _logger;

    public PepitoService(
        DiscordShardedClient discordClient,
        ILogger<StatusService> logger)
    {
        _discordClient = discordClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!File.Exists(FileName))
                File.Create(FileName);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var client = new HttpClient();

                var nowTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var result = await client.GetStringAsync($"https://is-pepito-out.vercel.app/?{nowTime}", stoppingToken);

                if (!string.IsNullOrEmpty(result))
                {
                    var status =
                        result.Split("<h1 class=\"text-white text-3xl lg:text-4xl opacity-50 text-center\">")[1]
                            .Split("(")[0]
                            .Replace("<!-- -->", "")
                            .TrimEnd() + "!";
                    var currentStatus = await File.ReadAllTextAsync(FileName, stoppingToken);

                    if (currentStatus != status)
                    {
                        await File.WriteAllTextAsync("Data/pepito.txt", status, stoppingToken);

                        var embed = Embeds.MakeBuilder();
                        embed.ThumbnailUrl =
                            "https://pbs.twimg.com/profile_images/909485797438640128/sO-YvCEm_400x400.jpg";
                        embed.Color = Color.DarkMagenta;
                        embed.Description = status;
                        embed.Title = "Pépito Status Update";

                        await ((ITextChannel)_discordClient.GetGuild(822907350217392160).GetChannel(822907350712189011))
                            .SendMessageAsync("<@&1131358985375719484>", embed: embed.Build());
                    }
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in PepitoService");
        }
    }
}