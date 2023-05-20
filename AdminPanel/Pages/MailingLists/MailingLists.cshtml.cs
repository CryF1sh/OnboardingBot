using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;
using System.Net.Http;
using System.Text;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel.Pages.MailingLists
{
    public class MailingListsModel : PageModel
    {
        private readonly TelegramBotClient _botClient;
        private readonly HttpClient _httpClient;

        public MailingListsModel(TelegramBotClient botClient, IHttpClientFactory httpClientFactory)
        {
            _botClient = botClient;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AdminPanel/1.0");
        }

        public async Task<bool> SendBroadcastMessageAsync(string message)
        {
            List<string> userIds = GetSubscribedUserIds();

            foreach (string userId in userIds)
            {
                var requestBody = new
                {
                    chat_id = userId,
                    text = message
                };

                var requestBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"https://api.telegram.org/bot {5654249846:AAHU6xVbyc8vGMpODDM9h9kAfS4khjBUgig}/sendMessage", content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }

            return true;
        }

        private List<string> GetSubscribedUserIds()
        {
            using (var context = new OnboardingBotContext())
            {
                List<string> userIds = context.Users.Select(u => u.Id).ToList().ConvertAll(Id => Id.ToString());
                return userIds;
            }
        }

        [BindProperty]
        public string Message { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                List<User> users = new List<User>();
                bool result = await SendBroadcastMessageAsync(Message);
                if (result)
                {
                    // Рассылка успешно отправлена
                    return RedirectToPage("Success");
                }
                else
                {
                    // Произошла ошибка при отправке рассылки
                    ModelState.AddModelError(string.Empty, "Ошибка при отправке рассылки");
                }
            }

            // Валидация не прошла, остаемся на текущей странице
            return RedirectToPage("Error");
        }
    }
}
