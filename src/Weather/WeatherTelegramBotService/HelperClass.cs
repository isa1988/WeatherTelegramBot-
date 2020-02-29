using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTelegramBotService
{
    static class HelperClass
    {
        public static DateTime GetAstanaAndDjako()
        {
            return DateTime.UtcNow.AddHours(6);
        }
        public static DateTime GetAstanaAndDjakoTomorow()
        {
            return DateTime.UtcNow.AddHours(6).AddDays(1);
        }
    }
}
