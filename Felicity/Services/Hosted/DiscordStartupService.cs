using System.Reflection;
using System.Text.Json;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetBungieAPI.Models.Exceptions;
using Felicity.Models;
using Felicity.Options;
using Felicity.Util;
using Fergun.Interactive;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;
using ExecuteResult = Discord.Interactions.ExecuteResult;
using IResult = Discord.Interactions.IResult;

namespace Felicity.Services.Hosted;

public class DiscordStartupService : BackgroundService
{
    private readonly LogAdapter<BaseSocketClient> _adapter;
    private readonly CommandService _commandService;
    private readonly IOptions<DiscordBotOptions> _discordBotOptions;
    private readonly DiscordShardedClient _discordShardedClient;
    private readonly InteractionService _interactionService;
    private readonly InteractiveService _interactiveService;
    private readonly ILogger<DiscordStartupService> _logger;
    private readonly MetricDb _metricDb;
    private readonly IServiceProvider _serviceProvider;

    private int _shardsReady;
    private TaskCompletionSource<bool>? _taskCompletionSource;

    public DiscordStartupService(
        DiscordShardedClient discordShardedClient,
        IOptions<DiscordBotOptions> discordBotOptions,
        InteractionService interactionService,
        CommandService commandService,
        IServiceProvider serviceProvider,
        LogAdapter<BaseSocketClient> adapter,
        MetricDb metricDb,
        InteractiveService interactiveService,
        ILogger<DiscordStartupService> logger)
    {
        _discordShardedClient = discordShardedClient;
        _discordBotOptions = discordBotOptions;
        _interactionService = interactionService;
        _commandService = commandService;
        _serviceProvider = serviceProvider;
        _adapter = adapter;
        _metricDb = metricDb;
        _interactiveService = interactiveService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _discordShardedClient.Log += async logMessage => { await _adapter.Log(logMessage); };
            _discordShardedClient.ShardDisconnected += OnShardDisconnected;

            _discordShardedClient.MessageReceived += OnMessageReceived;

            _discordShardedClient.JoinedGuild += OnJoinedGuild;
            _discordShardedClient.LeftGuild += OnLeftGuild;

            _discordShardedClient.InteractionCreated += OnInteractionCreated;
            _interactionService.SlashCommandExecuted += OnSlashCommandExecuted;

            PrepareClientAwaiter();
            await _discordShardedClient.LoginAsync(TokenType.Bot, _discordBotOptions.Value.Token);
            await _discordShardedClient.StartAsync();

            await WaitForReadyAsync(stoppingToken);

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            if (BotVariables.IsDebug)
            {
                await _discordShardedClient.Rest.DeleteAllGlobalCommandsAsync();

                await _interactionService.RegisterCommandsToGuildAsync(_discordBotOptions.Value.LogServerId);
            }
            else
            {
                await _interactionService.RegisterCommandsGloballyAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in DiscordStartupService");
        }
    }

    private static async Task OnJoinedGuild(SocketGuild arg)
    {
        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = "Felicity was added to a server."
        };
        embed.Color = Color.Green;
        embed.Title = arg.Name;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Owner",
                Value = $"ID: {arg.OwnerId}",
                IsInline = true
            },
            new()
            {
                Name = "Members",
                // Value = arg.MemberCount,
                Value = "disabled.",
                IsInline = true
            }
        };

        if (arg.IconUrl != null)
            embed.ThumbnailUrl = arg.IconUrl;

        if (arg.Description != null)
            embed.Description = arg.Description;

        await BotVariables.DiscordLogChannel!.SendMessageAsync(embed: embed.Build());
    }

    private static async Task OnLeftGuild(SocketGuild arg)
    {
        var buggedServers = new List<ulong>
        {
            260978723455631373,
            1068135541360578590,
            719161295232040961
        };

        if (buggedServers.Contains(arg.Id))
            return;

        var embed = Embeds.MakeBuilder();
        embed.Author = new EmbedAuthorBuilder
        {
            Name = $"Felicity was removed from a server ({arg.Id})."
        };
        embed.Color = Color.Red;

        if (arg.Name != null)
            embed.Title = arg.Name;

        await BotVariables.DiscordLogChannel!.SendMessageAsync(embed: embed.Build());
    }

    private static async Task OnSlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult result)
    {
        if (result.IsSuccess || !result.Error.HasValue)
            return;

        if (result.Error == InteractionCommandError.UnmetPrecondition)
            return;

        var errorEmbed = Embeds.MakeErrorEmbed();
        errorEmbed.Title = "Failed to execute command.";

        if (result is ExecuteResult
            {
                Exception: BungieNetAuthorizationErrorException
                {
                    Error.ErrorDescription: "AuthorizationRecordExpired"
                }
            })
        {
            errorEmbed.Description =
                "Your membership info has expired, this can happen due to a Bungie change that happened 2023-08-15.\n\n" +
                "To fix it, please:\n" +
                "> - manually sign-in to [bungie.net](https://www.bungie.net/7/en/Destiny)\n" +
                "> - fill in the date of birth and location info\n" +
                "> - run `/user register`\n" +
                "> - re-run the command that caused this error\n\n" +
                "Note: if you already have given your location and date of birth, just run `/user register`\n\n" +
                "Apologies for any inconvenience.";

            await arg2.Interaction.FollowupAsync(embed: errorEmbed.Build());

            return;
        }

        if (result.ErrorReason.StartsWith("Refresh token for membership id"))
        {
            errorEmbed.Description =
                "Your membership info has expired, this can happen when you haven't run any commands for a while.\n" +
                "To fix it, please run `/user register` and re-run the command.";

            await arg2.Interaction.FollowupAsync(embed: errorEmbed.Build());

            return;
        }

        errorEmbed.Description = BotVariables.ErrorMessage;

        var debugOptions = new List<string>();
        var options = ((SocketSlashCommand)arg2.Interaction).Data;
        if (options != null && options.Options.Count > 0)
        {
            var opt = options.Options.First();
            debugOptions.Add($"SubCommand = {opt.Name}");
            debugOptions.AddRange(opt.Options.Select(socketSlashCommandDataOption =>
                $"{socketSlashCommandDataOption.Name} = {socketSlashCommandDataOption.Value}"));
        }

        var errorMessage = $"{result.Error}: {result.ErrorReason}";

        if(result is ExecuteResult eResult)
            errorMessage = $"{eResult.Exception.InnerException!.GetType()}: {eResult.Exception.InnerException!.Message}";
        
        errorEmbed.AddField("Command", $"```{options!.Name}```");
        errorEmbed.AddField("Parameters", $"```{JsonSerializer.Serialize(debugOptions)}```");
        errorEmbed.AddField("Error", $"```{errorMessage}```");

        using (LogContext.PushProperty("context", new
               {
                   Sender = arg2.User.ToString(),
                   CommandName = options.Name,
                   CommandParameters = JsonSerializer.Serialize(debugOptions),
                   ServerId = arg2.Interaction.GuildId ?? 0
               }))
        {
            Log.Error(errorMessage);
        }

        await arg2.Interaction.FollowupAsync(embed: errorEmbed.Build());
    }

    private async Task OnMessageReceived(SocketMessage socketMessage)
    {
        if (socketMessage is not SocketUserMessage socketUserMessage)
            return;

        var argPos = 0;
        if (!socketUserMessage.HasStringPrefix(_discordBotOptions.Value.Prefix, ref argPos))
            return;

        if (_discordBotOptions.Value.BotStaff != null &&
            !_discordBotOptions.Value.BotStaff.Contains(socketMessage.Author.Id))
            return;

        var context = new ShardedCommandContext(_discordShardedClient, socketUserMessage);
        var command = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

        if (command.Error is not null) _logger.LogError("{Error}: {ErrorReason}", command.Error, command.ErrorReason);
    }

    private async Task OnInteractionCreated(SocketInteraction socketInteraction)
    {
        if (socketInteraction is SocketMessageComponent messageComponent &&
            _interactiveService.Callbacks.ContainsKey(messageComponent.Message.Id))
            return;

        if (_discordBotOptions.Value.BannedUsers is not null &&
            _discordBotOptions.Value.BannedUsers.Contains(socketInteraction.User.Id))
        {
            await socketInteraction.DeferAsync();
            _logger.LogInformation("Banned user `{User}` tried to run a command", socketInteraction.User.ToString());
            return;
        }

        var shardedInteractionContext = new ShardedInteractionContext(_discordShardedClient, socketInteraction);

        await _interactionService.ExecuteCommandAsync(shardedInteractionContext, _serviceProvider);

        try
        {
            if (shardedInteractionContext.Interaction.Type ==
                InteractionType.ApplicationCommandAutocomplete)
                return;

            if (shardedInteractionContext.Interaction is not SocketSlashCommand command)
                return;

            var cmdName = command.CommandName;

            if (command.Data.Options.Count > 0)
                cmdName = command.Data.Options
                    .Where(cmdOption => cmdOption.Type == ApplicationCommandOptionType.SubCommand)
                    .Aggregate(cmdName, (current, cmdOption) => current + $" {cmdOption.Name}");

            _metricDb.Metrics.Add(new Metric
            {
                Author = shardedInteractionContext.User.Username + "#" +
                         shardedInteractionContext.User.Discriminator,
                Name = cmdName,
                TimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            });

            await _metricDb.SaveChangesAsync();
        }
        catch (Exception e)
        {
            if (e.InnerException is not null && !e.InnerException.Message.StartsWith("Duplicate entry"))
                _logger.LogError(e, "Failed to push metrics");
        }
    }

    private void PrepareClientAwaiter()
    {
        _taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _shardsReady = 0;

        _discordShardedClient.ShardReady += OnShardReady;
    }

    private Task OnShardReady(DiscordSocketClient discordClient)
    {
        Log.Information(
            $"Connected as {discordClient.CurrentUser.Username}#{discordClient.CurrentUser.DiscriminatorValue}");
        BotVariables.DiscordLogChannel ??=
            (SocketTextChannel)discordClient.GetChannel(_discordBotOptions.Value.LogChannelId);

        _shardsReady++;

        if (_shardsReady != _discordShardedClient.Shards.Count)
            return Task.CompletedTask;

        _taskCompletionSource!.TrySetResult(true);
        _discordShardedClient.ShardReady -= OnShardReady;

        return Task.CompletedTask;
    }

    private static async Task OnShardDisconnected(Exception arg1, DiscordSocketClient arg2)
    {
        Log.Error(arg1, "Disconnected from gateway.");

        if (arg1.InnerException is GatewayReconnectException &&
            arg1.InnerException.Message == "Server missed last heartbeat")
        {
            await arg2.StopAsync();
            await Task.Delay(10000);
            await arg2.StartAsync();
        }
    }

    private Task WaitForReadyAsync(CancellationToken cancellationToken)
    {
        if (_taskCompletionSource is null)
            throw new InvalidOperationException(
                "The sharded client has not been registered correctly. Did you use ConfigureDiscordShardedHost on your HostBuilder?");

        if (_taskCompletionSource.Task.IsCompleted)
            return _taskCompletionSource.Task;

        var registration = cancellationToken.Register(
            state => { ((TaskCompletionSource<bool>)state!).TrySetResult(true); },
            _taskCompletionSource);

        return _taskCompletionSource.Task.ContinueWith(_ => registration.DisposeAsync(), cancellationToken);
    }
}
