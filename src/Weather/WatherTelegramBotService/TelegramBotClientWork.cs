using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private const int ONEMINUTE = 60 * ONESECOND;
        private const int THIRTYMINUTES = 30 * ONEMINUTE;
        private const int ONEHOUR = 60 * ONEMINUTE;
        private const int TWENTYMINETS = 18 * ONEMINUTE;
        private const int EIGHTHOURS = ONEHOUR * 8 - ONEMINUTE;
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
            // на тот случай если включил ночью
            else if (date.Hour == 23 || (date.Hour >= 0 && date.Hour < 6))
            {
                if (date.Hour == 5)
                {
                    if (date.Minute >= 0 && date.Minute < 30)
                    {
                        return THIRTYMINUTES;
                    }
                    else
                    {
                        return ONEMINUTE;
                    }
                }
                else
                {
                    return ONEHOUR;
                }
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
                return ONEMINUTE;
            }
        }

        /// <summary>
        /// Прогноз на завтра
        /// </summary>
        /// <param name="date"></param>
        private void WeatherTomorow(DateTime date)
        {
            string json = CurrentWeattherTomorow();
            SendGraphic(json, true);
            WeatherCurrent(date);
        }

        /// <summary>
        /// Прогноз на сегодня
        /// </summary>
        /// <param name="date"></param>
        private void WeatherToDay(DateTime date)
        {
            string json = CurrentWeattherToday();
            SendGraphic(json, true);
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
            messageBot.EditMessage(Message, currentWeatther, isAccumulate: false);
            messageBot.MessageWithTime(currChat, loger, date);
        }

        /// <summary>
        /// Вставить график 
        /// </summary>
        /// <param name="jsonWeather">Набор данныъ в формате Json</param>
        /// <param name="isToDay">На сегодня, если на завтра то false</param>
        private void SendGraphic(string jsonWeather, bool isToDay)
        {
            WeatherOfFiveDays responseAnswer = JsonConvert.DeserializeObject<WeatherOfFiveDays>(jsonWeather);
            List<DataOfWeatherForPicture> dataOfWeatherForPictures;
            if (isToDay)
            {
                dataOfWeatherForPictures = responseAnswer.GetDataToDayForPicture();
            }
            else
            {
                dataOfWeatherForPictures = responseAnswer.GetDataTomorowForPicture();
            }
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string jsonStr = JsonConvert.SerializeObject(dataOfWeatherForPictures);
                using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost:3000/chart/"))
                {
                    requestMessage.Headers.Add("req", jsonStr);
                    HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                    byte[] responseAsByte = response.Content.ReadAsByteArrayAsync().Result;
                    string caption = isToDay ? "Погода на сегодня" : "Погода на завтра";
                    messageBot.SendMessage(chat, caption);
                    messageBot.SedFoto(chat, caption, responseAsByte);
                }
            }
        }

        /// <summary>
        /// Настройки для запроса на погоду
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetResponseMessage(HttpClient client, string url)
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

                HttpResponseMessage response = GetResponseMessage(client, url).Result;

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

                HttpResponseMessage response = GetResponseMessage(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                return responseAsString;
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

                HttpResponseMessage response = GetResponseMessage(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                return responseAsString;
            }
        }
    }
}

