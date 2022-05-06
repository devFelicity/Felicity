using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Felicity.Configs;
using Felicity.Helpers;
using Felicity.Services;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Commands.SlashCommands;

[RequireBotModerator]
[Group("server", "Collection of server management commands for setting up your server.")]
public class ServerManagement : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("announcechannel", "Which channel should Felicity send announcements related to bot services?")]
    public async Task ServerAnnouncement(ITextChannel channel)
    {
        await DeferAsync(true);

        var serverSettings = GetServerSettings(Context.Guild.Id);
        serverSettings.Settings[Context.Guild.Id.ToString()].AnnouncementChannel = channel.Id;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set announcement channel to {channel.Mention}.");
    }

    [SlashCommand("modrole", "Which role should be allowed to change Felicity behavior?")]
    public async Task ModeratorRole(IRole role)
    {
        await DeferAsync(true);

        var serverSettings = GetServerSettings(Context.Guild.Id);
        serverSettings.Settings[Context.Guild.Id.ToString()].ModeratorRole = role.Id;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set moderator role to {role.Mention}.");
    }

    [SlashCommand("modchannel", "Which channel should Felicity send STAFF-ONLY announcements to?")]
    public async Task ModeratorChannel(ITextChannel channel)
    {
        await DeferAsync(true);

        var serverSettings = GetServerSettings(Context.Guild.Id);
        serverSettings.Settings[Context.Guild.Id.ToString()].StaffChannel = channel.Id;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set staff channel to {channel.Mention}.");
    }

    [SlashCommand("memberevents", "Where should Felicity send server member events to?")]
    public async Task MemberEvents(
        [Summary("memberchannel", "Which channel do I send to?")]
        ITextChannel channel,
        [Summary("memberjoined", "Should I send messages when a member joins?")]
        bool memberJoined,
        [Summary("memberleft", "Should I send messages when a member leaves?")]
        bool memberLeft,
        [Summary("memberkicked", "Should I send messages when a member is kicked?")]
        bool memberKicked,
        [Summary("memberbanned", "Should I send messages when a member is banned?")]
        bool memberBanned
    )
    {
        await DeferAsync(true);

        var serverSettings = GetServerSettings(Context.Guild.Id);
        serverSettings.Settings[Context.Guild.Id.ToString()].MemberEvents = new MemberEvents
        {
            LogChannel = channel.Id,
            MemberJoined = memberJoined,
            MemberLeft = memberLeft,
            MemberBanned = memberBanned,
            MemberKicked = memberKicked
        };

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync("Successfully set up member events.");
    }

    [SlashCommand("overview", "Check up on your servers settings.")]
    public async Task Overview()
    {
        await DeferAsync(true);

        var serverSettings = ConfigHelper.GetServerSettings(Context.Guild.Id);
        if (serverSettings == null)
        {
            await FollowupAsync("Server is not set up.");
            return;
        }

        var msg = "Staff role: ";

        var modRole = Context.Guild.GetRole(serverSettings.ModeratorRole);
        if (modRole != null)
            msg += $"{modRole.Mention}\n";
        else
            msg += "None.\n";

        msg +=
            $"Global announcements: {GetTextChannel(Context.Guild, serverSettings.AnnouncementChannel)}, Staff-only announcements: {GetTextChannel(Context.Guild, serverSettings.StaffChannel)}\n";
        msg +=
            $"Member events:\n- Channel: {GetTextChannel(Context.Guild, serverSettings.MemberEvents.LogChannel)}\n" +
            $"- Events: Join: **{serverSettings.MemberEvents.MemberJoined}**, Leave: **{serverSettings.MemberEvents.MemberLeft}**, " +
            $"Kicked: **{serverSettings.MemberEvents.MemberKicked}**, Banned: **{serverSettings.MemberEvents.MemberBanned}**";

        await FollowupAsync(msg, ephemeral: true);
    }

    [Group("twitch", "Manage Twitch stream notifications for this server.")]
    public class TwitchNotifications : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("add", "Add a Twitch stream to the server.")]
        public async Task TwitchNotification(
            [Summary("twitchname", "Stream name you'd like to subscribe to.")]
            string twitchName,
            [Summary("channel", "Channel you'd like me to post notifications to.")]
            ITextChannel channel,
            [Summary("everyone", "Should I ping everyone when they go live?")]
            bool mentionEveryone = false,
            [Summary("role", "Should I ping a role when they go live?")]
            IRole role = null,
            [Summary("discordname", "If the streamer is in your server, I can mention them.")]
            IGuildUser user = null)
        {
            await DeferAsync();

            twitchName = twitchName.ToLower();

            if (!TwitchService.UserExists(twitchName))
            {
                await FollowupAsync("Stream could not be found, check the spelling and try again.");
                return;
            }

            var serverSettings = GetServerSettings(Context.Guild.Id);
            serverSettings.Settings[Context.Guild.Id.ToString()].TwitchStreams ??= new Dictionary<string, TwitchStream>();

            var newStream = new TwitchStream
            {
                ChannelId = channel.Id,
                MentionEveryone = mentionEveryone
            };

            if (role != null) newStream.Mention = role.Id;
            if (user != null) newStream.UserId = user.Id;

            serverSettings.Settings[Context.Guild.Id.ToString()].TwitchStreams.Add(twitchName.ToLower(), newStream);

            await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

            TwitchService.RestartMonitor();

            await FollowupAsync($"Added {Format.Bold(twitchName)}'s stream to {channel.Mention}", ephemeral: true);
        }

        [SlashCommand("remove", "Remove an existing Twitch stream from the server.")]
        public async Task RemoveTwitchStream(
            [Autocomplete(typeof(TwitchStreamAutocomplete))]
            [Summary("twitchname", "Stream name you'd like to unsubscribe from.")]
            string twitchName)
        {
            await DeferAsync();

            var serverSettings = ServerConfig.FromJson();
            serverSettings.Settings[Context.Guild.Id.ToString()].TwitchStreams.Remove(twitchName);
            await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

            TwitchService.RestartMonitor();

            await FollowupAsync($"Successfully removed {Format.Bold(twitchName)}'s stream from server.", ephemeral: true);
        }
    }

    private static string GetTextChannel(SocketGuild contextGuild, ulong channelId)
    {
        var channel = contextGuild.GetTextChannel(channelId);
        return channel == null ? "None." : channel.Mention;
    }

    private static ServerConfig GetServerSettings(ulong guildId)
    {
        var serverSettings = ServerConfig.FromJson();
        if (serverSettings.Settings.ContainsKey(guildId.ToString()))
            return serverSettings;

        serverSettings.Settings.Add(guildId.ToString(), new ServerSetting());
        return serverSettings;
    }
}

public class TwitchStreamAutocomplete : AutocompleteHandler
{
#pragma warning disable CS1998
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
#pragma warning restore CS1998
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        var serverSettings = ConfigHelper.GetServerSettings(context.Guild.Id);

        if (serverSettings == null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "Couldn't find server config.");

        var suggestions = serverSettings.TwitchStreams
            .Select(stream => new AutocompleteResult {Name = stream.Key, Value = stream.Key}).ToList();

        return AutocompletionResult.FromSuccess(suggestions);
    }
}