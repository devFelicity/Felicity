using Discord.WebSocket;
using DotNetBungieAPI.Service.Abstractions;
using Serilog;

namespace Felicity.Services.Hosted;

public class ResetService : BackgroundService
{
    private readonly IBungieClient _bungieClient;
    private readonly DiscordShardedClient _discordClient;

    private readonly TimeSpan _delay = TimeSpan.FromMinutes(10);

    public ResetService(IBungieClient bungieClient, DiscordShardedClient discordClient)
    {
        _bungieClient = bungieClient;
        _discordClient = discordClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bungieClient.ResetService.WaitForNextDailyReset(_delay, stoppingToken);

                Log.Information("Reset task starting.");

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
            Log.Error($"Exception in ResetService\n{e.GetType()}: {e.Message}");
        }
    }
}