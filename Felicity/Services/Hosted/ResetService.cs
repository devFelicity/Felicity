using Discord.WebSocket;
using DotNetBungieAPI.Service.Abstractions;

namespace Felicity.Services.Hosted;

public class ResetService : BackgroundService
{
    private readonly IBungieClient _bungieClient;

    private readonly TimeSpan _delay = TimeSpan.FromMinutes(10);
    private readonly DiscordShardedClient _discordClient;
    private readonly ILogger<ResetService> _logger;

    public ResetService(
        IBungieClient bungieClient,
        DiscordShardedClient discordClient,
        ILogger<ResetService> logger)
    {
        _bungieClient = bungieClient;
        _discordClient = discordClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bungieClient.ResetService.WaitForNextDailyReset(_delay, stoppingToken);

                _logger.LogInformation("Reset task starting");

                switch (DateTime.UtcNow.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        break;
                    case DayOfWeek.Tuesday:
                        // weekly reset
                        break;
                    case DayOfWeek.Wednesday:
                        // gunsmith weird perk reset time
                        break;
                    case DayOfWeek.Thursday:
                        break;
                    case DayOfWeek.Friday:
                        // xur & trials
                        break;
                    case DayOfWeek.Saturday:
                        break;
                    case DayOfWeek.Sunday:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // do stuff
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in ResetService");
        }
    }
}