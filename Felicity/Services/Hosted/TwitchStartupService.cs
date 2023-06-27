namespace Felicity.Services.Hosted;

public class TwitchStartupService : BackgroundService
{
    private readonly TwitchService _twitchService;
    private readonly ILogger<TwitchStartupService> _logger;

    public TwitchStartupService(
        TwitchService twitchService,
        ILogger<TwitchStartupService> logger)
    {
        _twitchService = twitchService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _twitchService.ConfigureMonitor();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in TwitchStartupService");
        }

        return Task.CompletedTask;
    }
}