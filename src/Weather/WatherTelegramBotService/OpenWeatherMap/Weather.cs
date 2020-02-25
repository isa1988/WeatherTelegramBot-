using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    class Weather
    {
        [JsonProperty("coord")]
        public Coordinate Coordinate { get; set; }
        [JsonProperty("weather")]
        public List<WeatherInfo> WeatherInfo { get; set; }
        [JsonProperty("base")]
        public string BaseFrom { get; set; }
        [JsonProperty("main")]
        public Temperature Temperature { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public int Dt { get; set; }
        [JsonProperty("sys")]
        public LocationInfo LocationInfo { get; set; }
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
        public string GetWeatherOfCurrent()
        {
            DateTime dateAstanaAndDjako = HelperClass.GetAstanaAndDjako();
            string message = GetWeather("Погода на " + dateAstanaAndDjako.ToString("HH:mm"));
            return message;
        }

        /// <summary>
        /// Формирование сообщения
        /// </summary>
        /// <param name="header">Заговолок возле значка с погодой</param>
        /// <returns></returns>
        private string GetWeather(string header)
        {
            string nameIcon = GetNameIcon(WeatherInfo[0].Id);
            StringBuilder caption = new StringBuilder(nameIcon);
            caption.Append("  <b>" + header + "</b>");
            if (Temperature != null)
            {
                if (WeatherInfo.Count > 0)
                {
                    caption.Append(Environment.NewLine);
                    caption.Append("     " + Temperature.Temp + " °C  " + WeatherInfo[0].Description);
                }

                caption.Append(Environment.NewLine);
                caption.Append("чувствуется как " + Temperature.FeelsLike + " °C");
            }
            return caption.ToString();
        }

        public string GetWeatherOfToDayOrTomorow()
        {
            StringBuilder caption = new StringBuilder(Environment.NewLine);
            string message = GetWeather("В " + DtAstanaAndDjako.ToString("HH:mm")).ToString();
            caption.Append(message);
            return caption.ToString();
        }

        public string GetLoging()
        {
            StringBuilder log = new StringBuilder(string.Empty);
            if (Temperature != null)
            {
                for(int i = 0; i < WeatherInfo.Count; i++)
                {
                    log.Append("Id = " + WeatherInfo[i].Id);
                    log.Append(" Description = " + WeatherInfo[i].Description);
                    log.Append(" Main = " + WeatherInfo[i].Main);
                    log.Append(Environment.NewLine);
                }
            }
            return log.ToString();
        }

        private string GetNameIcon(int idWeather)
        {
            string retVal = string.Empty;

            int idFirstNumber = (int)Char.GetNumericValue(idWeather.ToString(), 0);
            if (idFirstNumber == (int)TypeWeather.Thunderstorm)
            {
                retVal = char.ToString('\u26C8');
            }
            else if (idFirstNumber == (int)TypeWeather.Drizzle)
            {
                retVal = char.ToString('\u2614');
            }
            else if (idFirstNumber == (int)TypeWeather.Rain)
            {
                retVal = "🌧";
            }
            else if (idFirstNumber == (int)TypeWeather.Snow)
            {
                retVal = "🌨";
            }
            else if (idFirstNumber == (int)TypeWeather.Atmosphere)
            {
                retVal = "🌫";
            }
            else if (idFirstNumber == (int)TypeWeather.Clear)
            {
                if (idWeather == 800)
                {
                    retVal = "☀";
                }
                else
                {
                    retVal = "☁";
                }
            }
            else if (idFirstNumber == (int)TypeWeather.Cloud)
            {
                retVal = "☁";
            }

            return retVal;
        }
    }
}
