using Newtonsoft.Json;
using OnboardingBot.Models;
using Server;
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
            //Логируем каждое сообщение
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            //Подписываемся на получение сообщений 
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }
            //Подписываемся на получение ответов
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
                    case "/useful_links":
                        await HandleOptionUsefulLinks(botClient, update);
                        break;
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ошибка! Для начала необходимо пройти регистрацию!\n Используйте /start, необходимо выбрать направление");
            }
        }
        public static async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            #region /start, Отправка сообщения для регистрация
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
            #endregion

            //await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось распознать команду!");
        }

        public static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var buttonData = callbackQuery.Data;

            #region Регистрация пользователя
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
            #endregion



            #region Функция Поиск кабинета
            if (callbackQuery.Data.StartsWith("/search_cabinet"))
            {
                string cabinetNumber = callbackQuery.Data.Substring("/search_cabinet ".Length);
                HttpResponseMessage response = await HttpClient.GetAsync($"api/Cabinets/search/{cabinetNumber}");
                if (response == null)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Ошибка! Не удалось найти кабинет!");
                    return;
                }
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage responseMessage = await HttpClient.GetAsync($"api/Cabinets/search/{cabinetNumber}");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Cabinet cabinet = JsonConvert.DeserializeObject<Cabinet>(responseContent);
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Найден кабинет:\n № кабинета {cabinet.Number}\n Кабинет называется: {cabinet.Name}\n\n Кабинет расположен: {cabinet.FloorLayoutId}");
                    return;
                }
            }
            #endregion

            #region Функция Задать вопрос
            if (callbackQuery.Data.StartsWith("/question"))
            {
                string question = callbackQuery.Data.Substring("/question ".Length);
                int lastIndex;
                using (var context = new OnboardingBotContext())
                {
                    lastIndex = context.UserQuestions
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefault();
                }
                var userQuestion = new
                {
                    Id = lastIndex++,
                    Question = question,
                    DateTimeQuestion = DateTime.Now,
                };
                var jsonPayload = JsonConvert.SerializeObject(userQuestion);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage questionResponse = await HttpClient.PostAsync("api/UserQuestions", content, cancellationToken);

                if (questionResponse.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Ваш вопрос был отправлен!");
                }
                else
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Ошибка! Не удалось отправить вопрос!");
                }
            }
            #endregion
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Записываем ошибки в консоль
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

        #region Функция Полезные ссылки
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
        #endregion

        #region Настройка HTTP клиента
        static async Task RunAsync()
        {
            HttpClient.BaseAddress = new Uri("https://localhost:7290");
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

    }
}
