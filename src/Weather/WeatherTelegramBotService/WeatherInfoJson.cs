using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService
{
    class WeatherInfoJson
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        public WeatherInfoJson()
        {
            Id = 0;
            Desc = string.Empty;
        }
    }
}
