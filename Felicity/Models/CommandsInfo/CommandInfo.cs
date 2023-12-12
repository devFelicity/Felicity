using System.Diagnostics;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Models.CommandsInfo;

[DebuggerDisplay("{CommandPath}, Parameters count = {ParametersInfo.Count}")]
public class CommandInfo
{
    public CommandInfo(string path, string desc)
    {
        CommandPath = path;
        CommandDescription = desc;
        ParametersInfo = new List<CommandParameterInfo>();
    }

    public string CommandPath { get; }
    public string CommandDescription { get; }
    public List<CommandParameterInfo> ParametersInfo { get; }
}
