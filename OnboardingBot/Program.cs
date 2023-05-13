using Newtonsoft.Json;
using OnboardingBot.Models;
using Server.Entities;
using System.Net.Http.Headers;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Update = Telegram.Bot.Types.Update;

namespace OnboardingBot
{
    class Program
    {
        static HttpClient HttpClient = new HttpClient();
        static ITelegramBotClient BotClient = new TelegramBotClient("5654249846:AAHU6xVbyc8vGMpODDM9h9kAfS4khjBUgig");

        public static async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
                return;
            }
            HttpResponseMessage registrationResponse = await HttpClient.GetAsync("api/Users/" + update.Message.From.Id);

            if (registrationResponse.IsSuccessStatusCode)//Проверка на зарегетрированного пользователя
            {
                switch (update.Message.Text)
                {
                    case "/my_teachers":
                        await HandleOptionEmployees(botClient, update);
                        break;
                    case "/search_cabinet":
                        await HandleOptionSearchCabinets(botClient, update);
                        break;
                    case "/question":
                        await HandleOptionQuestion(botClient, update);
                        break;
                    case "/useful_links":
                        await HandleOptionUsefulLinks(botClient, update);
                        break;
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ошибка! Для начала необходимо пройти регистрацию!");
            }
        }
        public static async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привет! Я Чат-бот путеводитель твоих первых шагов в нашем техникуме, моя задача помогать новым студентам адаптироваться к новым условиям!");
                //Получение JSON данных из api
                HttpResponseMessage response = await HttpClient.GetAsync("api/Directions");
                if (response == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось найти направления!");
                    return;
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                //Десирилизация json в список
                List<DirectionModel> directions = JsonConvert.DeserializeObject<List<DirectionModel>>(responseContent);

                List<InlineKeyboardButton> directionButtons = new List<InlineKeyboardButton>();

                //Перебираем каждое направление из созданного списка
                foreach (DirectionModel direction in directions)
                {
                    //Добавляем кнопку для каждого направления
                    InlineKeyboardButton button = InlineKeyboardButton.WithCallbackData(direction.NameDirection, "direction_" + direction.Id.ToString());
                    directionButtons.Add(button);
                }
                InlineKeyboardMarkup inlineKeyboardDirection = new InlineKeyboardMarkup(directionButtons);

                await botClient.SendTextMessageAsync(message.Chat.Id, "Чтобы продолжить, мне необходимо задать тебе вопрос:\nНа каком направлении ты обучаешься?", replyMarkup: inlineKeyboardDirection);

                return;
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось распознать команду!");
        }

        public static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var buttonData = callbackQuery.Data;
            //Регистрация пользователя
            if (buttonData.StartsWith("direction_"))
            {
                string directionId = buttonData.Substring(10);

                var userData = new
                {
                    id = callbackQuery.From.Id,
                    TelegramID = callbackQuery.From.Id,
                    DirectionID = directionId
                };

                var jsonPayload = JsonConvert.SerializeObject(userData);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage registrationResponse = await HttpClient.PostAsync("api/Users", content, cancellationToken);

                if (registrationResponse.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Регистрация прошла успешно! Вы выбрали направление - " + directionId);
                }
                else
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Ошибка! Не удалось провести регистрацию!");
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }            

        static void Main(string[] args)
        {
            Console.WriteLine("Bot launched: " + BotClient.GetMeAsync().Result.FirstName);

            RunAsync().GetAwaiter().GetResult();

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, 
            };
            BotClient.StartReceiving(HandleUpdatesAsync, HandleErrorAsync, receiverOptions, cancellationToken);
        }


        private static async Task HandleOptionEmployees(ITelegramBotClient botClient, Update update)

        {
            var message = update.Message.Chat.Id;
            await botClient.SendTextMessageAsync(message, "Сотрудники");
            return;
        }

        private static async Task HandleOptionSearchCabinets(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message.Chat.Id;
            await botClient.SendTextMessageAsync(message, "Поиск кабинета");
            return;
        }

        private static async Task HandleOptionQuestion(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message.Chat.Id;
            await botClient.SendTextMessageAsync(message, "Задать вопрос");
            return;
        }

        private static async Task HandleOptionUsefulLinks(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message.Chat.Id;
            string UlMessage = "Полезные ссылки:\n\n";

            HttpResponseMessage response = await HttpClient.GetAsync("api/UsefulLinks");
            if (response == null)
            {
                await botClient.SendTextMessageAsync(message, "Ошибка! Не удалось отобразить полезные ссылки!");
                return;
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            //Десирилизация json в список
            List<UsefulLink> usefulLinks = JsonConvert.DeserializeObject<List<UsefulLink>>(responseContent);

            foreach (UsefulLink usefulLink in usefulLinks)
            {
                UlMessage += $"{usefulLink.Link} - {usefulLink.Description}";
            }

            await botClient.SendTextMessageAsync(message, UlMessage);
            return;
        }

        static async Task RunAsync()
        {
            HttpClient.BaseAddress = new Uri("https://localhost:7290");
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}
