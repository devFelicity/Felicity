using System;
using System.Threading.Tasks;
using System.Transactions;
using Discord;
using Discord.WebSocket;

namespace Felicity.Helpers;

internal class LogHelper
{
    public static SocketTextChannel DiscordLogChannel { get; set; }

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
                throw new ArgumentOutOfRangeException(nameof(log));
        }

        return Task.CompletedTask;
    }

    public static void LogToDiscord(string message, LogSeverity severity = LogSeverity.Info)
    {
        DiscordLogChannel.SendMessageAsync(message);

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (severity)
        {
            case LogSeverity.Error:
                Serilog.Log.Error(message);
                break;
            case LogSeverity.Warning:
                Serilog.Log.Warning(message);
                break;
            case LogSeverity.Info:
                Serilog.Log.Information(message);
                break;
        }
    }
}