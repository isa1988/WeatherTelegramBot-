using System;
using System.Collections.Generic;
using System.Text;

namespace WatherTelegramBotService
{
    interface IWeather
    {
        string GetCurrent();
        string GetToDay();
        string GetTomorow();
    }
}
