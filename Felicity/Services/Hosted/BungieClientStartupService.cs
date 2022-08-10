using DotNetBungieAPI.Service.Abstractions;
using Serilog;

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
        try
        {
            await _bungieClient.DefinitionProvider.Initialize();
            await _bungieClient.DefinitionProvider.ReadToRepository(_bungieClient.Repository);
        }
        catch (Exception e)
        {
            Log.Error($"Exception in BungieClientStartupService\n{e.GetType()}: {e.Message}");
        }
    }
}