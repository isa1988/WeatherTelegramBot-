﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WatherTelegramBotService.OpenWeatherMap
{
    class RootObject : IWeather
    {
        public Coord Coord { get; set; }
        public List<Weather> Weather { get; set; }
        public string @Base { get; set; }
        [JsonProperty("main")]
        public MainWeather MainWeather { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public int Dt { get; set; }
        public Sys Sys { get; set; }
        public int Timezone { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cod { get; set; }

        
        [JsonProperty("dt_txt")]
        public string DtTxt { get; set; }

        public DateTime DtUtc { get; set; }

        public DateTime DtAstanaAndDjako { get; set; }

        public void ConvertionDtTxt()
        {
            if (DtTxt?.Length > 0)
            {
                DtAstanaAndDjako = Convert.ToDateTime(DtTxt); 
                DtUtc = DtAstanaAndDjako.AddHours(-6);
            }
        }
        public string GetCurrent()
        {
            DateTime date = DateTime.UtcNow.AddHours(6);
            string message = GetWeather("Погода на " + date.ToString("HH:mm"));
            return message;
        }

        /// <summary>
        /// Формирование сообщения
        /// </summary>
        /// <param name="header">Заговолок возле значка с погодой</param>
        /// <returns></returns>
        private string GetWeather(string header)
        {
            string nameIcon = Weather.Count > 0 ? Weather[0].Id.ToString().GetNameIcon() : string.Empty;
            StringBuilder caption = new StringBuilder(nameIcon);
            caption.Append("  <b>" + header + "</b>");
            if (MainWeather != null)
            {
                if (Weather.Count > 0)
                {
                    caption.Append(Environment.NewLine);
                    caption.Append("     " + Weather[0].Description);
                }

                caption.Append(Environment.NewLine);
                caption.Append(MainWeather.Temp + " °C");
                caption.Append(Environment.NewLine);
                caption.Append("чувствуется как " + MainWeather.FeelsLike + " °C");
            }
            return caption.ToString();
        }

        public string GetToDay()
        {
            StringBuilder caption = new StringBuilder(Environment.NewLine);
            string message = GetWeather("В" + DtAstanaAndDjako.ToString("HH:mm")).ToString();
            caption.Append(message);
            return caption.ToString();
        }

        public string GetTomorow()
        {
            throw new NotImplementedException();
        }

        public string GetLoging()
        {
            StringBuilder log = new StringBuilder(string.Empty);
            if (MainWeather != null)
            {
                for(int i = 0; i < Weather.Count; i++)
                {
                    log.Append("Id = " + Weather[i].Id);
                    log.Append(" Description = " + Weather[i].Description);
                    log.Append(" Main = " + Weather[i].Main);
                    log.Append(Environment.NewLine);
                }
            }
            return log.ToString();
        }

        public string GetLogingToDay()
        {
            throw new NotImplementedException();
        }

        public string GetLogingTomorow()
        {
            throw new NotImplementedException();
        }
    }
}
