using Telegram.Bot;

namespace AdminPanel
{
    public class TelegramService
    {
        private readonly TelegramBotClient _telegramClient;
        private readonly long _chatId; // Идентификатор чата для рассылки

        public TelegramService(string botToken, long chatId)
        {
            _telegramClient = new TelegramBotClient(botToken);
            _chatId = chatId;
        }

        public async Task SendMailingAsync(List<long> userIds, string message)
        {
            foreach (long userId in userIds)
            {
                await _telegramClient.SendTextMessageAsync(userId, message);
            }
        }
    }
}
