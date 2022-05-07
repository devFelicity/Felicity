using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using APIHelper;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Felicity.Helpers;
using Felicity.Services;
using Fergun.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using IResult = Discord.Interactions.IResult;

namespace Felicity;

internal class Felicity
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Remove unused variable
    private static bool _debug;
#pragma warning restore CS0649
#pragma warning restore IDE0044
#pragma warning restore IDE0079

    private readonly DiscordSocketClient _client;

    private readonly CommandService _commands;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _services;

    private Felicity()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers |
                             GatewayIntents.GuildPresences
        });
        _commands = new CommandService();
        _interaction = new InteractionService(_client);
        _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton<InteractiveService>()
            .AddSingleton<InteractionService>()
            .BuildServiceProvider();
    }

    private static void Main()
    {
#if DEBUG
        _debug = true;
#endif

        const string ASCIIName = @"    ______     ___      _ __" + "\n" +
                                 @"   / ____/__  / (_)____(_) /___  __" + "\n" +
                                 @"  / /_  / _ \/ / / ___/ / __/ / / /" + "\n" +
                                 @" / __/ /  __/ / / /__/ / /_/ /_/ /" + "\n" +
                                 @"/_/    \___/_/_/\___/_/\__/\__, /" + "\n" +
                                 @"                          /____/  @axsLeaf" + "\n";

        if (_debug)
        {
            Console.Title = "FelicityOne";
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(ASCIIName);
            Console.ForegroundColor = ConsoleColor.Gray;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }
        else
        {
            Console.WriteLine(ASCIIName);

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        new Felicity().StartAsync().GetAwaiter().GetResult();
    }

    private async Task StartAsync()
    {
        if (ConfigHelper.LoadConfigFiles())
            return;

        Log.Information($"Config loaded: {ConfigHelper.GetBotSettings().Note}");

        var client = _services.GetRequiredService<DiscordSocketClient>();
        client.Log += LogHelper.Log;

        var commands = _services.GetRequiredService<InteractionService>();
        commands.Log += LogHelper.Log;

        await InitializeListeners();

        await _client.LoginAsync(TokenType.Bot, ConfigHelper.GetBotSettings().DiscordToken);
        await _client.StartAsync();

        // TODO: make use of bungiesharper for this
        API.FetchManifest();
        await Jobs.StartJobs();
        EmoteHelper._client = _client;
        StatusService._client = _client;
        await OAuthService.Start(_client);

        await Task.Delay(-1);
    }

    private async Task InitializeListeners()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.MessageReceived += HandleMessageAsync;
        _client.InteractionCreated += HandleInteraction;

        _client.UserJoined += HandleJoin;
        _client.UserLeft += HandleLeft;
        _client.UserBanned += HandleBan;
        _client.JoinedGuild += HandleJoinedGuild;
        _client.LeftGuild += HandleLeftGuild;

        _client.UserVoiceStateUpdated += HandleVC;

        _client.Ready += async () =>
        {
            TwitchService.Setup(_client);

            //await _client.Rest.DeleteAllGlobalCommandsAsync();
            if (_debug)
            {
                await _interaction.RegisterCommandsToGuildAsync(960484926950637608);
                await _interaction.RegisterCommandsToGuildAsync(764586645684355092);
            }
            else
            {
                var guild = _client.GetGuild(960484926950637608);
                await guild.DeleteApplicationCommandsAsync();

                await _interaction.RegisterCommandsGloballyAsync();
                TwitchService.ConfigureMonitor();
            }

            Log.Information($"Connected as {_client.CurrentUser.Username}#{_client.CurrentUser.DiscriminatorValue}");
            LogHelper.DiscordLogChannel =
                (SocketTextChannel) _client.GetChannel(ConfigHelper.GetBotSettings().ManagementChannel);

            StatusService.ChangeGame();
        };

        _interaction.SlashCommandExecuted += SlashCommandExecuted;

        _client.SelectMenuExecuted += SelectMenuHandler;
    }

    private static Task HandleJoinedGuild(SocketGuild arg)
    {
        if (ConfigHelper.GetBotSettings().BannedUsers.Contains(arg.OwnerId))
        {
            LogHelper.LogToDiscord($"Bot was added to server owned by a banned user: `{arg.Owner}`");
            arg.LeaveAsync();
        }

        LogHelper.LogToDiscord($"Bot was added to `{arg.Name}` owned by `{arg.Owner}`.");
        return Task.CompletedTask;
    }

    private static Task HandleLeftGuild(SocketGuild arg)
    {
        LogHelper.LogToDiscord($"Bot was removed from `{arg.Name}` owned by `{arg.Owner}`.");
        return Task.CompletedTask;
    }


    private static async Task HandleJoin(SocketGuildUser arg)
    {
        var serverSettings = ConfigHelper.GetServerSettings(arg.Guild.Id);
        if (serverSettings != null)
            if (serverSettings.MemberEvents.MemberJoined)
                await arg.Guild.GetTextChannel(serverSettings.MemberEvents.LogChannel).SendMessageAsync(
                    embed: Extensions.GenerateMessageEmbed(arg.Username, arg.GetAvatarUrl(),
                        $"{arg} has joined the server!").Build());
    }

    private static async Task HandleLeft(SocketGuild arg1, SocketUser arg2)
    {
        var serverSettings = ConfigHelper.GetServerSettings(arg1.Id);
        if (serverSettings != null)
            if (serverSettings.MemberEvents.MemberLeft)
                await arg1.GetTextChannel(serverSettings.MemberEvents.LogChannel).SendMessageAsync(
                    embed: Extensions.GenerateMessageEmbed(arg2.Username, arg2.GetAvatarUrl(),
                        $"{arg2} has left the server!").Build());
    }

    private static async Task HandleBan(SocketUser arg1, SocketGuild arg2)
    {
        var serverSettings = ConfigHelper.GetServerSettings(arg2.Id);
        if (serverSettings != null)
            if (serverSettings.MemberEvents.MemberBanned)
                await arg2.GetTextChannel(serverSettings.MemberEvents.LogChannel).SendMessageAsync(
                    embed: Extensions.GenerateMessageEmbed(arg1.Username, arg1.GetAvatarUrl(),
                        $"{arg1} has been banned from the server!").Build());
    }

    private static Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context,
        IResult result)
    {
        if (result.IsSuccess)
            return Task.CompletedTask;

        if (result.Error == null)
            return Task.CompletedTask;

        LogHelper.LogToDiscord($"Error in `{context.Guild.Name}`:\n{result.Error.GetType()}: {result.ErrorReason}", LogSeverity.Error);
        
        return Task.CompletedTask;
    }

    private static async Task SelectMenuHandler(SocketMessageComponent interaction)
    {
        await interaction.RespondAsync("Not yet implemented");
    }

    private async Task HandleMessageAsync(SocketMessage arg)
    {
        if (arg.Author.IsWebhook || arg.Author.IsBot) return;
        if (arg.Content.Length <= 0) return;
        if (arg.Author.Id == _client.CurrentUser.Id) return;
        if (arg is not SocketUserMessage msg) return;

        var argPos = 0;

        if (msg.HasStringPrefix(ConfigHelper.GetBotSettings().CommandPrefix, ref argPos))
        {
            var isStaff = arg.Author.IsStaff();

            if (arg.Channel.GetType() == typeof(SocketDMChannel) && !isStaff)
            {
                await arg.Channel.SendMessageAsync("I do not accept commands through Direct Messages.");
                return;
            }

            {
                await TryHandleCommandAsync(msg, argPos).ConfigureAwait(false);
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    Description =
                        "All text-based commands, similar to this one, have been migrated to Slash Commands. Use /help for more info.",
                    Color = ConfigHelper.GetEmbedColor()
                };
                await msg.ReplyAsync(embed: embed.Build());
            }
        }
    }

    private async Task TryHandleCommandAsync(SocketUserMessage msg, int argPos)
    {
        var context = new SocketCommandContext(_client, msg);

        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (result.Error.HasValue)
            await context.Channel.SendMessageAsync($"[{result.Error.Value}]: {result.ErrorReason}")
                .ConfigureAwait(false);
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        var context = new SocketInteractionContext(_client, arg);

        if (ConfigHelper.GetBotSettings().BannedUsers.Contains(context.User.Id))
        {
            LogHelper.LogToDiscord($"Banned user `{context.User}` tried to run a command.");
            return;
        }

        try
        {
            await _interaction.ExecuteCommandAsync(context, _services);
        }
        catch (Exception ex)
        {
            /*LogHelper.LogToDiscord($"Error in `{context.Guild.Name}`:\n{msg}", LogSeverity.Error);
            await context.Interaction.FollowupAsync(msg);*/

            var msg = $"{ex.GetType()}: {ex.Message}";
            Log.Error(msg);

            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
                await arg.GetOriginalResponseAsync().ContinueWith(async msgtask => await msgtask.Result.DeleteAsync());
        }
    }
}