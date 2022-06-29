using Discord;
using Discord.Interactions;
using Felicity.DiscordCommands.Interactions;
using Felicity.Models;

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

        foreach (var availableByte in FunCommands.AvailableBytes.Where(availableByte => currentSearch != null && availableByte.ToLower().Contains(currentSearch.ToLower())))
        {
            resultList.Add(new AutocompleteResult{Name = availableByte, Value = i});
            i++;
        }

        return AutocompletionResult.FromSuccess(resultList);
    }
}