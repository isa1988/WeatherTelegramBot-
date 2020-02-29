using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherTelegramBotService
{
    class HttpRequestOfOpenWeatherMap
    {
        /// <summary>
        /// Настройки для запроса на погоду
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetResponseMessage(HttpClient client, string url)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Add("x-rapidapi-host", "");
                requestMessage.Headers.Add("x-rapidapi-key", "");
                return await client.SendAsync(requestMessage);
            }
        }

        /// <summary>
        /// Погода на текущий момент 
        /// </summary>
        /// <returns></returns>
        public string CurrentWeatther()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/weather?lang=ru&units=metric&q=Bishkek";

                HttpResponseMessage response = GetResponseMessage(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                return responseAsString;
            }
        }

        /// <summary>
        /// Погода на текущий день 
        /// </summary>
        /// <returns></returns>
        public string CurrentWeattherToday()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/forecast?q=Bishkek&units=metric&lang=ru";

                HttpResponseMessage response = GetResponseMessage(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                return responseAsString;
            }
        }

        /// <summary>
        /// Погода на завтра
        /// </summary>
        /// <returns></returns>
        public string CurrentWeattherTomorow()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://community-open-weather-map.p.rapidapi.com/forecast?q=Bishkek&units=metric&lang=ru";

                HttpResponseMessage response = GetResponseMessage(client, url).Result;

                string responseAsString = response.Content.ReadAsStringAsync().Result;

                return responseAsString;
            }
        }
    }
}
