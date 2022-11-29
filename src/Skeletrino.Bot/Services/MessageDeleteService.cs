using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using Skeletrino.Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skeletrino.Bot.Services
{
    public class MessageDeleteService : IMessageDeleteService
    {
        private readonly DiscordEmoji _redCrossEmoji;
        private readonly ILogger<MessageDeleteService> _logger;

        public MessageDeleteService(DiscordClient client, ILogger<MessageDeleteService> logger)
        {
            _redCrossEmoji = DiscordEmoji.FromUnicode("❎");
            _logger = logger;

            client.MessageReactionAdded += DeleteResentMessage;
            _logger.LogInformation($"{nameof(MessageDeleteService)} loaded");
        }

        private async Task DeleteResentMessage(DiscordClient sender, MessageReactionAddEventArgs reactionInfo)
        {
            if (reactionInfo.User.Id == Bot.BOT_ID)
                return;

            if (reactionInfo.Emoji != _redCrossEmoji)
                return;

            var currentChannel = reactionInfo.Channel;
            var currentMessageId = reactionInfo.Message.Id;
            var currentMessage = await currentChannel.GetMessageAsync(currentMessageId);
            if (!currentMessage.Reactions.Any(x => x.Emoji == _redCrossEmoji && x.IsMe))
                return;

            var respondedMessage = currentMessage.ReferencedMessage;
            if (respondedMessage is null)
                return;

            if (respondedMessage.Author.Id != reactionInfo.User.Id)
                return;

            var allMessagesAfterCurrent = await currentChannel.GetMessagesAfterAsync(currentMessageId, 5);

            var deletingMessages = new List<DiscordMessage>();
            deletingMessages.Add(reactionInfo.Message);

            foreach (var message in allMessagesAfterCurrent)
            {
                if (message.Author.Id != Bot.BOT_ID)
                {   
                    break;
                }

                deletingMessages.Add(message);
            }

            await currentChannel.DeleteMessagesAsync(deletingMessages);
        }
    }
}
