using System;
using System.Threading.Tasks;
using Discord;

namespace Felicity.Helpers;

internal class LogHelper
{
    public static Task Log(LogMessage log)
    {
        var ex = "";

        if (log.Exception != null) ex += $"{log.Exception} - ";

        ex += log.Message;

        switch (log.Severity)
        {
            case LogSeverity.Critical:
                Serilog.Log.Fatal(ex);
                break;
            case LogSeverity.Error:
                Serilog.Log.Error(ex);
                break;
            case LogSeverity.Warning:
                Serilog.Log.Warning(ex);
                break;
            case LogSeverity.Info:
                Serilog.Log.Information(ex);
                break;
            case LogSeverity.Verbose:
                Serilog.Log.Verbose(ex);
                break;
            case LogSeverity.Debug:
                Serilog.Log.Debug(ex);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return Task.CompletedTask;
    }
}