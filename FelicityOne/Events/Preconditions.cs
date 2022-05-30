using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FelicityOne.Configs;
using FelicityOne.Helpers;
using FelicityOne.Services;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace FelicityOne.Events;

public class RequireOAuth : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo commandInfo, IServiceProvider services)
    {
        var oauth = ((SocketUser) context.User).OAuth();

        if (oauth != null) return Task.FromResult(PreconditionResult.FromSuccess());

        const string msg =
            "This command requires you to be registered to provide user information to the API.\nPlease /register and try again.";
        context.Interaction.RespondAsync(msg);

        return Task.FromResult(PreconditionResult.FromError(msg));
    }
}

public class RequireBotModerator : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo commandInfo, IServiceProvider services)
    {
        if (((IGuildUser) context.User).GuildPermissions.ManageGuild)
            return Task.FromResult(PreconditionResult.FromSuccess());

        var serverSettings = ConfigService.GetServerSettings(context.Guild.Id);

        if (serverSettings == null)
        {
            var allServerSettings =
                ConfigHelper.FromJson<ServerConfig>(File.ReadAllText(ConfigService.ServerConfigPath));
            allServerSettings?.Settings.Add(context.Guild.Id.ToString(), new ServerSetting());
            if (allServerSettings != null)
                File.WriteAllText(ConfigService.ServerConfigPath, ConfigHelper.ToJson(allServerSettings));

            serverSettings = ConfigService.GetServerSettings(context.Guild.Id);
        }

        var guildUser = context.Guild.GetUserAsync(context.User.Id).Result;

        if (context.Guild.OwnerId == guildUser.Id)
            return Task.FromResult(PreconditionResult.FromSuccess());

        if (serverSettings != null && guildUser.RoleIds.Contains(serverSettings.ModeratorRole))
            return Task.FromResult(PreconditionResult.FromSuccess());

        const string msg =
            "You are not a bot moderator for this server, your server owner should not have this command enabled for roles other than the designated moderator role.";
        context.Interaction.RespondAsync(msg);

        return Task.FromResult(PreconditionResult.FromError(msg));
    }
}