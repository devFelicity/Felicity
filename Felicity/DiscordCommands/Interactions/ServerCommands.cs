using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Felicity.Models;
using Felicity.Services;
using Felicity.Util;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

[RequireContext(ContextType.Guild)]
[DefaultMemberPermissions(GuildPermission.ManageGuild)]
[Group("server", "Collection of server management commands for setting up your server.")]
public class ServerCommands : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly ServerDb _serverDb;

    public ServerCommands(ServerDb serverDb)
    {
        _serverDb = serverDb;
    }

    [SlashCommand("configure", "Set up your server.")]
    public async Task ServerConfigure(
        [Summary("announcements", "Which channel should Felicity send announcements related to bot services?")]
        ITextChannel announcementChannel,
        [Summary("staff", "Which channel should Felicity send STAFF-ONLY announcements to?")]
        ITextChannel staffChannel,
        [Summary("memberchannel", "Which channel do I send to?")]
        ITextChannel? memberLogChannel = null,
        [Summary("memberjoined", "Should I send messages when a member joins?")]
        bool memberJoined = false,
        [Summary("memberleft", "Should I send messages when a member leaves?")]
        bool memberLeft = false)
    {
        await DeferAsync(true);

        var server = MiscUtils.GetServer(_serverDb, Context.Guild.Id);
        server.AnnouncementChannel = announcementChannel.Id;
        server.StaffChannel = staffChannel.Id;

        if (memberLogChannel != null)
            server.MemberLogChannel = memberLogChannel.Id;

        server.MemberJoined = memberJoined;
        server.MemberLeft = memberLeft;

        await _serverDb.SaveChangesAsync();

        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = Context.Guild.Name,
            IconUrl = Context.Guild.IconUrl
        };
        embed.Description = "Summary of server settings:";

        var channelSummary = $"Announcements: {announcementChannel.Mention}\nStaff: {staffChannel.Mention}";
        if (memberLogChannel != null)
            channelSummary += $"\nJoin/Leave: {memberLogChannel.Mention}";

        embed.AddField("Channels", channelSummary, true);
        embed.AddField("Language", server.BungieLocale, true);
        embed.AddField("Member Events", $"Joins: {memberJoined}\nLeaves: {memberLeft}", true);

        await FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("summary", "Get a summary of current server settings.")]
    public async Task ServerSummary()
    {
        await DeferAsync(true);

        var server = MiscUtils.GetServer(_serverDb, Context.Guild.Id);

        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = Context.Guild.Name,
            IconUrl = Context.Guild.IconUrl
        };
        embed.Description = "Summary of server settings:";

        var channelSummary = $"Announcements: {GetChannel(Context.Guild, server.AnnouncementChannel)}\n" +
                             $"Staff: {GetChannel(Context.Guild, server.StaffChannel)}";
        if (server.MemberLogChannel != null)
            channelSummary += $"\nJoin/Leave: {GetChannel(Context.Guild, server.MemberLogChannel)}";

        embed.AddField("Channels", channelSummary, true);
        embed.AddField("Language", server.BungieLocale, true);
        embed.AddField("Member Events", $"Joins: {server.MemberJoined}\nLeaves: {server.MemberLeft}", true);

        await FollowupAsync(embed: embed.Build());
    }

    private static string GetChannel(SocketGuild guild, ulong? channelId)
    {
        return channelId == null ? "not set." : guild.GetTextChannel((ulong)channelId).Mention;
    }

    [RequireContext(ContextType.Guild)]
    [Preconditions.RequireBotModerator]
    [Group("twitch", "Manage Twitch stream notifications for this server.")]
    public class TwitchNotifications : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly TwitchStreamDb _streamDb;
        private readonly TwitchService _twitchClientService;

        public TwitchNotifications(TwitchStreamDb streamDb, TwitchService twitchClientService)
        {
            _streamDb = streamDb;
            _twitchClientService = twitchClientService;
        }

        [SlashCommand("add", "Add a Twitch stream to the server.")]
        public async Task ServerTwitchAdd(
            [Summary("twitchname", "Stream name you'd like to subscribe to.")]
            string twitchName,
            [Summary("channel", "Channel you'd like me to post notifications to.")]
            ITextChannel channel,
            [Summary("everyone", "Should I ping everyone when they go live?")]
            bool mentionEveryone = false,
            [Summary("role", "Should I ping a role when they go live?")]
            IRole? role = null,
            [Summary("discordname", "If the streamer is in your server, I can mention them.")]
            IGuildUser? user = null)
        {
            await DeferAsync();

            var stream = new TwitchStream
            {
                TwitchName = twitchName.ToLower(),
                ServerId = Context.Guild.Id,
                ChannelId = channel.Id,
                MentionEveryone = mentionEveryone
            };

            if (role != null) stream.MentionRole = role.Id;
            if (user != null) stream.UserId = user.Id;

            _streamDb.TwitchStreams.Add(stream);
            await _streamDb.SaveChangesAsync();

            _twitchClientService.RestartMonitor();

            await FollowupAsync($"Added {Format.Bold(twitchName)}'s stream to {channel.Mention}", ephemeral: true);
        }

        [SlashCommand("remove", "Remove an existing Twitch stream from the server.")]
        public async Task ServerTwitchRemove(
            [Autocomplete(typeof(TwitchStreamAutocomplete))]
            [Summary("twitchname", "Stream name you'd like to unsubscribe from.")]
            int streamId)
        {
            await DeferAsync();

            var stream = _streamDb.TwitchStreams.FirstOrDefault(x => x.Id == streamId);
            if (stream == null)
            {
                await FollowupAsync("Failed to find stream.");
                return;
            }

            _streamDb.TwitchStreams.Remove(stream);
            await _streamDb.SaveChangesAsync();

            _twitchClientService.RestartMonitor();

            await FollowupAsync($"Successfully removed {Format.Bold(stream.TwitchName)}'s stream from server.",
                ephemeral: true);
        }
    }
}