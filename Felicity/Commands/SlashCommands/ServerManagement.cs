using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Felicity.Configs;
using Felicity.Helpers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.Commands.SlashCommands;

[Group("server", "Collection of server management commands for setting up your server.")]
public class ServerManagement : InteractionModuleBase<SocketInteractionContext>
{
    [RequireBotModerator]
    [SlashCommand("announcechannel", "Which channel should Felicity send announcements related to bot services?")]
    public async Task ServerAnnouncement(ITextChannel channel)
    {
        await DeferAsync(true);

        var serverSettings = ServerConfig.FromJson();
        serverSettings.Settings[Context.Guild.Id.ToString()].AnnouncementChannel = channel.Id;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set announcement channel to {channel.Mention}.");
    }

    [RequireBotModerator]
    [SlashCommand("modrole", "Which role should be allowed to change Felicity behavior?")]
    public async Task ModeratorRole(IRole role)
    {
        await DeferAsync(true);

        var serverSettings = ServerConfig.FromJson();
        serverSettings.Settings[Context.Guild.Id.ToString()].ModeratorRole = role.Id;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set moderator role to {role.Mention}.");
    }

    [RequireBotModerator]
    [SlashCommand("modchannel", "Which channel should Felicity send STAFF-ONLY announcements to?")]
    public async Task ModeratorChannel(ITextChannel channel)
    {
        await DeferAsync(true);

        var serverSettings = ServerConfig.FromJson();
        serverSettings.Settings[Context.Guild.Id.ToString()].StaffChannel = channel.Id;

        await File.WriteAllTextAsync(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(serverSettings));

        await FollowupAsync($"Successfully set staff channel to {channel.Mention}.");
    }

    [RequireBotModerator]
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

        var serverSettings = ServerConfig.FromJson();
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

    [RequireBotModerator]
    [SlashCommand("overview", "Check up on your servers settings.")]
    public async Task Overview()
    {
        await DeferAsync(true);

        var serverSettings = ConfigHelper.GetServerSettings(Context.Guild.Id);

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

    private static string GetTextChannel(SocketGuild contextGuild, ulong channelId)
    {
        var channel = contextGuild.GetTextChannel(channelId);
        return channel == null ? "None." : channel.Mention;
    }
}