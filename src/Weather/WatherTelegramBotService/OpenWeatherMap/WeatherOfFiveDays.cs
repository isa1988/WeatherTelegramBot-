using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    class WeatherOfFiveDays
    {
        public string Cod { get; set; }
        public string message { get; set; }

        public int Cnt { get; set; }

        [JsonProperty("list")]
        public List<Weather> Weathers { get; set; }

        public City City { get; set; }

        [JsonIgnore]
        private List<Weather> WeatherOfDay { get; set; }
        public WeatherOfFiveDays()
        {
            Weathers = new List<Weather>();
            WeatherOfDay = new List<Weather>();
        }


        /// <summary>
        /// Выявить погоду на день
        /// </summary>
        /// <param name="date">Интересующая дата</param>
        private void UpdateWeatherOfDay(DateTime date)
        {
            if (WeatherOfDay?.Count > 0) return;

            
            for (int i = 0; i < Weathers.Count; i++)
            {
                Weathers[i].ConvertionDtTxt();
            }

            WeatherOfDay = Weathers.Where(n => n.DtAstanaAndDjako.Date == date.Date).ToList();
        }

        /// <summary>
        /// Прогноз погоды на сегодняшний день
        /// </summary>
        /// <returns></returns>
        public string GetWeatherOfToDay()
        {
            DateTime dateAstanaAndDjako = HelperClass.GetAstanaAndDjako();
            return GetODate(dateAstanaAndDjako, "Доброе утро. Погода на сегодня");
        }

        /// <summary>
        /// Прогноз погоды на завтра
        /// </summary>
        /// <returns></returns>
        public string GetWeatherOfTomorow()
        {
            DateTime dateAstanaAndDjakoTomorow = HelperClass.GetAstanaAndDjakoTomorow();
            return GetODate(dateAstanaAndDjakoTomorow, "Добрый вечер. Погода на завтра");
        }

        /// <summary>
        /// Логирование на сегодняшний день
        /// </summary>
        /// <returns></returns>
        public string GetLogingToDay()
        {
            DateTime dateAstanaAndDjako = HelperClass.GetAstanaAndDjako();
            return GetODate(dateAstanaAndDjako, "логирование на сегодня", true);
        }

        /// <summary>
        /// Логирование на завтра
        /// </summary>
        /// <returns></returns>
        public string GetLogingTomorow()
        {
            DateTime dateAstanaAndDjakoTomorow = HelperClass.GetAstanaAndDjakoTomorow();
            return GetODate(dateAstanaAndDjakoTomorow, "логирование на завтра", true);
        }

        /// <summary>
        /// Сообщение на дату
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="startCaption">Первое сообщение</param>
        /// <param name="isLoging">Логирование</param>
        /// <returns></returns>
        public string GetODate(DateTime date, string startCaption, bool isLoging = false)
        {
            if (Weathers == null || Weathers.Count == 0) return string.Empty;
            UpdateWeatherOfDay(date);
            StringBuilder caption = new StringBuilder("<b>"+startCaption+"</b>");

            for (int i = 0; i < WeatherOfDay.Count; i++)
            {
                if (isLoging)
                {
                    caption.Append(WeatherOfDay[i].GetLoging());
                }
                else
                {
                    caption.Append(WeatherOfDay[i].GetWeatherOfToDayOrTomorow());
                }
            }

            return caption.ToString();
        }

        /// <summary>
        /// Данные для графика на сегодняшний день
        /// </summary>
        /// <returns></returns>
        public List<DataOfWeatherForPicture> GetDataToDayForPicture()
        {
            DateTime dateAstanaAndDjako = HelperClass.GetAstanaAndDjako();
            return GetDataOfWeatherForPictures(dateAstanaAndDjako);
        }

        /// <summary>
        /// Данные для графика на завтра
        /// </summary>
        /// <returns></returns>
        public List<DataOfWeatherForPicture> GetDataTomorowForPicture()
        {
            DateTime dateAstanaAndDjakoTomorow = HelperClass.GetAstanaAndDjakoTomorow();
            return GetDataOfWeatherForPictures(dateAstanaAndDjakoTomorow);
        }

        /// <summary>
        /// Данные для графика
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public List<DataOfWeatherForPicture> GetDataOfWeatherForPictures(DateTime date)
        {
            List<DataOfWeatherForPicture> dataOfWeatherForPictures = new List<DataOfWeatherForPicture>();
            if (Weathers == null || Weathers.Count == 0) return dataOfWeatherForPictures;
            UpdateWeatherOfDay(date);

            for (int i = 0; i < WeatherOfDay.Count; i++)
            {
                dataOfWeatherForPictures.Add(new DataOfWeatherForPicture(WeatherOfDay[i].DtAstanaAndDjako.ToString("HH:mm"), (int)WeatherOfDay[i].Temperature.Temp));
            }

            return dataOfWeatherForPictures;
        }
    }
}
