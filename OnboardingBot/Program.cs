using Newtonsoft.Json;
using System.Net.Http.Headers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using OnboardingBot.Models;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using System.Text;
using System.Net.Http;

namespace OnboardingBot
{
    class Program
    {
        static HttpClient HttpClient = new HttpClient();
        static ITelegramBotClient BotClient = new TelegramBotClient("5654249846:AAHU6xVbyc8vGMpODDM9h9kAfS4khjBUgig");

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
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
        }

        public static async Task BotClient_OnCallbackQuery(ITelegramBotClient botClient, Update update, object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var buttonData = callbackQuery.Data;
            var message = update.Message;

            if (buttonData.StartsWith("direction_"))
            {
                
                HttpResponseMessage registrationResponse = await HttpClient.PostAsync("api/users", null);

                if (registrationResponse.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Регистрация прошла успешно! Вы выбрали направление - ");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось провести регистрацию! Попробуйте позже.");
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

            //string connectionString = "Host=localhost;Port=5432;Database=OnboardingBot;Username=onboardingbot;Password=onboardingbot";

            RunAsync().GetAwaiter().GetResult();

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            BotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
        static async Task RunAsync()
        {
            HttpClient.BaseAddress = new Uri("https://localhost:7290");
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}
