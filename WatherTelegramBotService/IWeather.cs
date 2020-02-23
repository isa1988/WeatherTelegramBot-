using System;
using System.Collections.Generic;
using System.Text;

namespace WatherTelegramBotService
{
    interface IWeather
    {
        /// <summary>
        /// Погода на текущий момент
        /// </summary>
        /// <returns></returns>
        string GetCurrent();
        /// <summary>
        /// Погода на сегодня
        /// </summary>
        /// <returns></returns>
        string GetToDay();
        /// <summary>
        /// Погода на завтра
        /// </summary>
        /// <returns></returns>
        string GetTomorow();
        /// <summary>
        /// Логирование погоды на текущий момент
        /// </summary>
        /// <returns></returns>
        string GetLoging();
        /// <summary>
        /// Логирование погоды на сегодня
        /// </summary>
        /// <returns></returns>
        string GetLogingToDay();
        /// <summary>
        /// Логирование погоды на завтра
        /// </summary>
        /// <returns></returns>
        string GetLogingTomorow();
    }
}
