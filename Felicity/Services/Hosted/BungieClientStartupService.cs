using DotNetBungieAPI.Clients;

namespace Felicity.Services.Hosted;

public class BungieClientStartupService : BackgroundService
{
    private readonly IBungieClient _bungieClient;

    public BungieClientStartupService(
        IBungieClient bungieClient)
    {
        _bungieClient = bungieClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _bungieClient.DefinitionProvider.Initialize();
        await _bungieClient.DefinitionProvider.ReadToRepository(_bungieClient.Repository);
    }
}