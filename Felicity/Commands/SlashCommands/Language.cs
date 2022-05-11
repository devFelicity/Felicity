using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Felicity.Configs;
using Felicity.Helpers;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.Commands.SlashCommands;

public class Language : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("language", "Set the language of your server.")]
    public async Task SetLanguage([Autocomplete(typeof(LanguageAutocomplete))] int language)
    {
        await DeferAsync(true);

        var serverSettings = ServerConfig.GetServerSettings(Context.Guild.Id);
        serverSettings.Settings[Context.Guild.Id.ToString()].Language = (Lang) language;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set server language to {language}.");
    }

    private class LanguageAutocomplete : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter, IServiceProvider services)
        {
            await Task.Delay(0);

            var resultList = new List<AutocompleteResult>
            {
                new("English", Lang.En),
                new("Deutsch / German", Lang.De),
                new("Español / Spanish", Lang.Es),
                new("Français / French", Lang.Fr),
                new("Italiano / Italian", Lang.It),
                new("日本語 / Japanese", Lang.Ja),
                new("한국어 / Korean", Lang.Ko),
                new("Nederlandse / Dutch", Lang.Nl),
                new("Polskie / Polish", Lang.Pl),
                new("Português / Portuguese", Lang.PtBr),
                new("Русский / Russian", Lang.Ru),
                new("简体中文 / Chinese (Simplified)", Lang.ZhChs),
                new("繁體中文 / Chinese (Traditional)", Lang.ZhCht)
            };

            return AutocompletionResult.FromSuccess(resultList);
        }
    }
}
