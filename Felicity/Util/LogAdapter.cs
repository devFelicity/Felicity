using Discord;
using Felicity.Options;
using Microsoft.Extensions.Options;

namespace Felicity.Util;

public class LogAdapter<T> where T : class
{
    private readonly Func<LogMessage, Exception?, string> _formatter;
    private readonly ILogger<T> _logger;

    public LogAdapter(ILogger<T> logger, IOptions<DiscordBotOptions> options)
    {
        _logger = logger;
        _formatter = options.Value.LogFormat;
    }

    public Task Log(LogMessage message)
    {
        _logger.Log(GetLogLevel(message.Severity), default, message, message.Exception, _formatter);
        return Task.CompletedTask;
    }

    private static LogLevel GetLogLevel(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }
}