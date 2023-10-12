using System.Collections.ObjectModel;
using Felicity.Models.CommandsInfo;

namespace Felicity.Services.Hosted.Interfaces;

public interface ICommandsInfoService
{
    ReadOnlyCollection<CommandInfo> CommandsInfo { get; }
    void Initialize();
}