﻿using Discord;
using Discord.Interactions;
using DotNetBungieAPI.Extensions;
using DotNetBungieAPI.Models.Destiny.Definitions.Metrics;
using DotNetBungieAPI.Service.Abstractions;
using Felicity.Models;
using Felicity.Models.Caches;
using Felicity.Util.Enums;
using Microsoft.EntityFrameworkCore;

namespace Felicity.Util;

public class TwitchStreamAutocomplete : AutocompleteHandler
{
    private readonly TwitchStreamDb _streamDb;

    public TwitchStreamAutocomplete(TwitchStreamDb streamDb)
    {
        _streamDb = streamDb;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var streamList = await _streamDb.TwitchStreams.Where(stream => stream.ServerId == context.Guild.Id)
            .ToListAsync();

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

public class MetricAutocomplete : AutocompleteHandler
{
    private readonly IBungieClient _bungieClient;

    public MetricAutocomplete(IBungieClient bungieClient)
    {
        _bungieClient = bungieClient;
    }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var resultList = new List<AutocompleteResult>();

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        var metricsList = _bungieClient.Repository.GetAll<DestinyMetricDefinition>();

        if (currentSearch != null)
            resultList.AddRange(from destinyMetricDefinition in metricsList
                where destinyMetricDefinition.DisplayProperties.Name.ToLower().Contains(currentSearch.ToLower())
                select new AutocompleteResult(
                    $"{destinyMetricDefinition.DisplayProperties.Name} ({destinyMetricDefinition.Traits.Last().Select(x => x.DisplayProperties.Name)})",
                    destinyMetricDefinition.Hash));
        else
            resultList.AddRange(from destinyMetricDefinition in metricsList
                select new AutocompleteResult(
                    $"{destinyMetricDefinition.DisplayProperties.Name} ({destinyMetricDefinition.Traits.Last().Select(x => x.DisplayProperties.Name)})",
                    destinyMetricDefinition.Hash));

        return Task.FromResult(
            AutocompletionResult.FromSuccess(resultList.OrderBy(_ => Random.Shared.Next()).Take(25)));
    }
}

public class MementoWeaponAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var source = (from autocompleteOption in autocompleteInteraction.Data.Options
            where autocompleteOption.Name == "source"
            select Enum.Parse<MementoSource>(autocompleteOption.Value.ToString() ?? string.Empty)).FirstOrDefault();

        var memCache = await ProcessMementoData.ReadJsonAsync();

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

public class LootTableAutocomplete : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var resultList = new List<LootTableDefinition>();

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        resultList.AddRange(currentSearch != null
            ? LootTables.KnownTables.Where(autocompleteResult =>
                autocompleteResult.Name!.ToLower().Contains(currentSearch.ToLower()))
            : LootTables.KnownTables);

        var autocompleteList = resultList
            .Select(lootTable => new AutocompleteResult($"{lootTable.ActivityType}: {lootTable.Name}", lootTable.Name))
            .ToList();

        autocompleteList = autocompleteList.OrderBy(x => x.Name).ToList();

        return Task.FromResult(AutocompletionResult.FromSuccess(autocompleteList));
    }
}

public class WishAutocomplete : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var resultList = new List<Wish>();

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        resultList.AddRange(currentSearch != null
            ? Wishes.KnownWishes.Where(autocompleteResult =>
                autocompleteResult.Description!.ToLower().Contains(currentSearch.ToLower()))
            : Wishes.KnownWishes);

        var autocompleteList = resultList
            .Select(wish => new AutocompleteResult($"Wish {wish.Number}: {wish.Description}", wish.Number)).ToList();

        return Task.FromResult(AutocompletionResult.FromSuccess(autocompleteList));
    }
}

public class RollFinderAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var weaponList = await ProcessRollData.FromJsonAsync();

        if (weaponList == null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "Failed to parse weapon rolls.");

        var mode = Convert.ToInt32(autocompleteInteraction.Data.Options.First().Value);

        var rollList = mode switch
        {
            0 => weaponList.PvE,
            1 => weaponList.PvP,
            _ => null
        };

        if (rollList == null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "Failed to parse weapon rolls.");

        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        var resultList = new List<Roll>();

        if (string.IsNullOrEmpty(currentSearch))
            resultList.AddRange(rollList);
        else
            foreach (var roll in rollList.Where(roll =>
                         roll.WeaponName != null && roll.WeaponName.ToLower().Contains(currentSearch.ToLower())))
                if (!resultList.Select(r => r.WeaponName == roll.WeaponName).Any())
                    resultList.Add(roll);

        var autocompleteList =
            resultList.Select(roll => new AutocompleteResult(roll.WeaponName, roll.WeaponId)).ToList();

        return AutocompletionResult.FromSuccess(string.IsNullOrEmpty(currentSearch)
            ? autocompleteList.OrderBy(_ => Random.Shared.Next()).Take(25)
            : autocompleteList.OrderBy(x => x.Name).Take(25));
    }
}

public class CheckpointAutocomplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var currentSearch = autocompleteInteraction.Data.Current.Value.ToString();

        var checkpointList = await CheckpointParser.FetchAsync();
        if (checkpointList == null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful,
                "Failed to fetch checkpoint list.");
        // WHY AM I REQUIRED TO INCLUDE A REASON WHEN DISCORD
        // HAS BEEN IGNORING THIS PARAMETER EVER SINCE THE RELEASE OF AUTO-COMPLETES???

        var autocompleteList = new List<AutocompleteResult>();
        if (string.IsNullOrEmpty(currentSearch))
            autocompleteList.AddRange(checkpointList.Official?.Select(officialCp =>
                                          new AutocompleteResult($"{officialCp.Activity} - {officialCp.Encounter}",
                                              officialCp.DisplayOrder)) ??
                                      Enumerable.Empty<AutocompleteResult>());
        else
            autocompleteList.AddRange(from officialCp in checkpointList.Official
                where $"{officialCp.Activity} {officialCp.Encounter}".ToLower().Contains(currentSearch.ToLower())
                select new AutocompleteResult($"{officialCp.Activity} - {officialCp.Encounter}",
                    officialCp.DisplayOrder));

        return AutocompletionResult.FromSuccess(string.IsNullOrEmpty(currentSearch)
            ? autocompleteList
            : autocompleteList.OrderBy(x => x.Name));
    }
}
