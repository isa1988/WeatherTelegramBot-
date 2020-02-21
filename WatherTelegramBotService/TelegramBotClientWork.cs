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
            /*else if ((date.Hour >= 22 && date.Hour <= 23) || (date.Hour >= 0 && date.Hour < 6))
            {
                if (date.Hour == 5 && date.Minute >= 30)
                {
                    MessageWithTime("5 утра", date);
                    return ONEMiNUTE;
                }
                MessageWithTime("ночь", date);
                return TWENTYMINETS;
            }*/
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

        /// <summary>
        /// Для тестирования погоды на текущий момент
        /// </summary>
        /// <returns></returns>
        string CurrentWeattherTest()
        {
            //612
            string textTest =
             "{\"coord\":{\"lon\":74.59,\"lat\":42.87},\"weather\":[{\"id\":500,\"main\":\"Snow\",\"description\":\"интенсивная ледяная крупа\",\"icon\":\"13n\"}],\"base\":\"stations\",\"main\"" +
             ":{\"temp\":0,\"feels_like\":-2.75,\"temp_min\":0,\"temp_max\":0,\"pressure\":1029,\"humidity\":97},\"visibility\":7000,\"wind\":{\"speed\":1,\"deg\":210},\"clouds\":{\"all\":90}," +
             "\"dt\":1581802447,\"sys\":{\"type\":1,\"id\":8871,\"country\":\"KG\",\"sunrise\":1581818341,\"sunset\":1581856367},\"timezone\":21600,\"id\":1528675,\"name\":\"Бишкек\",\"cod\":200}";

            RootObject response = JsonConvert.DeserializeObject<RootObject>(textTest);
            IWeather weather = response;
            string currWeather = weather.GetCurrent();
            return currWeather;
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
        /// Для тестирования погоды на текущий день
        /// </summary>
        /// <returns></returns>
        string CurrentWeattherTodayTest()
        {
            string textTest = GetJSonForTest();
            RootObjectOfFiveDays response = JsonConvert.DeserializeObject<RootObjectOfFiveDays>(textTest);
            IWeather weather = response;
            string currWeather = weather.GetToDay();
            return currWeather;
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
        /// Для тестирования погоды на завтра
        /// </summary>
        /// <returns></returns>
        string CurrentWeattherTomorowTest()
        {
            string textTest = GetJSonForTest();
            RootObjectOfFiveDays response = JsonConvert.DeserializeObject<RootObjectOfFiveDays>(textTest);
            IWeather weather = response;
            string currWeather = weather.GetTomorow();
            return currWeather;
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

        /// <summary>
        /// JSon для тестирования
        /// </summary>
        /// <returns></returns>
        private string GetJSonForTest()
        {
            return "{\"cod\":\"200\",\"message\":0,\"cnt\":40,\"list\":[{\"dt\":1581962400,\"main\":{\"temp\":-0.99,\"feels_like\":-4.43,\"temp_min\":-1.08,\"temp_max\":-0.99,\"pressure\":1035,\"sea_level\"" +
                ":1035,\"grnd_level\":953,\"humidity\":66,\"temp_kf\":0.09},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04n\"}],\"clouds\":{\"all\":100},\"wind\":{\"" +
                "speed\":0.97,\"deg\":140},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-17 18:00:00\"},{\"dt\":1581973200,\"main\":{\"temp\":-1.16,\"feels_like\":-4.2,\"temp_min\":-1.23,\"temp_max\":-1.16,\"" +
                "pressure\":1033,\"sea_level\":1033,\"grnd_level\":952,\"humidity\":61,\"temp_kf\":0.07},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04n\"}],\"clouds\"" +
                ":{\"all\":100},\"wind\":{\"speed\":0.24,\"deg\":203},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-17 21:00:00\"},{\"dt\":1581984000,\"main\":{\"temp\":-1.2,\"feels_like\":-4.61,\"temp_min\"" +
                ":-1.24,\"temp_max\":-1.2,\"pressure\":1033,\"sea_level\":1033,\"grnd_level\":952,\"humidity\":72,\"temp_kf\":0.04},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\"" +
                ",\"icon\":\"04n\"}],\"clouds\":{\"all\":100},\"wind\":{\"speed\":1.06,\"deg\":230},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-18 00:00:00\"},{\"dt\":1581994800,\"main\":{\"temp\":-2.02,\"" +
                "feels_like\":-4.55,\"temp_min\":-2.04,\"temp_max\":-2.02,\"pressure\":1033,\"sea_level\":1033,\"grnd_level\":952,\"humidity\":94,\"temp_kf\":0.02},\"weather\":[{\"id\":600,\"main\":\"Snow\"" +
                ",\"description\":\"небольшой снег\",\"icon\":\"13d\"}],\"clouds\":{\"all\":100},\"wind\":{\"speed\":0.23,\"deg\":25},\"snow\":{\"3h\":0.69},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"" +
                "2020-02-18 03:00:00\"},{\"dt\":1582005600,\"main\":{\"temp\":-0.42,\"feels_like\":-3.94,\"temp_min\":-0.42,\"temp_max\":-0.42,\"pressure\":1032,\"sea_level\":1032,\"grnd_level\":953,\"" +
                "humidity\":85,\"temp_kf\":0},\"weather\":[{\"id\":600,\"main\":\"Snow\",\"description\":\"небольшой снег\",\"icon\":\"13d\"}],\"clouds\":{\"all\":100},\"wind\":{\"speed\":1.69,\"deg\":22},\"" +
                "snow\":{\"3h\":0.44},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-18 06:00:00\"},{\"dt\":1582016400,\"main\":{\"temp\":-0.29,\"feels_like\":-4.51,\"temp_min\":-0.29,\"temp_max\":-0.29,\"" +
                "pressure\":1032,\"sea_level\":1032,\"grnd_level\":953,\"humidity\":87,\"temp_kf\":0},\"weather\":[{\"id\":600,\"main\":\"Snow\",\"description\":\"небольшой снег\",\"icon\":\"13d\"}],\"clouds\"" +
                ":{\"all\":100},\"wind\":{\"speed\":2.76,\"deg\":355},\"snow\":{\"3h\":0.31},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-18 09:00:00\"},{\"dt\":1582027200,\"main\":{\"temp\":-1.15,\"" +
                "feels_like\":-5.24,\"temp_min\":-1.15,\"temp_max\":-1.15,\"pressure\":1035,\"sea_level\":1035,\"grnd_level\":955,\"humidity\":92,\"temp_kf\":0},\"weather\":[{\"id\":600,\"main\":\"Snow\",\"" +
                "description\":\"небольшой снег\",\"icon\":\"13d\"}],\"clouds\":{\"all\":100},\"wind\":{\"speed\":2.57,\"deg\":317},\"snow\":{\"3h\":0.5},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-18 12:00:00\"" +
                "},{\"dt\":1582038000,\"main\":{\"temp\":-2.41,\"feels_like\":-6.3,\"temp_min\":-2.41,\"temp_max\":-2.41,\"pressure\":1039,\"sea_level\":1039,\"grnd_level\":957,\"humidity\":91,\"temp_kf\":0},\"" +
                "weather\":[{\"id\":600,\"main\":\"Snow\",\"description\":\"небольшой снег\",\"icon\":\"13n\"}],\"clouds\":{\"all\":93},\"wind\":{\"speed\":2.04,\"deg\":280},\"snow\":{\"3h\":0.63},\"sys\":{\"pod\"" +
                ":\"n\"},\"dt_txt\":\"2020-02-18 15:00:00\"},{\"dt\":1582048800,\"main\":{\"temp\":-3.41,\"feels_like\":-6.23,\"temp_min\":-3.41,\"temp_max\":-3.41,\"pressure\":1040,\"sea_level\":1040,\"" +
                "grnd_level\":958,\"humidity\":87,\"temp_kf\":0},\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04n\"}],\"clouds\":{\"all\":71},\"wind\":{\"" +
                "speed\":0.26,\"deg\":247},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-18 18:00:00\"},{\"dt\":1582059600,\"main\":{\"temp\":-3.74,\"feels_like\":-7.14,\"temp_min\":-3.74,\"temp_max\":-3.74,\"" +
                "pressure\":1041,\"sea_level\":1041,\"grnd_level\":958,\"humidity\":88,\"temp_kf\":0},\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"небольшая облачность\",\"icon\":\"02n\"}],\"" +
                "clouds\":{\"all\":18},\"wind\":{\"speed\":1.06,\"deg\":68},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-18 21:00:00\"},{\"dt\":1582070400,\"main\":{\"temp\":-4.11,\"feels_like\":-7.56,\"temp_min\"" +
                ":-4.11,\"temp_max\":-4.11,\"pressure\":1041,\"sea_level\":1041,\"grnd_level\":957,\"humidity\":88,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"" +
                "01n\"}],\"clouds\":{\"all\":9},\"wind\":{\"speed\":1.09,\"deg\":48},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-19 00:00:00\"},{\"dt\":1582081200,\"main\":{\"temp\":-3.42,\"feels_like\":-6.81,\"" +
                "temp_min\":-3.42,\"temp_max\":-3.42,\"pressure\":1041,\"sea_level\":1041,\"grnd_level\":957,\"humidity\":80,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"" +
                "icon\":\"01d\"}],\"clouds\":{\"all\":0},\"wind\":{\"speed\":0.92,\"deg\":56},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-19 03:00:00\"},{\"dt\":1582092000,\"main\":{\"temp\":0.08,\"feels_like\"" +
                ":-4.07,\"temp_min\":0.08,\"temp_max\":0.08,\"pressure\":1039,\"sea_level\":1039,\"grnd_level\":957,\"humidity\":58,\"temp_kf\":0},\"weather\":[{\"id\":802,\"main\":\"Clouds\",\"description\":\"" +
                "переменная облачность\",\"icon\":\"03d\"}],\"clouds\":{\"all\":31},\"wind\":{\"speed\":1.89,\"deg\":22},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-19 06:00:00\"},{\"dt\":1582102800,\"main\":{\"" +
                "temp\":1.43,\"feels_like\":-3.14,\"temp_min\":1.43,\"temp_max\":1.43,\"pressure\":1037,\"sea_level\":1037,\"grnd_level\":955,\"humidity\":63,\"temp_kf\":0},\"weather\":[{\"id\":803,\"main\":\"" +
                "Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04d\"}],\"clouds\":{\"all\":57},\"wind\":{\"speed\":2.82,\"deg\":311},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-19 09:00:00\"},{\"" +
                "dt\":1582113600,\"main\":{\"temp\":-0.77,\"feels_like\":-4.41,\"temp_min\":-0.77,\"temp_max\":-0.77,\"pressure\":1037,\"sea_level\":1037,\"grnd_level\":956,\"humidity\":79,\"temp_kf\":0},\"weather\"" +
                ":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04d\"}],\"clouds\":{\"all\":78},\"wind\":{\"speed\":1.63,\"deg\":302},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"" +
                "2020-02-19 12:00:00\"},{\"dt\":1582124400,\"main\":{\"temp\":-2.46,\"feels_like\":-5.77,\"temp_min\":-2.46,\"temp_max\":-2.46,\"pressure\":1038,\"sea_level\":1038,\"grnd_level\":956,\"humidity\"" +
                ":82,\"temp_kf\":0},\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04n\"}],\"clouds\":{\"all\":74},\"wind\":{\"speed\":0.99,\"deg\":83},\"sys\":{\"" +
                "pod\":\"n\"},\"dt_txt\":\"2020-02-19 15:00:00\"},{\"dt\":1582135200,\"main\":{\"temp\":-2.45,\"feels_like\":-6.31,\"temp_min\":-2.45,\"temp_max\":-2.45,\"pressure\":1036,\"sea_level\":1036,\"" +
                "grnd_level\":953,\"humidity\":78,\"temp_kf\":0},\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04n\"}],\"clouds\":{\"all\":73},\"wind\":{\"speed\"" +
                ":1.67,\"deg\":81},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-19 18:00:00\"},{\"dt\":1582146000,\"main\":{\"temp\":-2.69,\"feels_like\":-6.73,\"temp_min\":-2.69,\"temp_max\":-2.69,\"pressure\"" +
                ":1034,\"sea_level\":1034,\"grnd_level\":951,\"humidity\":71,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01n\"}],\"clouds\":{\"all\":3},\"wind\":{\"" +
                "speed\":1.74,\"deg\":64},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-19 21:00:00\"},{\"dt\":1582156800,\"main\":{\"temp\":-2.88,\"feels_like\":-6.5,\"temp_min\":-2.88,\"temp_max\":-2.88,\"pressure\"" +
                ":1032,\"sea_level\":1032,\"grnd_level\":949,\"humidity\":66,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01n\"}],\"clouds\":{\"all\":2},\"wind\":{\"" +
                "speed\":1,\"deg\":26},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-20 00:00:00\"},{\"dt\":1582167600,\"main\":{\"temp\":-2.04,\"feels_like\":-6.26,\"temp_min\":-2.04,\"temp_max\":-2.04,\"pressure\"" +
                ":1031,\"sea_level\":1031,\"grnd_level\":949,\"humidity\":59,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01d\"}],\"clouds\":{\"all\":0},\"wind\":{\"" +
                "speed\":1.77,\"deg\":51},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-20 03:00:00\"},{\"dt\":1582178400,\"main\":{\"temp\":2.33,\"feels_like\":-2.01,\"temp_min\":2.33,\"temp_max\":2.33,\"pressure\"" +
                ":1028,\"sea_level\":1028,\"grnd_level\":947,\"humidity\":46,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01d\"}],\"clouds\":{\"all\":0},\"wind\":{\"" +
                "speed\":2.05,\"deg\":26},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-20 06:00:00\"},{\"dt\":1582189200,\"main\":{\"temp\":4.37,\"feels_like\":0.06,\"temp_min\":4.37,\"temp_max\":4.37,\"pressure\"" +
                ":1024,\"sea_level\":1024,\"grnd_level\":945,\"humidity\":52,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01d\"}],\"clouds\":{\"all\":0},\"wind\":{\"" +
                "speed\":2.49,\"deg\":9},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-20 09:00:00\"},{\"dt\":1582200000,\"main\":{\"temp\":1.5,\"feels_like\":-2.14,\"temp_min\":1.5,\"temp_max\":1.5,\"pressure\":1023,\"" +
                "sea_level\":1023,\"grnd_level\":944,\"humidity\":72,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01d\"}],\"clouds\":{\"all\":2},\"wind\":{\"speed\":1.79,\"" +
                "deg\":14},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-20 12:00:00\"},{\"dt\":1582210800,\"main\":{\"temp\":-1,\"feels_like\":-4.2,\"temp_min\":-1,\"temp_max\":-1,\"pressure\":1024,\"sea_level\":1024,\"" +
                "grnd_level\":944,\"humidity\":74,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01n\"}],\"clouds\":{\"all\":1},\"wind\":{\"speed\":0.84,\"deg\":37},\"sys\"" +
                ":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-20 15:00:00\"},{\"dt\":1582221600,\"main\":{\"temp\":-0.95,\"feels_like\":-4.58,\"temp_min\":-0.95,\"temp_max\":-0.95,\"pressure\":1023,\"sea_level\":1023,\"grnd_level\"" +
                ":943,\"humidity\":73,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01n\"}],\"clouds\":{\"all\":1},\"wind\":{\"speed\":1.43,\"deg\":32},\"sys\":{\"pod\":\"" +
                "n\"},\"dt_txt\":\"2020-02-20 18:00:00\"},{\"dt\":1582232400,\"main\":{\"temp\":-0.67,\"feels_like\":-3.88,\"temp_min\":-0.67,\"temp_max\":-0.67,\"pressure\":1022,\"sea_level\":1022,\"grnd_level\":941,\"" +
                "humidity\":72,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01n\"}],\"clouds\":{\"all\":5},\"wind\":{\"speed\":0.84,\"deg\":66},\"sys\":{\"pod\":\"n\"},\"" +
                "dt_txt\":\"2020-02-20 21:00:00\"},{\"dt\":1582243200,\"main\":{\"temp\":-0.49,\"feels_like\":-4.55,\"temp_min\":-0.49,\"temp_max\":-0.49,\"pressure\":1021,\"sea_level\":1021,\"grnd_level\":939,\"humidity\"" +
                ":68,\"temp_kf\":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01n\"}],\"clouds\":{\"all\":4},\"wind\":{\"speed\":1.98,\"deg\":81},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"" +
                "2020-02-21 00:00:00\"},{\"dt\":1582254000,\"main\":{\"temp\":1.74,\"feels_like\":-2.05,\"temp_min\":1.74,\"temp_max\":1.74,\"pressure\":1020,\"sea_level\":1020,\"grnd_level\":939,\"humidity\":57,\"temp_kf\"" +
                ":0},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"ясно\",\"icon\":\"01d\"}],\"clouds\":{\"all\":0},\"wind\":{\"speed\":1.56,\"deg\":95},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"" +
                "2020-02-21 03:00:00\"},{\"dt\":1582264800,\"main\":{\"temp\":9.19,\"feels_like\":5.19,\"temp_min\":9.19,\"temp_max\":9.19,\"pressure\":1018,\"sea_level\":1018,\"grnd_level\":938,\"humidity\":40,\"" +
                "temp_kf\":0},\"weather\":[{\"id\":802,\"main\":\"Clouds\",\"description\":\"переменная облачность\",\"icon\":\"03d\"}],\"clouds\":{\"all\":36},\"wind\":{\"speed\":2.19,\"deg\":22},\"sys\":{\"pod\":\"d\"" +
                "},\"dt_txt\":\"2020-02-21 06:00:00\"},{\"dt\":1582275600,\"main\":{\"temp\":10.25,\"feels_like\":6.75,\"temp_min\":10.25,\"temp_max\":10.25,\"pressure\":1016,\"sea_level\":1016,\"grnd_level\":938,\"humidity\"" +
                ":46,\"temp_kf\":0},\"weather\":[{\"id\":802,\"main\":\"Clouds\",\"description\":\"переменная облачность\",\"icon\":\"03d\"}],\"clouds\":{\"all\":27},\"wind\":{\"speed\":1.99,\"deg\":343},\"sys\":{\"pod\":\"d\"}" +
                ",\"dt_txt\":\"2020-02-21 09:00:00\"},{\"dt\":1582286400,\"main\":{\"temp\":7.51,\"feels_like\":4.38,\"temp_min\":7.51,\"temp_max\":7.51,\"pressure\":1016,\"sea_level\":1016,\"grnd_level\":938,\"humidity\"" +
                ":59,\"temp_kf\":0},\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"небольшая облачность\",\"icon\":\"02d\"}],\"clouds\":{\"all\":13},\"wind\":{\"speed\":1.64,\"deg\":38},\"sys\":{\"pod\":\"d\"}" +
                ",\"dt_txt\":\"2020-02-21 12:00:00\"},{\"dt\":1582297200,\"main\":{\"temp\":5.69,\"feels_like\":2.25,\"temp_min\":5.69,\"temp_max\":5.69,\"pressure\":1017,\"sea_level\":1017,\"grnd_level\":939,\"humidity\":60" +
                ",\"temp_kf\":0},\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04n\"}],\"clouds\":{\"all\":60},\"wind\":{\"speed\":1.78,\"deg\":79},\"sys\":{\"pod\":\"n\"}" +
                ",\"dt_txt\":\"2020-02-21 15:00:00\"},{\"dt\":1582308000,\"main\":{\"temp\":4.73,\"feels_like\":1.33,\"temp_min\":4.73,\"temp_max\":4.73,\"pressure\":1017,\"sea_level\":1017,\"grnd_level\":939,\"humidity\":62" +
                ",\"temp_kf\":0},\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"облачно с прояснениями\",\"icon\":\"04n\"}],\"clouds\":{\"all\":55},\"wind\":{\"speed\":1.64,\"deg\":297},\"sys\":{\"pod\":\"n\"}" +
                ",\"dt_txt\":\"2020-02-21 18:00:00\"},{\"dt\":1582318800,\"main\":{\"temp\":2.97,\"feels_like\":0.07,\"temp_min\":2.97,\"temp_max\":2.97,\"pressure\":1018,\"sea_level\":1018,\"grnd_level\":940,\"humidity\":69" +
                ",\"temp_kf\":0},\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"небольшая облачность\",\"icon\":\"02n\"}],\"clouds\":{\"all\":24},\"wind\":{\"speed\":0.88,\"deg\":326},\"sys\":{\"pod\":\"n\"}" +
                ",\"dt_txt\":\"2020-02-21 21:00:00\"},{\"dt\":1582329600,\"main\":{\"temp\":2.68,\"feels_like\":-1.03,\"temp_min\":2.68,\"temp_max\":2.68,\"pressure\":1019,\"sea_level\":1019,\"grnd_level\":941,\"humidity\":70" +
                ",\"temp_kf\":0},\"weather\":[{\"id\":802,\"main\":\"Clouds\",\"description\":\"переменная облачность\",\"icon\":\"03n\"}],\"clouds\":{\"all\":50},\"wind\":{\"speed\":2.03,\"deg\":112},\"sys\":{\"pod\":\"n\"}" +
                ",\"dt_txt\":\"2020-02-22 00:00:00\"},{\"dt\":1582340400,\"main\":{\"temp\":4.06,\"feels_like\":1.09,\"temp_min\":4.06,\"temp_max\":4.06,\"pressure\":1021,\"sea_level\":1021,\"grnd_level\":942,\"humidity\":60" +
                ",\"temp_kf\":0},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04d\"}],\"clouds\":{\"all\":95},\"wind\":{\"speed\":0.84,\"deg\":128},\"sys\":{\"pod\":\"d\"},\"dt_txt\"" +
                ":\"2020-02-22 03:00:00\"},{\"dt\":1582351200,\"main\":{\"temp\":10.01,\"feels_like\":6.5,\"temp_min\":10.01,\"temp_max\":10.01,\"pressure\":1020,\"sea_level\":1020,\"grnd_level\":942,\"humidity\":39,\"temp_kf\"" +
                ":0},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04d\"}],\"clouds\":{\"all\":94},\"wind\":{\"speed\":1.55,\"deg\":106},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"" +
                "2020-02-22 06:00:00\"},{\"dt\":1582362000,\"main\":{\"temp\":12.85,\"feels_like\":8.78,\"temp_min\":12.85,\"temp_max\":12.85,\"pressure\":1019,\"sea_level\":1019,\"grnd_level\":940,\"humidity\":33,\"temp_kf\"" +
                ":0},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04d\"}],\"clouds\":{\"all\":100},\"wind\":{\"speed\":2.41,\"deg\":95},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"" +
                "2020-02-22 09:00:00\"},{\"dt\":1582372800,\"main\":{\"temp\":9.6,\"feels_like\":6.38,\"temp_min\":9.6,\"temp_max\":9.6,\"pressure\":1020,\"sea_level\":1020,\"grnd_level\":941,\"humidity\":46,\"temp_kf\":0},\"" +
                "weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04d\"}],\"clouds\":{\"all\":99},\"wind\":{\"speed\":1.47,\"deg\":15},\"sys\":{\"pod\":\"d\"},\"dt_txt\":\"2020-02-22 12:00:00\"" +
                "},{\"dt\":1582383600,\"main\":{\"temp\":5.79,\"feels_like\":1.09,\"temp_min\":5.79,\"temp_max\":5.79,\"pressure\":1023,\"sea_level\":1023,\"grnd_level\":944,\"humidity\":70,\"temp_kf\":0},\"weather\":[{\"id\"" +
                ":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04n\"}],\"clouds\":{\"all\":100},\"wind\":{\"speed\":4.04,\"deg\":264},\"sys\":{\"pod\":\"n\"},\"dt_txt\":\"2020-02-22 15:00:00\"}],\"city\":{\"" +
                "id\":1528675,\"name\":\"Бишкек\",\"coord\":{\"lat\":42.87,\"lon\":74.59},\"country\":\"KG\",\"population\":900000,\"timezone\":21600,\"sunrise\":1581904656,\"sunset\":1581942845}}";
        }

    }
}
