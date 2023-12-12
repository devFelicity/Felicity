using System.Diagnostics;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace Felicity.Models.CommandsInfo;

[DebuggerDisplay("{Name}")]
public class CommandParameterInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsOptional { get; set; }
    public object DefaultValue { get; set; }
    public bool IsAutocomplete { get; set; }
    public bool IsSelect { get; set; }
}
