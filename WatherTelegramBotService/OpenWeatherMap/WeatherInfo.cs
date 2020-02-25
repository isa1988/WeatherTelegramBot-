using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    class WeatherInfo
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
