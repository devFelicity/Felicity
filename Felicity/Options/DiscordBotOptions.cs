using Discord;

namespace Felicity.Options;

public class DiscordBotOptions
{
    public string? Token { get; set; }
    public string? Prefix { get; set; }
    public ulong LogChannelId { get; set; }
    public ulong LogServerId { get; set; }
    public ulong[] BotStaff { get; set; }
    public ulong[] BannedUsers { get; set; }
    public Func<LogMessage, Exception?, string> LogFormat { get; set; } = (message, _) => $"{message.Source}: {message.Message}";
}