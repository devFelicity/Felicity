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
        _twitchService.ConfigureMonitor();

        return Task.CompletedTask;
    }
}