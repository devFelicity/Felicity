using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Felicity.Configs;

namespace Felicity.Helpers;

public class RequireOAuth : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo commandInfo, IServiceProvider services)
    {
        var oauth = (context.User as SocketUser).OAuth();

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

        var serverSettings = ConfigHelper.GetServerSettings(context.Guild.Id);

        if (serverSettings == null)
        {
            var allServerSettings = ServerConfig.FromJson();
            allServerSettings.Settings.Add(context.Guild.Id.ToString(), new ServerSetting());
            File.WriteAllText(ConfigHelper.ServerConfigPath, ServerConfig.ToJson(allServerSettings));

            serverSettings = ConfigHelper.GetServerSettings(context.Guild.Id);
        }

        var guildUser = context.Guild.GetUserAsync(context.User.Id).Result;

        if (context.Guild.OwnerId == guildUser.Id)
            return Task.FromResult(PreconditionResult.FromSuccess());

        if (guildUser.RoleIds.Contains(serverSettings.ModeratorRole))
            return Task.FromResult(PreconditionResult.FromSuccess());

        const string msg =
            "You are not a bot moderator for this server, your server owner should not have this command enabled for roles other than the designated moderator role.";
        context.Interaction.RespondAsync(msg);

        return Task.FromResult(PreconditionResult.FromError(msg));
    }
}