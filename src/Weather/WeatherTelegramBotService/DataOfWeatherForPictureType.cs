using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService
{
    /// <summary>
    /// Кретерий для картинки
    /// </summary>
    enum DataOfWeatherForPictureType : int
    {
        /// <summary>
        /// Температура
        /// </summary>
        Temperature,

        /// <summary>
        /// Температура по осущениям
        /// </summary>
        FeelTemperature,

        /// <summary>
        /// Описание
        /// </summary>
        Desc,
    }
}
