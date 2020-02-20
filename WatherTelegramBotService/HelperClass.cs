using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WatherTelegramBotService.OpenWeatherMap;

namespace WatherTelegramBotService
{
    static class HelperClass
    {
        public static string GetNameIcon(this string idWeather)
        {
            string retVal = string.Empty;

            int idFirstNumber = idWeather?.Length > 0 ? int.Parse(idWeather[0].ToString()) : 0;
            if (idFirstNumber == (int)TypeWeather.Thunderstorm)
            {
                retVal = char.ToString('\u26C8');
            }
            else if (idFirstNumber == (int)TypeWeather.Drizzle)
            {
                retVal = char.ToString('\u2614');
            }
            else if (idFirstNumber == (int)TypeWeather.Rain)
            {
                retVal = "🌧";
            }
            else if (idFirstNumber == (int)TypeWeather.Snow)
            {
                retVal = "🌨";
            }
            else if (idFirstNumber == (int)TypeWeather.Atmosphere)
            {
                retVal = "🌫";
            }
            else if (idFirstNumber == (int)TypeWeather.Clear)
            {
                retVal = "☀";
            }
            else if (idFirstNumber == (int)TypeWeather.Cloud)
            {
                retVal = "☁";
            }

            return retVal;
        }

    }
}
