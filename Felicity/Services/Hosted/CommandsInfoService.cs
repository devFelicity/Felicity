using System.Collections.ObjectModel;
using System.Reflection;
using Discord.Interactions;
using Felicity.Models.CommandsInfo;
using Felicity.Services.Hosted.Interfaces;
using Serilog;
using CommandParameterInfo = Felicity.Models.CommandsInfo.CommandParameterInfo;

namespace Felicity.Services.Hosted;

public class CommandsInfoService : ICommandsInfoService
{
    private readonly Type _baseCommandType = typeof(IInteractionModuleBase);
    public ReadOnlyCollection<CommandInfo> CommandsInfo { get; private set; }

    public CommandsInfoService()
    {
    }

    public void Initialize()
    {
        var commandsInfo = new List<CommandInfo>();
        var assembly = Assembly.GetAssembly(typeof(CommandsInfoService));

        if (assembly == null)
        {
            Log.Error("Assembly failed to populate.");
            return;
        }

        var assemblyTypes = assembly.GetTypes();
        var commandTypes = assemblyTypes.Where(x => x.IsAssignableTo(_baseCommandType)).ToArray();

        foreach (var commandType in commandTypes)
        {
            var groupAttribute = commandType.GetCustomAttribute<GroupAttribute>();

            if (groupAttribute is null) continue;

            var methods = commandType.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (var methodData in methods)
            {
                var slashCommandAttribute = methodData.GetCustomAttribute<SlashCommandAttribute>();
                if (slashCommandAttribute is null) continue;

                var commandInfo = new CommandInfo(
                    $"{groupAttribute.Name} {slashCommandAttribute.Name}",
                    slashCommandAttribute.Description);

                var parameters = methodData.GetParameters();
                foreach (var parameter in parameters)
                {
                    var commandParameterInfo = new CommandParameterInfo();
                    var summaryAttribute = parameter.GetCustomAttribute<SummaryAttribute>();
                    var autoCompleteAttribute = parameter.GetCustomAttribute<AutocompleteAttribute>();

                    if (summaryAttribute is not null)
                    {
                        commandParameterInfo.Name = summaryAttribute.Name;
                        commandParameterInfo.Description = summaryAttribute.Description;
                    }
                    else
                    {
                        commandParameterInfo.Name = parameter.Name!;
                    }

                    if (autoCompleteAttribute is not null) commandParameterInfo.IsAutocomplete = true;

                    commandInfo.ParametersInfo.Add(commandParameterInfo);
                }

                commandsInfo.Add(commandInfo);
            }
        }

        CommandsInfo = new ReadOnlyCollection<CommandInfo>(commandsInfo);
    }
}