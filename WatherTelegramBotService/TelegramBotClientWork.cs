using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using WeatherTelegramBotService.OpenWeatherMap;

namespace WeatherTelegramBotService
{
    class TelegramBotClientWork : IDisposable
    {
        private MessageBot messageBot;
        private string loger = string.Empty;
        ChatId chat = new ChatId("@your chanel");
        ChatId currChat;
        private Message Message;
        private const int ONESECOND = 1000;
        private const int ONEMiNUTE = 60 * ONESECOND;
        private const int TWENTYMINETS = 18 * ONEMiNUTE;
        private const int EIGHTHOURS = (60 * ONEMiNUTE * 8) - ONEMiNUTE;
        public TelegramBotClientWork(ITelegramBotClient botClient, ChatId currChat)
        {
            this.currChat = currChat;
            this.Message = null;
            this.messageBot = new MessageBot(botClient);
        }

        public void Dispose()
        {
            this.chat = null;
            this.currChat = null;
            this.messageBot = null;
        }

        public void Start()
        {
            int timeSleep = 0;
            while (true)
            {
                timeSleep = GetMainTimer();
                Thread.Sleep(timeSleep);
            }
        }

        /// <summary>
        /// Прогноз погодны
        /// </summary>
        /// <returns>Секудны задержки</returns>
        private int GetMainTimer()
        {
            DateTime date = DateTime.UtcNow;
            date = date.AddHours(6);

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
                if (Message != null)
                {
                    WeatherCurrentEdit(date);
                }
                else
                {
                    WeatherCurrent(date);
                }
                return TWENTYMINETS;
            }
            else
            {
                return ONEMiNUTE;
            }
        }

        /// <summary>
        /// Прогноз на завтра
        /// </summary>
        /// <param name="date"></param>
        private void WeatherTomorow(DateTime date)
        {
            string currentWeatther = CurrentWeattherTomorow();
            messageBot.SendMessage(chat, currentWeatther);
            WeatherCurrent(date);
        }

        /// <summary>
        /// Прогноз на сегодня
        /// </summary>
        /// <param name="date"></param>
        private void WeatherToDay(DateTime date)
        {
            string currentWeatther = CurrentWeattherToday();
            messageBot.SendMessage(chat, currentWeatther);
            WeatherCurrent(date);
        }

        /// <summary>
        /// Текущий прогноз
        /// </summary>
        /// <param name="date"></param>
        private void WeatherCurrent(DateTime date)
        {
            string currentWeatther = CurrentWeatther();
            messageBot.MessageWithTime(currChat, loger, date);
            Message = messageBot.GetAfterSendMessage(chat, currentWeatther).Result;
            
        }

        /// <summary>
        /// Редактирует сообщение о текущей погоде
        /// </summary>
        /// <param name="date"></param>
        private void WeatherCurrentEdit(DateTime date)
        {
            string currentWeatther = CurrentWeatther();
            currentWeatther += Environment.NewLine;
            messageBot.EditMessage(Message, currentWeatther);
            messageBot.MessageWithTime(currChat, loger, date);
        }
        
        private async Task<HttpResponseMessage> Get(HttpClient client, string url)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {

                requestMessage.Headers.Add("x-rapidapi-host", "");
                requestMessage.Headers.Add("x-rapidapi-key", "");
                return await client.SendAsync(requestMessage);
            }
        }

        /// <summary>
        /// Погода на текущий момент 
        /// </summary>
        /// <returns></returns>
        private string CurrentWeatther()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/weather?lang=ru&units=metric&q=Bishkek";

                HttpResponseMessage response = Get(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                Weather responseAnswer = JsonConvert.DeserializeObject<Weather>(responseAsString);
                
                string currWeather = responseAnswer.GetWeatherOfCurrent();

                loger = responseAnswer.GetLoging();
                
                return currWeather;
            }
        }

        /// <summary>
        /// Погода на текущий день 
        /// </summary>
        /// <returns></returns>
        private string CurrentWeattherToday()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/forecast?q=Bishkek&units=metric&lang=ru";

                HttpResponseMessage response = Get(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                WeatherOfFiveDays responseAnswer = JsonConvert.DeserializeObject<WeatherOfFiveDays>(responseAsString);
                
                string currWeather = responseAnswer.GetWeatherOfToDay();
                
                return currWeather;
            }
        }

        /// <summary>
        /// Погода на завтра
        /// </summary>
        /// <returns></returns>
        private string CurrentWeattherTomorow()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/forecast?q=Bishkek&units=metric&lang=ru";

                HttpResponseMessage response = Get(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                WeatherOfFiveDays responseAnswer = JsonConvert.DeserializeObject<WeatherOfFiveDays>(responseAsString);
                
                string currWeather = responseAnswer.GetWeatherOfTomorow();
                
                return currWeather;
            }
        }

    }
}

