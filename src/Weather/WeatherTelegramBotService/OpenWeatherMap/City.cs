using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    class City
    {
        public long Id { get; set; }
        
        public string Name { get; set; }

        public Coordinate Coord { get; set; }
        
        public string Country { get; set; }

        public long Population { get; set; }

        public long Timezone { get; set; }

        public long Sunrise { get; set; }

        public long Sunset { get; set; }
    }
}
