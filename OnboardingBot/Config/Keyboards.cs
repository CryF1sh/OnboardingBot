using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace OnboardingBot.Config
{
    public static class Keyboards
    {
        public static InlineKeyboardMarkup Menu = new(new[]
        {
            
                InlineKeyboardButton.WithCallbackData(text: "Предподаватели", callbackData: "employees"),
                InlineKeyboardButton.WithCallbackData(text: "Поиск кабинета", callbackData: "search_cabinets"),
                InlineKeyboardButton.WithCallbackData(text: "Задать вопрос", callbackData: "question"),
                InlineKeyboardButton.WithCallbackData(text: "Полезные ссылки", callbackData: "useful_links"),
            
        });
        //public static InlineKeyboardMarkup Direction = new(new[]
        //{
        //    new []
        //    {
                
        //    },
        //});
    }
}
