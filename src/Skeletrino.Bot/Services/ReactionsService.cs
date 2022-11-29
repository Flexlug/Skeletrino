using DSharpPlus;
using Microsoft.Extensions.Logging;
using Skeletrino.Bot.Services.Interfaces;

namespace Skeletrino.Bot.Services;

public class ReactionsService : IReactionsService
{
    private DiscordClient _client;
    private ILogger<ReactionsService> _logger;

    public ReactionsService(ILogger<ReactionsService> logger, DiscordClient client)
    {
        _client = client;
        _logger = logger;

        _client.MessageCreated += Client_DetectSayHi;

        _logger.LogInformation($"{nameof(ReactionsService)} loaded");
    }
    
    private async Task Client_DetectSayHi(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
    {
        string msg = e.Message.Content.ToLower();

        if (msg.Contains("привет") && msg.Contains("скелетик"))
        {
            await e.Message.RespondAsync("https://cdn.discordapp.com/attachments/776568856167972904/836541954779119616/4a5b505b4026b6fe30376b0b79d3e108fa755e07r1-540-540_hq.gif");
            return;
        }

        if (msg.Contains("вставай припадочный"))
        {
            await e.Message.RespondAsync("https://cdn.discordapp.com/attachments/776568856167972904/838014941884579880/JeRWf8iDd_4.png");
            return;
        }

        if (msg.Contains("привет") && (msg.Contains("виталий") || msg.Contains("припадочный") || msg.Contains("виталя")))
        {
            await e.Message.RespondAsync(":skull:");
            return;
        }
    }
}