using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using FelicityOne.Caches;
using FelicityOne.Events;
using FelicityOne.Helpers;
using FelicityOne.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using IResult = Discord.Interactions.IResult;

namespace FelicityOne;

internal class Felicity
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _services;

    private Felicity()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = (GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents) |
                             GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true
        });
        _commands = new CommandService();
        _interaction = new InteractionService(_client);
        _services = new ServiceCollection()
            .AddSingleton(_client)
            // .AddSingleton<InteractiveService>()
            .AddSingleton<InteractionService>()
            .BuildServiceProvider();
    }

    private static void Main()
    {
        const string ASCIIName = @"    ______     ___      _ __" + "\n" +
                                 @"   / ____/__  / (_)____(_) /___  __" + "\n" +
                                 @"  / /_  / _ \/ / / ___/ / __/ / / /" + "\n" +
                                 @" / __/ /  __/ / / /__/ / /_/ /_/ /" + "\n" +
                                 @"/_/    \___/_/_/\___/_/\__/\__, /" + "\n" +
                                 @"                          /____/  @axsLeaf" + "\n";

        if (ConfigService.LoadConfigFiles())
            return;

        if (IsDebug())
        {
            Console.Title = "FelicityOne";
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(ASCIIName);
            Console.ForegroundColor = ConsoleColor.Gray;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Sentry(o =>
                {
                    o.AttachStacktrace = true;
                    o.Dsn = ConfigService.GetBotSettings().SentryDsn;
                    o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                    o.MinimumEventLevel = LogEventLevel.Warning;
                    o.Release = "FelicityOne@" + ConfigService.GetBotSettings().Version;
                })
                .CreateLogger();
        }

        new Felicity().StartAsync().GetAwaiter().GetResult();
    }

    private async Task StartAsync()
    {
        _services.GetRequiredService<DiscordSocketClient>().Log += LogService.SendLog;
        _services.GetRequiredService<InteractionService>().Log += LogService.SendLog;

        await InitializeListeners();

        await _client.LoginAsync(TokenType.Bot, ConfigService.GetBotSettings().DiscordToken);
        await _client.StartAsync();

        EmoteHelper.DiscordClient = _client;
        StatusService.DiscordClient = _client;

        await Jobs.StartJobs();

        await OAuthService.Start(_client);
    }

    private async Task InitializeListeners()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.MessageReceived += HandleMessage;
        _client.InteractionCreated += HandleInteraction;
        _interaction.SlashCommandExecuted += HandleSlashCommand;

        _client.MessageUpdated += DiscordEvents.HandleMessageUpdated;

        _client.UserJoined += DiscordEvents.HandleJoin;
        _client.UserLeft += DiscordEvents.HandleLeft;
        _client.UserVoiceStateUpdated += DiscordEvents.HandleVC;

        // _client.PresenceUpdated += DiscordEvents.HandlePresenceUpdated;

        _client.JoinedGuild += DiscordEvents.HandleJoinedGuild;
        _client.LeftGuild += DiscordEvents.HandleLeftGuild;
        _client.InviteCreated += DiscordEvents.HandleInviteCreated;

        _client.Ready += async () =>
        {
            Log.Information($"Connected as {_client.CurrentUser.Username}#{_client.CurrentUser.DiscriminatorValue}");
            LogService.DiscordLogChannel =
                (SocketTextChannel) _client.GetChannel(ConfigService.GetBotSettings().ManagementChannel);

            TwitchService.Setup(_client);

            await _client.Rest.DeleteAllGlobalCommandsAsync();

            if (IsDebug())
            {
                await _interaction.RegisterCommandsToGuildAsync(960484926950637608);
                await _interaction.RegisterCommandsToGuildAsync(764586645684355092);
            }
            else
            {
                var testGuild = _client.GetGuild(960484926950637608);
                await testGuild.DeleteApplicationCommandsAsync();

                await _interaction.RegisterCommandsGloballyAsync();

                TwitchService.ConfigureMonitor();
            }

            StatusService.ChangeGame();
        };
    }

    private async Task HandleMessage(SocketMessage arg)
    {
        if (arg.Channel.Id == ConfigService.GetBotSettings().CheckpointChannel && arg.Author.IsBot)
        {
            ProcessCPData.Populate(arg);
            return;
        }

        if (arg.Author.IsBot || arg.Author.IsWebhook) return;
        if (arg.Content.Length <= 0) return;
        if (arg.Author.Username is "Felicity" or "Felicity-Beta") return;

        if (arg is not SocketUserMessage msg) return;

        var argPos = 0;

        if (msg.HasStringPrefix(ConfigService.GetBotSettings().CommandPrefix, ref argPos))
        {
            if (!arg.Author.IsStaff())
                return;

            await TryHandleCommandAsync(msg, argPos).ConfigureAwait(false);
        }
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        var context = new SocketInteractionContext(_client, arg);

        var banned = ConfigService.GetBotSettings().BannedUsers
            .Any(bannedUser => bannedUser.Id == arg.User.Id);

        if (banned)
        {
            LogService.SendLogDiscord($"Banned user `{context.User}` tried to run a command.");
            await arg.DeferAsync();
            return;
        }

        try
        {
            await _interaction.ExecuteCommandAsync(context, _services);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to execute command.");

            if (arg.Type == InteractionType.ApplicationCommand)
                await arg.GetOriginalResponseAsync().ContinueWith(async msgtask => await msgtask.Result.DeleteAsync());
        }
    }

    private static Task HandleSlashCommand(SlashCommandInfo info, IInteractionContext context,
        IResult result)
    {
        if (result.IsSuccess || !result.Error.HasValue)
            return Task.CompletedTask;

        using (LogContext.PushProperty("context",
                   new
                   {
                       Command = context.Interaction.Data, Invoker = context.User, Server = context.Guild,
                       Result = result
                   }))
        {
            var msg = $"Failed to execute command: {result.Error.GetType()}: {result.ErrorReason}";
            Log.Error(msg);

            if (result.Error == InteractionCommandError.UnmetPrecondition)
                return Task.CompletedTask;

            context.Interaction.FollowupAsync("Command failed to execute, logs have been forwarded to staff.");
            context.Interaction.RespondAsync("Command failed to execute, logs have been forwarded to staff.");
        }

        return Task.CompletedTask;
    }

    private async Task TryHandleCommandAsync(SocketUserMessage msg, int argPos)
    {
        var context = new SocketCommandContext(_client, msg);

        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (result.Error.HasValue)
            using (LogContext.PushProperty("context",
                       new {context.Message.Content, Invoker = context.User, Server = context.Guild}))
            {
                Log.Error($"[{result.Error.Value}]: {result.ErrorReason}");
            }
    }

    public static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}