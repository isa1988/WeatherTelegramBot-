using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
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
        private Message[] Messages;
        private List<WeatherInfoJson> currWeatherInfoCodeList;
        private bool isFirstMessage;
        private string currentWeattherJson;
        private HttpRequestOfOpenWeatherMap requestOfOpenWeatherMap;
        private HttpRequestOfPicure requestOfPicure;
        Dictionary<DataOfWeatherForPictureType, StringBuilder> messageOldJsom;
        private const int ONESECOND = 1000;
        private const int ONEMINUTE = 60 * ONESECOND;
        private const int THIRTYMINUTES = 30 * ONEMINUTE;
        private const int ONEHOUR = 60 * ONEMINUTE;
        private const int TWENTYMINETS = 18 * ONEMINUTE;
        public TelegramBotClientWork(ITelegramBotClient botClient, ChatId currChat)
        {
            this.currChat = currChat;
            this.Messages = null;
            this.messageBot = new MessageBot(botClient);
            this.isFirstMessage = true;
            this.currentWeattherJson = string.Empty;
            this.requestOfPicure = new HttpRequestOfPicure();
            this.requestOfOpenWeatherMap = new HttpRequestOfOpenWeatherMap();
            this.currWeatherInfoCodeList = new List<WeatherInfoJson>();
            this.messageOldJsom = new Dictionary<DataOfWeatherForPictureType, StringBuilder>();
            MessageOldJsomEmpty();
        }

        private void MessageOldJsomEmpty()
        {
            if (messageOldJsom.Count == 0)
            {
                messageOldJsom.Add(DataOfWeatherForPictureType.Temperature, new StringBuilder(string.Empty));
                messageOldJsom.Add(DataOfWeatherForPictureType.FeelTemperature, new StringBuilder(string.Empty));
                messageOldJsom.Add(DataOfWeatherForPictureType.Desc, new StringBuilder(string.Empty));
            }
            else
            {
                messageOldJsom[DataOfWeatherForPictureType.Temperature] = new StringBuilder(string.Empty);
                messageOldJsom[DataOfWeatherForPictureType.FeelTemperature] = new StringBuilder(string.Empty);
                messageOldJsom[DataOfWeatherForPictureType.Desc] = new StringBuilder(string.Empty);
            }
            currWeatherInfoCodeList.Clear();
            Messages = null;
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
                WeatherCurrent();
                WeatherTomorow();
                MessageOldJsomEmpty();
                UpDataofCurrentWeather(currentWeattherJson);
                return ONEHOUR;
            }
            else if (date.Hour == 6 && date.Minute == 0)
            {
                WeatherToDay();
                WeatherCurrent();
                MessageOldJsomEmpty();
                UpDataofCurrentWeather(currentWeattherJson);
                return TWENTYMINETS;
            }
            else if ((date.Hour == 10 && date.Minute == 0) || (date.Hour == 14 && date.Minute == 0) || (date.Hour == 18 && date.Minute == 0))
            {
                WeatherCurrent();
                MessageOldJsomEmpty();
                UpDataofCurrentWeather(currentWeattherJson);
                return TWENTYMINETS;
            }
            // на тот случай если включил ночью
            else if (date.Hour == 23 || (date.Hour >= 0 && date.Hour < 6))
            {
                if (date.Hour == 5)
                {
                    if (date.Minute >= 0 && date.Minute < 30)
                    {
                        UpDataofCurrentWeather(currentWeattherJson);
                        return THIRTYMINUTES;
                    }
                    else
                    {
                        return ONEMINUTE;
                    }
                }
                else
                {
                    if (date.Hour >= 2)
                    {
                        UpDataofCurrentWeather(currentWeattherJson);
                    }
                    messageBot.MessageWithTime(currChat, "Ночь", date);
                    return ONEHOUR;
                }
            }
            else if (date.Minute == 0 || date.Minute == 20 || date.Minute == 40)
            {
                WeatherCurrent();
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
        private void WeatherTomorow()
        {
            string json = requestOfOpenWeatherMap.CurrentWeattherTomorow();
            SendGraphicOfDay(json, false);
        }

        /// <summary>
        /// Прогноз на сегодня
        /// </summary>
        private void WeatherToDay()
        {
            string json = requestOfOpenWeatherMap.CurrentWeattherToday();
            SendGraphicOfDay(json, true);
        }

        /// <summary>
        /// Текущий прогноз
        /// </summary>
        /// <param name="date"></param>
        private void WeatherCurrent()
        {
            currentWeattherJson = requestOfOpenWeatherMap.CurrentWeatther();
            SendCurrrentGraphic(currentWeattherJson);
        }

        /// <summary>
        /// Вставить график 
        /// </summary>
        /// <param name="jsonWeather">Набор данныъ в формате Json</param>
        /// <param name="isToDay">На сегодня, если на завтра то false</param>
        private void SendGraphicOfDay(string jsonWeather, bool isToDay)
        {
            WeatherOfFiveDays responseAnswer = JsonConvert.DeserializeObject<WeatherOfFiveDays>(jsonWeather);
            List<DataOfWeatherForPicture> dataTemperature;
            List<DataOfWeatherForPicture> dataFeelTemperature;
            List<DataOfWeatherForPicture> dataDescTemperature;
            string dateWeather = string.Empty;
            DateTime date;
            if (isToDay)
            {
                dataTemperature = responseAnswer.GetDataToDayForPicture(DataOfWeatherForPictureType.Temperature);
                dataFeelTemperature = responseAnswer.GetDataToDayForPicture(DataOfWeatherForPictureType.FeelTemperature);
                dataDescTemperature = responseAnswer.GetDataToDayForPicture(DataOfWeatherForPictureType.Desc);
                
                date = HelperClass.GetAstanaAndDjako();
            }
            else
            {
                dataTemperature = responseAnswer.GetDataTomorowForPicture(DataOfWeatherForPictureType.Temperature);
                dataFeelTemperature = responseAnswer.GetDataTomorowForPicture(DataOfWeatherForPictureType.FeelTemperature);
                dataDescTemperature = responseAnswer.GetDataTomorowForPicture(DataOfWeatherForPictureType.Desc);
                
                date = HelperClass.GetAstanaAndDjakoTomorow();
            }
            dateWeather = date.ToString("dd.MM.yyyy");

            List<WeatherInfoJson> weatherInfoJsons = responseAnswer.GetWeatherInfoJsonListOfDay(date);

            string jsonTemperature = JsonConvert.SerializeObject(dataTemperature);
            string jsonFeelTemperature = JsonConvert.SerializeObject(dataFeelTemperature);
            string jsonDescTemperature = JsonConvert.SerializeObject(dataDescTemperature);
            string jsonWeatherInfo = JsonConvert.SerializeObject(weatherInfoJsons);

            List<byte[]> responseAsBytes = GetGraphics(jsonTemperature, jsonFeelTemperature, jsonDescTemperature, jsonWeatherInfo, dateWeather);
            string caption = isToDay ? "Погода на сегодня" : "Погода на завтра";
            //messageBot.SendMessage(chat, caption);
            messageBot.SedFoto(chat, caption, responseAsBytes);
        }


        /// <summary>
        /// Вставить график на текущий момент
        /// </summary>
        /// <param name="jsonWeather">Набор данныъ в формате Json</param>
        /// <param name="isToDay">На сегодня, если на завтра то false</param>
        private void SendCurrrentGraphic(string jsonWeather)
        {
            UpDataofCurrentWeather(jsonWeather);

            if (isFirstMessage)
            {
                isFirstMessage = false;
                return;
            }

            string dateWeather = HelperClass.GetAstanaAndDjako().ToString("dd.MM.yyyy");
            string jsonWeatherInfo = JsonConvert.SerializeObject(currWeatherInfoCodeList);

            List<byte[]> responseAsBytes = GetGraphics(messageOldJsom[DataOfWeatherForPictureType.Temperature].ToString(),
                                                       messageOldJsom[DataOfWeatherForPictureType.FeelTemperature].ToString(),
                                                       messageOldJsom[DataOfWeatherForPictureType.Desc].ToString(), jsonWeatherInfo, dateWeather);

            string caption = "Погода на текущий момент";
            //messageBot.SendMessage(currChat, caption);
            if (Messages == null || Messages.Length != 2)
            {
                Messages = messageBot.SedFoto(chat, caption, responseAsBytes);
            }
            else
            {
                Messages = messageBot.EditFoto(Messages, responseAsBytes);
            }
        }

        /// <summary>
        /// Обновить данные для погоды
        /// </summary>
        /// <param name="jsonWeather">Набор данныъ в формате Json</param>
        private void UpDataofCurrentWeather(string jsonWeather)
        {
            Weather responseAnswer = JsonConvert.DeserializeObject<Weather>(jsonWeather);
            DataOfWeatherForPicture dataTemperature = responseAnswer.GetDataOfWeatherForPictures(DataOfWeatherForPictureType.Temperature);
            DataOfWeatherForPicture dataFeelTemperature = responseAnswer.GetDataOfWeatherForPictures(DataOfWeatherForPictureType.FeelTemperature);
            DataOfWeatherForPicture dataDescTemperature = responseAnswer.GetDataOfWeatherForPictures(DataOfWeatherForPictureType.Desc);

            responseAnswer.UpdateListWeatherInfoJson(currWeatherInfoCodeList);

            string jsonTemperature = JsonConvert.SerializeObject(dataTemperature);
            string jsonFeelTemperature = JsonConvert.SerializeObject(dataFeelTemperature);
            string jsonDescTemperature = JsonConvert.SerializeObject(dataDescTemperature);
            
            SaveMessga(DataOfWeatherForPictureType.Temperature, jsonTemperature);
            SaveMessga(DataOfWeatherForPictureType.FeelTemperature, jsonFeelTemperature);
            SaveMessga(DataOfWeatherForPictureType.Desc, jsonDescTemperature);
        }

        /// <summary>
        /// Сделать запрос на сервер Node Js и вернуть графики
        /// </summary>
        /// <param name="jsonTemperature">Теипература</param>
        /// <param name="jsonFeelTemperature">Температура по ощущениям</param>
        /// <param name="jsonDescTemperature"></param>
        /// <param name="jsonWeatherInfo">Данные о погоды</param>
        /// <param name="dateWeather">Дата</param>
        /// <returns></returns>
        private List<byte[]> GetGraphics(string jsonTemperature, string jsonFeelTemperature, string jsonDescTemperature,
                                         string jsonWeatherInfo, string dateWeather)
        {
            List<byte[]> responseAsBytes = new List<byte[]>();

            byte[] photoTempeture = requestOfPicure.GetPhotoTempeture(jsonTemperature, jsonFeelTemperature, dateWeather);
            responseAsBytes.Add(photoTempeture);

            byte[] photoDescTempeture = requestOfPicure.GetPhotoDescTempeture(jsonDescTemperature, jsonWeatherInfo, dateWeather);
            responseAsBytes.Add(photoDescTempeture);

            return responseAsBytes;
        }

        /// <summary>
        /// Сохранить значение
        /// </summary>
        /// <param name="key">Тип сообщения</param>
        /// <param name="json">Сообщение в формате JSon</param>
        private void SaveMessga(DataOfWeatherForPictureType key, string json)
        {
            string oldMesage = messageOldJsom[key].ToString();
            messageOldJsom[key] = new StringBuilder(string.Empty);
            if (string.IsNullOrEmpty(oldMesage))
            {
                messageOldJsom[key].Append(json);
            }
            else
            {
                StringBuilder buffer = new StringBuilder(string.Empty);
                if (oldMesage[0].ToString() == "[")
                {
                    buffer.Append(oldMesage.Insert(oldMesage.Length - 1, ", " + json));
                }
                else
                {
                    buffer.Append("[");
                    buffer.Append(oldMesage);
                    buffer.Append(", ");
                    buffer.Append(json);
                    buffer.Append("]");
                }
                messageOldJsom[key] = buffer;
            }
        }
    }
}

