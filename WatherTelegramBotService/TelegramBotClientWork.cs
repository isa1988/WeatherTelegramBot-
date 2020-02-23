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
using WatherTelegramBotService.OpenWeatherMap;

namespace WatherTelegramBotService
{
    class TelegramBotClientWork : IDisposable
    {
        private object myLock;
        private ITelegramBotClient botClient;
        private MessageSend messageSend;
        private string loging = string.Empty;
        ChatId chat = new ChatId("@your chanel");
        ChatId currChat;
        private Message Message;
        private const int ONESECOND = 1000;
        private const int ONEMiNUTE = 60 * ONESECOND;
        private const int TWENTYMINETS = 18 * ONEMiNUTE;
        private const int EIGHTHOURS = (60 * ONEMiNUTE * 8) - ONEMiNUTE;
        public TelegramBotClientWork(ITelegramBotClient botClient, ChatId currChat)
        {
            myLock = new object();
            this.botClient = botClient;
            this.currChat = currChat;
            this.Message = null;
            this.messageSend = new MessageSend(botClient);
        }

        public void Dispose()
        {
            botClient = null;
            chat = null;
            currChat = null;
            this.messageSend = null;
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
            //messageSend.MessageWithTime(currChat, "в цикле", date);

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
                //WeatherCurrent(date);
                //MessageWithTime("в иначе", date);
                return ONEMiNUTE;
            }
        }

        /// <summary>
        /// Прогноз на завтра
        /// </summary>
        /// <param name="date"></param>
        private async void WeatherTomorow(DateTime date)
        {
            lock (myLock)
            {
                Message = null;
                //MessageLog = null;
            }
            string currentWeatther = await CurrentWeattherTomorow();
            messageSend.SendMessage(chat, currentWeatther);
            currentWeatther = await CurrentWeatther();
            messageSend.SendMessage(chat, currentWeatther);
            //string logMessage = "в 22 " + date.ToString("dd.MM.yyyy HH:mm:ss");
            //await messageSend.Log(currChat, logMessage);
            await messageSend.Log(currChat, loging);
        }

        /// <summary>
        /// Прогноз на сегодня
        /// </summary>
        /// <param name="date"></param>
        private async void WeatherToDay(DateTime date)
        {
            string currentWeatther = await CurrentWeattherToday();
            messageSend.SendMessage(chat, currentWeatther);
            currentWeatther = await CurrentWeatther();
            Message = await messageSend.GetSendMessage(chat, currentWeatther);
            //string logMessage = "в 8 час " + date.Hour.ToString() + " мин " + date.Minute.ToString();
            //MessageLog = await messageSend.Log(currChat, logMessage);
            await messageSend.Log(currChat, loging);
        }

        /// <summary>
        /// Текущий прогноз
        /// </summary>
        /// <param name="date"></param>
        private async void WeatherCurrent(DateTime date)
        {
            string currentWeatther = await CurrentWeatther();
            await messageSend.GetMessageWithTime(currChat, loging, date);
            lock (myLock)
            {
                //"в 20 40"
                //MessageLog = messageSend.GetMessageWithTime(currChat, loging, date).Result;
                Message = messageSend.GetSendMessage(chat, currentWeatther).Result;
                Message.Text = currentWeatther;
            }
        }

        /// <summary>
        /// Текущий прогноз (изменить сообщение)
        /// </summary>
        /// <param name="chat">Чат в котором надо изменить сообщение</param>
        /// <param name="message">Сообщение которое надо изменить</param>
        /// <param name="date"></param>
        private async void WeatherCurrentEdit(DateTime date)
        {
            string currentWeatther = await CurrentWeatther();
            currentWeatther += Environment.NewLine;
            messageSend.EditMessage(Message, currentWeatther);
            //string logMessage = "в 20 40 на дату " + date.ToString("dd.MM.yyyy HH:mm:ss");
            //messageSend.LogEdit(MessageLog, logMessage);
            await messageSend.GetMessageWithTime(currChat, loging, date);
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

                lock (myLock)
                {
                    loging = weather.GetLoging();
                }
                
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
                
                lock (myLock)
                {
                    loging = weather.GetLogingToDay();
                }

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

                lock (myLock)
                {
                    loging = weather.GetLogingTomorow();
                }

                return currWeather;
            }
        }

    }
}

