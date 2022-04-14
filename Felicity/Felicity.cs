using System;
using System.IO;
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
#pragma warning disable CS0649  // Remove unused variable
    private static bool _debug;
#pragma warning restore CS0649
#pragma warning restore IDE0044
#pragma warning restore IDE0079

    private readonly DiscordSocketClient _client;

    private readonly CommandService _commands;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _services;
    
    public Felicity()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged
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

    public async Task StartAsync()
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
        await OAuthService.Start(_client);
        
        await Task.Delay(-1);
    }

    private async Task InitializeListeners()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.MessageReceived += HandleMessageAsync;
        _client.InteractionCreated += HandleInteraction;

        _client.Ready += async () =>
        {
            /*var guild = _client.GetGuild(764586645684355092);
            await guild.DeleteApplicationCommandsAsync();*/

            //await _client.Rest.DeleteAllGlobalCommandsAsync();

            await _interaction.RegisterCommandsToGuildAsync(960484926950637608);
            
            // await _interaction.RegisterCommandsGloballyAsync();
            
            if (!_debug) 
                TwitchService.Setup(_client);

            Log.Information($"Connected as {_client.CurrentUser.Username}#{_client.CurrentUser.DiscriminatorValue}");
        };

        _interaction.SlashCommandExecuted += SlashCommandExecuted;

        _client.SelectMenuExecuted += SelectMenuHandler;
    }

    private static Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context,
        IResult result)
    {
        if (result.IsSuccess)
            return Task.CompletedTask;

        if (result.Error != null)
            Log.Error($"{result.Error.GetType()}: {result.ErrorReason}");

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

            if (isStaff)
            {
                await TryHandleCommandAsync(msg, argPos).ConfigureAwait(false);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithDescription(
                        "All text-based commands, similar to this one, have been migrated to Slash Commands. Use /help for more info.")
                    .WithColor(ConfigHelper.GetEmbedColor());
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
        try
        {
            var context = new SocketInteractionContext(_client, arg);
            await _interaction.ExecuteCommandAsync(context, _services);
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.GetType()}: {ex.Message}");

            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
                await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }
}