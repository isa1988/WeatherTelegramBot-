using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WatherTelegramBotService.OpenWeatherMap
{
    class RootObjectOfFiveDays : IWeather
    {
        public string Cod { get; set; }
        public string message { get; set; }

        public int Cnt { get; set; }

        [JsonProperty("list")]
        public List<RootObject> rootObjects { get; set; }

        public City City { get; set; }
        public RootObjectOfFiveDays()
        {
            rootObjects = new List<RootObject>();
        }

        public string GetCurrent()
        {
            throw new NotImplementedException();
        }

        public string GetToDay()
        {
            DateTime date = DateTime.UtcNow.AddHours(6);
            return GetODate(date, "Доброе утро. Погода на сегодня");
        }

        public string GetTomorow()
        {
            DateTime date = DateTime.UtcNow.AddHours(6).AddDays(1);
            return GetODate(date, "Добрый вечер. Погода на завтра");
        }

        /// <summary>
        /// Сообщение на дату
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="startCaption">Первое сообщение</param>
        /// <returns></returns>
        public string GetODate(DateTime date, string startCaption)
        {
            if (rootObjects == null || rootObjects.Count == 0) return string.Empty;
            for (int i = 0; i < rootObjects.Count; i++)
            {
                rootObjects[i].ConvertionDtTxt();
            }
            
            List<RootObject> rootObjectTemps = rootObjects.Where(n => n.DtAstanaAndDjako.Date == date.Date).ToList();
            StringBuilder caption = new StringBuilder(startCaption);

            for (int i = 0; i < rootObjectTemps.Count; i++)
            {
                caption.Append(rootObjectTemps[i].GetToDay());
            }

            return caption.ToString();
        }
    }
}
