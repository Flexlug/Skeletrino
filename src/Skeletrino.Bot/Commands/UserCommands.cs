using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using Skeletrino.Bot.Services;
using Skeletrino.Bot.Services.Interfaces;

namespace Skeletrino.Bot.Commands;

public class UserCommands : SkBaseCommandModule
{
    private ILogger<UserCommands> _logger;
    private IReactionsService _reactionsService;
    
    public UserCommands(ILogger<UserCommands> logger, 
        IReactionsService reactionsService, 
        DiscordClient client)
    {
        ModuleName = "Разное";
        
        _logger = logger;
        _reactionsService = reactionsService;
        
        _logger.LogInformation($"{nameof(UserCommands)} loaded");
    }
    
    /// <summary>
    /// Prints out the latency between the bot and discord api servers.
    /// </summary>
    /// <param name="commandContext">CommandContext from the message that has executed this command.</param>
    /// <returns></returns>
    [Command("ping"), Description("Показывает пинг бота."), Hidden]
    public async Task PingAsync(CommandContext commandContext)
    {
        await commandContext.RespondAsync($"Bot latency to the discord api server: {commandContext.Client.Ping}");
    }
}