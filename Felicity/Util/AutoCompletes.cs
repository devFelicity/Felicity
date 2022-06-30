using Discord;
using Discord.Interactions;
using Felicity.DiscordCommands.Interactions;
using Felicity.Models;
using Felicity.Models.Caches;

namespace Felicity.Util;

public class TwitchStreamAutocomplete : AutocompleteHandler
{
    private readonly TwitchStreamDb _streamDb;

    public TwitchStreamAutocomplete(TwitchStreamDb streamDb)
    {
        _streamDb = streamDb;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        await Task.Delay(0);

        var streamList = _streamDb.TwitchStreams.Where(stream => stream.ServerId == context.Guild.Id).ToList();

        if (streamList.Count == 0)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "No streams found.");

        var resultList = streamList.Select(twitchStream => new AutocompleteResult
        {
            Name = $"{twitchStream.TwitchName} ({context.Guild.GetChannelAsync(twitchStream.ChannelId).Result.Name})",
            Value = twitchStream.Id
        }).ToList();

        return AutocompletionResult.FromSuccess(resultList);
    }
}

public class RunByteAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        await Task.Delay(0);

        var resultList = new List<AutocompleteResult>();
        var i = 0;

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        foreach (var availableByte in FunCommands.AvailableBytes.Where(availableByte =>
                     currentSearch != null && availableByte.ToLower().Contains(currentSearch.ToLower())))
        {
            resultList.Add(new AutocompleteResult { Name = availableByte, Value = i });
            i++;
        }

        return AutocompletionResult.FromSuccess(resultList);
    }
}

public class CheckpointAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        await Task.Delay(0);

        var cpCache = ProcessCpData.ReadJson();

        var result = cpCache?.CpInventory.Checkpoints
            .Select(activeCp => new AutocompleteResult(activeCp.Name, activeCp.Name)).ToList();

        if (result == null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful,
                "Failed to fetch checkpoints from cache.");
        result = result.OrderBy(x => x.Name).ToList();

        result.Add(new AutocompleteResult("--- Others", "Other"));

        return AutocompletionResult.FromSuccess(result);
    }
}

public class MementoWeaponAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        await Task.Delay(0);

        var source = (from autocompleteOption in autocompleteInteraction.Data.Options
            where autocompleteOption.Name == "source"
            select Enum.Parse<MementoSource>(autocompleteOption.Value.ToString() ?? string.Empty)).FirstOrDefault();

        var memCache = ProcessMementoData.ReadJson();

        if (memCache == null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "Memento cache not found.");

        var goodSource = memCache.MementoInventory?.FirstOrDefault(x => x.Source == source);

        if (goodSource?.WeaponList == null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "Memento cache not found.");

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        var results = (from weapon in goodSource.WeaponList
            where currentSearch == null || weapon.WeaponName!.ToLower().Contains(currentSearch.ToLower())
            select new AutocompleteResult { Name = weapon.WeaponName, Value = weapon.WeaponName }).ToList();

        results = results.OrderBy(x => x.Name).ToList();

        return AutocompletionResult.FromSuccess(results);
    }
}