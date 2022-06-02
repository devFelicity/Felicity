using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.Interactions;
using FelicityOne.Enums;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace FelicityOne.Commands.SlashCommands;

public class Mementos : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("memento", "Curious to see how a memento will look?")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter",
        Justification = "Parameter is used in autocomplete but not in command itself.")]
    public async Task Memento(
        [Summary("mementotype", "What type of memento are you looking for?")]
        MementoType mementoType,
        [Summary("source", "Where does the weapon you want to check out come from?")]
        // ReSharper disable once UnusedParameter.Global
        MementoSource mementoSource,
        [Autocomplete(typeof(MementoWeaponAutocomplete))] [Summary("weapon", "Name of the weapon you want to see.")]
        string weapon)
    {
        await DeferAsync();

        var mementoList = mementoType switch
        {
            MementoType.Gambit => KnownMementos.KnownMementoList.MementoList.Gambit,
            MementoType.Trials => KnownMementos.KnownMementoList.MementoList.Trials,
            MementoType.Nightfall => KnownMementos.KnownMementoList.MementoList.Nightfall,
            _ => Array.Empty<MementoCat>()
        };

        var goodMemento = mementoList.FirstOrDefault(memento => memento.Name == weapon);

        if (mementoList == Array.Empty<MementoCat>() || goodMemento == null)
        {
            await FollowupAsync("An error occured retreiving memento.");
            return;
        }

        var goodSource = goodMemento.Source switch
        {
            MementoSource.OpenWorld => "Open World",
            MementoSource.RaidVotD => "Vow of the Disciple",
            MementoSource.SeasonRisen => "Seasonal (Risen)",
            MementoSource.SeasonHaunted => "Seasonal (Haunted)",
            MementoSource.ThroneWorld => "Throne World",
            _ => "Unknown"
        };

        var embed = new EmbedBuilder
        {
            Title = "Memento Preview",
            Fields = new List<EmbedFieldBuilder>
            {
                new() {IsInline = true, Name = "Memento", Value = mementoType},
                new() {IsInline = true, Name = "Weapon", Value = goodMemento.Name},
                new() {IsInline = true, Name = "Source", Value = goodSource}
            },
            ImageUrl = goodMemento.Url,
            Footer = new EmbedFooterBuilder
            {
                IconUrl = Images.FelicityLogo,
                Text = $"{Strings.FelicityVersion} | Image by {goodMemento.Author}"
            },
            ThumbnailUrl = Strings.GetMementoImage(mementoType)
        };

        await FollowupAsync(embed: embed.Build());
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

        var mementoList = (from autocompleteOption in autocompleteInteraction.Data.Options
                where autocompleteOption.Name == "mementotype"
                select Enum.Parse<MementoType>(autocompleteOption.Value.ToString() ?? string.Empty))
            .FirstOrDefault() switch
            {
                MementoType.Gambit => KnownMementos.KnownMementoList.MementoList.Gambit,
                MementoType.Trials => KnownMementos.KnownMementoList.MementoList.Trials,
                MementoType.Nightfall => KnownMementos.KnownMementoList.MementoList.Nightfall,
                _ => Array.Empty<MementoCat>()
            };

        var result = (from knownMemento in mementoList
            where knownMemento.Source == source
            select new AutocompleteResult(knownMemento.Name, knownMemento.Name)).ToList();

        result = result.OrderBy(x => x.Name).ToList();

        return AutocompletionResult.FromSuccess(result);
    }
}