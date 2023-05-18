using Newtonsoft.Json;
using OnboardingBot.Models;
using Server;
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
                await HandleMessage(botClient, update.Message, cancellationToken);
                return;
            }
            //Подписываемся на получение ответов
            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
                return;
            }
        }

        public static async Task HandleMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
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
                //Читаем содержимое ответа в строку и десериализуем содержимое ответа в список объектов направлений.
                string responseContent = await response.Content.ReadAsStringAsync();
                List<DirectionModel> directions = JsonConvert.DeserializeObject<List<DirectionModel>>(responseContent);

                //Создаём список кнопок
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

            #region Функция Мои предподаватели
            if (message.Text.ToLower() == "/my_teachers")
            {
                //Для функции необходимо выбрать направление, поэтому проверяем зарегестрирован ли пользователь
                HttpResponseMessage registrationResponse = await HttpClient.GetAsync("api/Users/" + message.From.Id);

                //Читаем содержимое ответа в строку и десериализуем содержимое ответа в объект пользователя.
                string userResponseContent = await registrationResponse.Content.ReadAsStringAsync();
                Models.User user = JsonConvert.DeserializeObject<Models.User>(userResponseContent);

                if (registrationResponse.IsSuccessStatusCode)//Проверка на зарегетрированного пользователя
                {
                    HttpResponseMessage employeeDirectionResponse = await HttpClient.GetAsync("api/EmployeeDirections");

                    //Читаем содержимое ответа в строку и десериализуем содержимое ответа в Список объектов.
                    string responseContent = await employeeDirectionResponse.Content.ReadAsStringAsync();
                    List<EmployeeDirection> employeeDirections = JsonConvert.DeserializeObject<List<EmployeeDirection>>(responseContent);

                    foreach (EmployeeDirection employeeDirection in employeeDirections)
                    {
                        if (employeeDirection.DirectionId == user.DirectionId)
                        {
                            HttpResponseMessage employeeResponse = await HttpClient.GetAsync($"api/Employees/{employeeDirection.EmployeeId}");
                            //Читаем содержимое ответа в строку и десериализуем содержимое ответа в Список объектов.
                            string responseEmployeeContent = await employeeResponse.Content.ReadAsStringAsync();
                            Employee employee = JsonConvert.DeserializeObject<Employee>(responseEmployeeContent);

                            string employeeMessage = $"{employee.FullName}\nДолжность: {employee.Position}\n\n";
                            // Проверка наличия описания
                            if (employee.Description != null)
                            {
                                // Добавление информации об описании
                                employeeMessage += $"{employee.Description}\n\n";
                            }

                            // Проверка наличия контактной информации
                            if (employee.Email != null || employee.Telephone != null || employee.VkLink != null || employee.TelegramLink != null)
                            {
                                employeeMessage += "Контакты:\n";

                                // Проверка наличия электронной почты
                                if (employee.Email != null)
                                {
                                    // Добавление информации об электронной почте
                                    employeeMessage += $"Email: {employee.Email}\n";
                                }

                                // Проверка наличия телефона
                                if (employee.Telephone != null)
                                {
                                    // Добавление информации о телефоне
                                    employeeMessage += $"Телефон: {employee.Telephone}\n";
                                }

                                // Проверка наличия ссылки на ВКонтакте
                                if (employee.VkLink != null)
                                {
                                    // Добавление информации о ссылке на ВКонтакте
                                    employeeMessage += $"VK: {employee.VkLink}\n";
                                }

                                // Проверка наличия ссылки на Telegram
                                if (employee.TelegramLink != null)
                                {
                                    // Добавление информации о ссылке на Telegram
                                    employeeMessage += $"Telegram: {employee.TelegramLink}\n";
                                }
                            }

                            // Проверка наличия ссылки на фото
                            if (employee.PhotoLink != null)
                            {
                                // Отправка сообщения с фото и информацией о сотруднике
                                await botClient.SendPhotoAsync(message.Chat.Id, photo: InputFile.FromUri(employee.PhotoLink), caption: employeeMessage);
                            }
                            else
                            {
                                // Отправка текстового сообщения с информацией о сотруднике
                                await botClient.SendTextMessageAsync(message.Chat.Id, employeeMessage);
                            }


                        }
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Для начала необходимо пройти регистрацию!\n Используйте /start, необходимо выбрать направление");
                }
            }
            #endregion

            #region Функция Полезные ссылки
            if (message.Text.ToLower() == "/useful_links")
            {
                string UlMessage = "Полезные ссылки:\n\n";

                HttpResponseMessage response = await HttpClient.GetAsync("api/UsefulLinks");
                if (!response.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось отобразить полезные ссылки!");
                    return;
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                //Десирилизация json в список
                List<UsefulLink> usefulLinks = JsonConvert.DeserializeObject<List<UsefulLink>>(responseContent);

                foreach (UsefulLink usefulLink in usefulLinks)
                {
                    UlMessage += $"{usefulLink.Link} - {usefulLink.Description}";
                }

                await botClient.SendTextMessageAsync(message.Chat.Id, UlMessage);
                return;
            }
            #endregion

            #region Функция Поиск кабинета
            if (message.Text.StartsWith("/search_cabinet"))
            {
                // Извлечение номера кабинета из callback-запроса
                string cabinetNumber = message.Text.Substring("/search_cabinet".Length).Trim();
                if (cabinetNumber == "")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Необходимо через пробел ввести номер кабинета!");
                    return;
                }
                // Отправка асинхронного GET-запроса для поиска кабинета по указанному номеру
                HttpResponseMessage response = await HttpClient.GetAsync($"api/Cabinets/{cabinetNumber}");

                if (response.IsSuccessStatusCode)
                {
                    // Чтение содержимого ответа в виде строки и Десериализация содержимого ответа в объект Cabinet
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Cabinet cabinet = JsonConvert.DeserializeObject<Cabinet>(responseContent);

                    // Отправка GET-запроса для получения информации о планировке этажа, связанной с кабинетом
                    HttpResponseMessage responseMessage = await HttpClient.GetAsync($"api/FloorLayouts/{cabinet.FloorLayoutId}");

                    // Чтение содержимого ответа о планировке этажа в виде строки и Десериализация содержимого ответа о планировке этажа в объект FloorLayout
                    string responseContentFloorLayout = await responseMessage.Content.ReadAsStringAsync();
                    FloorLayout floorLayout = JsonConvert.DeserializeObject<FloorLayout>(responseContentFloorLayout);
                    string cabinetMessage = $"Найден кабинет:\n № кабинета {cabinet.Number}\n";

                    // Проверка наличия названия кабинета
                    if (cabinet.Name != null)
                    {
                        cabinetMessage += $"Кабинет называется: {cabinet.Name}\n\n";
                    }

                    // Проверка наличия названия планировки этажа
                    if (floorLayout.Name != null)
                    {
                        cabinetMessage += $"Кабинет расположен на {floorLayout.Name}\n\n";
                    }

                    // Проверка наличия описания планировки этажа
                    if (floorLayout.Description != null)
                    {
                        cabinetMessage += $"У этажа есть описание:\n {floorLayout.Description}\n";
                    }

                    // Проверка наличия ссылки на фото планировки этажа
                    if (floorLayout.PhotoLink != null)
                    {
                        // Отправка сообщения с фото планировки и сообщением о кабинете в качестве подписи
                        await botClient.SendPhotoAsync(message.Chat.Id, photo: InputFile.FromUri(floorLayout.PhotoLink), caption: cabinetMessage);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, cabinetMessage);
                    }
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось найти кабинет!");
                    return;
                }
            }
            #endregion

            #region Функция Задать вопрос
            if (message.Text.StartsWith("/question"))
            {
                // Извлечение вопроса из callback-запроса
                string question = message.Text.Substring("/question".Length).Trim();
                if(question == "")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Необходимо ввести вопрос");
                    return;
                }
                int lastIndex;
                // Использование контекста базы данных OnboardingBotContext
                using (var context = new OnboardingBotContext())
                {
                    // Получение последнего индекса вопроса из базы данных
                    lastIndex = context.UserQuestions
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.Id)
                        .FirstOrDefault();
                }
                int newIndex = lastIndex+1;
                // Создание объекта userQuestion с данными о вопросе
                var userQuestion = new
                {
                    Id = newIndex,
                    UserID = message.From.Id,
                    Question = question,
                    //DateTimeQuestion = DateTime.Now,
                };

                // Сериализация userQuestion в JSON-строку
                var jsonPayload = JsonConvert.SerializeObject(userQuestion);

                // Создание HTTP-контента с JSON-строкой
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Отправка POST-запроса для добавления вопроса в базу данных
                HttpResponseMessage questionResponse = await HttpClient.PostAsync("api/UserQuestions", content, cancellationToken);

                // Проверка успешности ответа на POST-запрос
                if (questionResponse.IsSuccessStatusCode)
                {
                    // Отправка сообщения о успешной отправке вопроса
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ваш вопрос был отправлен!");
                }
                else
                {
                    // Отправка сообщения об ошибке, если не удалось отправить вопрос
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! Не удалось отправить вопрос!");
                }
            }
            #endregion
        }

        public static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var buttonData = callbackQuery.Data;

            #region Регистрация пользователя
            if (buttonData.StartsWith("direction_"))
            {
                string directionId = buttonData.Substring(10);

                // Создание объекта с данными пользователя
                var userData = new
                {
                    id = callbackQuery.From.Id,
                    TelegramID = callbackQuery.From.Id,
                    DirectionID = directionId
                };

                var jsonPayload = JsonConvert.SerializeObject(userData);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                // Отправка POST-запроса для регистрации пользователя
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
            Console.ReadKey();
        }

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
