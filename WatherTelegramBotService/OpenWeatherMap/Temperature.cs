using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    class Temperature
    {
        public float Temp { get; set; }
        [JsonProperty("feels_like")]
        public float FeelsLike { get; set; }
        [JsonProperty("temp_min")]
        public float TempMin { get; set; }
        [JsonProperty("temp_max")]
        public float TempMax { get; set; }
        public float Pressure { get; set; }
        public float Humidity { get; set; }
    }
}
