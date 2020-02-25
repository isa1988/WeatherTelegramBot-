using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService
{
    class DataOfWeatherForPicture
    {
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("data")]
        public int Temperature { get; set; }
        public DataOfWeatherForPicture()
        {
            Time = string.Empty;
            Temperature = 0;
        }
        public DataOfWeatherForPicture(string time, int tempeture)
        {
            Time = time;
            Temperature = tempeture;
        }
    }
}
