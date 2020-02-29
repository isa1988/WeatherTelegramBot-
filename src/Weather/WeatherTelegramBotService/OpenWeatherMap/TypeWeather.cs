using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService.OpenWeatherMap
{
    enum TypeWeather
    {
        /// <summary>
        /// Гроза 
        /// </summary>
        Thunderstorm = 2,

        /// <summary>
        /// Мокрый дождь
        /// </summary>
        Drizzle,

        /// <summary>
        /// Дождь
        /// </summary>
        Rain = 5,

        /// <summary>
        /// Снег
        /// </summary>
        Snow = 6,

        /// <summary>
        /// Туман
        /// </summary>
        Atmosphere = 7,

        /// <summary>
        /// Ясно
        /// </summary>
        Clear = 8,

        /// <summary>
        /// Облака
        /// </summary>
        Cloud = 9
    }
}
