using DotNetBungieAPI.Service.Abstractions;

namespace Felicity.Services.Hosted;

public class BungieClientStartupService : BackgroundService
{
    private readonly IBungieClient _bungieClient;
    private readonly ILogger<BungieClientStartupService> _logger;

    public BungieClientStartupService(
        IBungieClient bungieClient,
        ILogger<BungieClientStartupService> logger)
    {
        _bungieClient = bungieClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _bungieClient.DefinitionProvider.Initialize();
            await _bungieClient.DefinitionProvider.ReadToRepository(_bungieClient.Repository);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in BungieClientStartupService");
        }
    }
}