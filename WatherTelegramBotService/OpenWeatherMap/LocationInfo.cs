using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    class LocationInfo
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public string Country { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }
}
