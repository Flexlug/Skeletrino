using System.Globalization;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Emzi0767.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Skeletrino.Bot.Commands;
using Skeletrino.Bot.Services;
using Skeletrino.Bot.Services.Interfaces;

namespace Skeletrino.Bot;

public class Bot : IDisposable
{
    public const ulong BOT_ID = 1047230967259082793;

    private Settings.Settings _settings { get; }
    
    private DiscordClient _discord;
    private CommandsNextExtension _commandsNext;

    private IServiceProvider _services;
    
    private ILoggerFactory _logFactory;
    private ILogger<Bot> _logger;

    private bool _isRunning;
    private bool _isDisposed;
    
    public Bot(Settings.Settings settings)
    {
        _settings = settings;

        _logFactory = new LoggerFactory().AddSerilog();
        _logger = _logFactory.CreateLogger<Bot>();

        _discord = new DiscordClient(new DiscordConfiguration
        {
            Token = _settings.Token,
            TokenType = TokenType.Bot,
            LoggerFactory = _logFactory,
            Intents = DiscordIntents.All
        });

        _discord.ClientErrored += (sender, args) =>
        {
            _logger.LogError(args.Exception, "Discord_ClientErrored");
            return Task.CompletedTask; ;
        };

        // For correct datetime recognizing
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        
        ConfigureServices();
        RegisterCommands();
    }

    private void ConfigureServices()
    {
        _services = new ServiceCollection()
            .AddLogging(conf => conf.AddSerilog(dispose: true))
            .AddSingleton(_settings)
            .AddSingleton(_discord)
            // .AddSingleton<IMessageResendService, MessageResendService>()
            // .AddSingleton<IMessageDeleteService, MessageDeleteService>()
            .AddSingleton<IReactionsService, ReactionsService>()
            .BuildServiceProvider();
    }

    private void RegisterCommands()
    {
        _logger.LogInformation("Registering commands");
        var commandsNextConfiguration = new CommandsNextConfiguration
        {
            StringPrefixes = _settings.Prefixes,
            Services = _services
        };
        _commandsNext = _discord.UseCommandsNext(commandsNextConfiguration);
        _commandsNext.SetHelpFormatter<CustomHelpFormatter>();
        _commandsNext.RegisterCommands<UserCommands>();

        _commandsNext.CommandErrored += Discord_OnCommandErrored;
    }

    private Task Discord_OnCommandErrored(object sender, CommandErrorEventArgs e)
    {
        if (e.Exception is ArgumentException)
        {
            e.Context.RespondAsync(
                $"Не удалось вызвать команду `sk!{e.Command.QualifiedName}` с заданными аргументами. Используйте `sk!help`, чтобы проверить правильность вызова команды.");
            return Task.CompletedTask;
        }

        if (e.Exception is DSharpPlus.CommandsNext.Exceptions.CommandNotFoundException)
        {
            e.Context.RespondAsync($"Не удалось найти данную команду.");
            return Task.CompletedTask;
        }

        DiscordEmbed embed = new DiscordEmbedBuilder()
            .WithTitle("Error")
            .WithDescription($"StackTrace: {e.Exception.StackTrace}")
            .AddField("Command", e.Command?.Name ?? "-")
            .AddField("Overload",
                e.Context.Overload.Arguments.Count == 0
                    ? "-"
                    : string.Join(' ', e.Context.Overload.Arguments.Select(x => x.Name)?.ToArray()))
            .AddField("Exception", e.Exception.GetType().ToString())
            .AddField("Exception msg", e.Exception.Message)
            .AddField("Inner exception", e.Exception.InnerException?.Message ?? "-")
            .AddField("Channel", e.Context.Channel.Name)
            .AddField("Author", e.Context.Member.Username)
            .Build();
        
        return Task.CompletedTask;
    }
    
    public async Task RunAsync()
    {
        if (_isRunning)
        {
            throw new MethodAccessException("The bot is already running");
        }

        await _discord.ConnectAsync();
        _isRunning = true;
        while (_isRunning)
        {
            await Task.Delay(200);
        }
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            _discord.Dispose();
        }
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}