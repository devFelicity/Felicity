using Discord;
using Discord.WebSocket;
using Serilog;

namespace FelicityOne.Services;

internal static class LogService
{
    public static SocketTextChannel? DiscordLogChannel { get; set; }

    public static Task SendLog(LogMessage log)
    {
        switch (log.Severity)
        {
            case LogSeverity.Critical:
                if (log.Exception != null)
                    Log.Fatal(log.Exception, log.Message);
                else
                    Log.Fatal(log.Message);
                break;
            case LogSeverity.Error:
                if (log.Exception != null)
                    Log.Error(log.Exception, log.Message);
                else
                    Log.Error(log.Message);
                break;
            case LogSeverity.Warning:
                if (log.Exception != null)
                    Log.Warning(log.Exception, log.Message);
                else
                    Log.Warning(log.Message);
                break;
            case LogSeverity.Info:
                if (log.Exception != null)
                    Log.Information(log.Exception, log.Message);
                else
                    Log.Information(log.Message);
                break;
            case LogSeverity.Verbose:
                if (log.Exception != null)
                    Log.Verbose(log.Exception, log.Message);
                else
                    Log.Verbose(log.Message);
                break;
            case LogSeverity.Debug:
                if (log.Exception != null)
                    Log.Debug(log.Exception, log.Message);
                else
                    Log.Debug(log.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(log));
        }
        
        return Task.CompletedTask;
    }

    public static void SendLogDiscord(string message)
    {
        DiscordLogChannel?.SendMessageAsync(message);
    }
}