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
        public List<Weather> rootObjects { get; set; }

        public City City { get; set; }
        public WeatherOfFiveDays()
        {
            rootObjects = new List<Weather>();
        }

        public string GetWeatherOfToDay()
        {
            DateTime date = DateTime.UtcNow.AddHours(6);
            return GetODate(date, "Доброе утро. Погода на сегодня");
        }

        public string GetWeatherOfTomorow()
        {
            DateTime date = DateTime.UtcNow.AddHours(6).AddDays(1);
            return GetODate(date, "Добрый вечер. Погода на завтра");
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
            if (rootObjects == null || rootObjects.Count == 0) return string.Empty;
            for (int i = 0; i < rootObjects.Count; i++)
            {
                rootObjects[i].ConvertionDtTxt();
            }
            
            List<Weather> rootObjectTemps = rootObjects.Where(n => n.DtAstanaAndDjako.Date == date.Date).ToList();
            StringBuilder caption = new StringBuilder("<b>"+startCaption+"</b>");

            for (int i = 0; i < rootObjectTemps.Count; i++)
            {
                if (isLoging)
                {
                    caption.Append(rootObjectTemps[i].GetLoging());
                }
                else
                {
                    caption.Append(rootObjectTemps[i].GetWeatherOfToDayOrTomorow());
                }
            }

            return caption.ToString();
        }
        
        public string GetLogingToDay()
        {
            DateTime date = DateTime.UtcNow.AddHours(6);
            return GetODate(date, "логирование на сегодня");
        }

        public string GetLogingTomorow()
        {
            DateTime date = DateTime.UtcNow.AddHours(6).AddDays(1);
            return GetODate(date, "логирование на завтра");
        }
    }
}
