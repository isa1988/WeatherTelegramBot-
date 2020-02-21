using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using WatherTelegramBotService.OpenWeatherMap;

namespace WatherTelegramBotService
{
    class TelegramBotClientWork : IDisposable
    {
        private ITelegramBotClient botClient;
        ChatId chat = new ChatId("@you chanel");
        ChatId currChat;
        private const int ONESECOND = 1000;
        private const int ONEMiNUTE = 60 * ONESECOND;
        private const int TWENTYMINETS = 18 * ONEMiNUTE;
        private const int EIGHTHOURS = (60 * ONEMiNUTE * 8) - ONEMiNUTE;
        public TelegramBotClientWork(ITelegramBotClient botClient, ChatId currChat)
        {
            this.botClient = botClient;
            this.currChat = currChat;
        }

        public void Dispose()
        {
            botClient = null;
            chat = null;
            currChat = null;
        }

        /// <summary>
        /// Прогноз погодны
        /// </summary>
        /// <returns>Секудны задержки</returns>
        public int GetMainTimer()
        {
            DateTime date = DateTime.UtcNow;
            date = date.AddHours(6);
            MessageWithTime("в цикле", date);

            if (date.Hour == 22 && date.Minute == 0)
            {
                WeatherTomorow(date);
                return EIGHTHOURS;
            }
            else if (date.Hour == 6 && date.Minute == 0)
            {
                WeatherToDay(date);
                return TWENTYMINETS;
            }
            else if (date.Minute == 0 || date.Minute == 20 || date.Minute == 40)
            {
                WeatherCurrent(date);
                return TWENTYMINETS;
            }
            else
            {
                MessageWithTime("в иначе", date);
                return ONEMiNUTE;
            }
        }

        private void AnswerToAdminIsWork()
        {
            //ответ админу что я запустился
            var me = botClient.GetMeAsync().Result;
            Log($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        }

        /// <summary>
        /// Прогноз на завтра
        /// </summary>
        /// <param name="date"></param>
        private async void WeatherTomorow(DateTime date)
        {
            string currentWeatther = await CurrentWeattherTomorow();
            SendMessage(chat, currentWeatther);
            currentWeatther = await CurrentWeatther();
            SendMessage(chat, currentWeatther);
            Log("в 22 " + date.ToString("dd.MM.yyyy HH:mm:ss"));
        }

        /// <summary>
        /// Прогноз на сегодня
        /// </summary>
        /// <param name="date"></param>
        private async void WeatherToDay(DateTime date)
        {
            string currentWeatther = await CurrentWeattherToday();
            SendMessage(chat, currentWeatther);
            currentWeatther = await CurrentWeatther();
            SendMessage(chat, currentWeatther);
            Log("в 8 час " + date.Hour.ToString() + " мин " + date.Minute.ToString());
        }

        /// <summary>
        /// Текущий прогноз
        /// </summary>
        /// <param name="date"></param>
        private async void WeatherCurrent(DateTime date)
        {
            string currentWeatther = await CurrentWeatther();
            SendMessage(chat, currentWeatther);
            Log("в 20 40" + date.Minute.ToString());
        }

        /// <summary>
        /// Логирование на дату
        /// </summary>
        /// <param name="message"></param>
        /// <param name="date"></param>
        private void MessageWithTime(string message, DateTime date)
        {
            Log(message + " " + date.ToString("dd.MM.yyyy HH:mm:ss"));
        }
        
        private async Task<HttpResponseMessage> Get(HttpClient client, string url)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            requestMessage.Headers.Add("x-rapidapi-host", "");
            requestMessage.Headers.Add("x-rapidapi-key", "");
            return await client.SendAsync(requestMessage);
        }

        /// <summary>
        /// Погода на текущий момент 
        /// </summary>
        /// <returns></returns>
        async Task<string> CurrentWeatther()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/weather?lang=ru&units=metric&q=Bishkek";

                HttpResponseMessage response = await Get(client, url);

                string responseAsString = await response.Content.ReadAsStringAsync();

                RootObject responseAnswer = JsonConvert.DeserializeObject<RootObject>(responseAsString);
                IWeather weather = responseAnswer;
                string currWeather = weather.GetCurrent();
                return currWeather;
            }
        }

        /// <summary>
        /// Погода на текущий день 
        /// </summary>
        /// <returns></returns>
        async Task<string> CurrentWeattherToday()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/forecast?q=Bishkek&units=metric&lang=ru";

                HttpResponseMessage response = await Get(client, url);

                string responseAsString = await response.Content.ReadAsStringAsync();

                RootObjectOfFiveDays responseAnswer = JsonConvert.DeserializeObject<RootObjectOfFiveDays>(responseAsString);
                IWeather weather = responseAnswer;
                string currWeather = weather.GetToDay();
                return currWeather;
            }
        }
        
        /// <summary>
        /// Погода на завтра
        /// </summary>
        /// <returns></returns>
        async Task<string> CurrentWeattherTomorow()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/forecast?q=Bishkek&units=metric&lang=ru";

                HttpResponseMessage response = await Get(client, url);

                string responseAsString = await response.Content.ReadAsStringAsync();

                RootObjectOfFiveDays responseAnswer = JsonConvert.DeserializeObject<RootObjectOfFiveDays>(responseAsString);
                IWeather weather = responseAnswer;
                string currWeather = weather.GetTomorow();
                return currWeather;
            }
        }

        /// <summary>
        /// Посылка сообщения на завтра
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        void SendMessage(ChatId chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
        }

        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isToTeleTelegram"></param>
        private void Log(string message, bool isToTeleTelegram = true)
        {
            if (isToTeleTelegram)
            {
                SendMessage(currChat, message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
