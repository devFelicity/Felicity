using Serilog;

namespace Felicity.Services.Hosted;

public class TwitchStartupService : BackgroundService
{
    private readonly TwitchService _twitchService;

    public TwitchStartupService(TwitchService twitchService)
    {
        _twitchService = twitchService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _twitchService.ConfigureMonitor();
        }
        catch (Exception e)
        {
            Log.Error($"Exception in BungieClientStartupService\n{e.GetType()}: {e.Message}");
        }

        return Task.CompletedTask;
    }
}