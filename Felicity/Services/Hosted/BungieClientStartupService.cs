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
        // TODO: if run on machines with more ram, uncomment this and use repository
        // await _bungieClient.DefinitionProvider.ReadToRepository(_bungieClient.Repository);
    }
}