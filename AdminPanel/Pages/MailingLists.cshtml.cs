using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Entities;

namespace AdminPanel.Pages
{
    public class MailingListsModel : PageModel
    {
        public void OnGet()
        {
            //TelegramService telegramService = new TelegramService("5654249846:AAHU6xVbyc8vGMpODDM9h9kAfS4khjBUgig", );

            //// Получите список идентификаторов пользователей, которым нужно отправить рассылку
            //List<User> userIds = new List<User>();

            //// Отправьте рассылку
            //await telegramService.SendMailingAsync(userIds, "Привет! Это ваше сообщение рассылки.");
        }
    }
}
