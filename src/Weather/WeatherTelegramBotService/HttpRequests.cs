using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WeatherTelegramBotService
{
    class HttpRequestOfPicure
    {
        /// <summary>
        /// Запрос на создание картинки в разрезе темперуры
        /// </summary>
        /// <param name="jsonTemperature">Данные температуры</param>
        /// <param name="jsonFeelTemperature">Данные о температуре по ощущениям</param>
        /// <param name="dateWeather">На дату</param>
        /// <returns></returns>
        public byte[] GetPhotoTempeture(string jsonTemperature, string jsonFeelTemperature, string dateWeather)
        {
            byte[] foto = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                MediaTypeWithQualityHeaderValue mediaValue = new MediaTypeWithQualityHeaderValue("application/json");
                mediaValue.CharSet = "utf-8";
                client.DefaultRequestHeaders.Accept.Add(mediaValue);

                using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "charttemperature/"))
                {
                    requestMessage.Headers.Clear();
                    requestMessage.Headers.Add("temperature", jsonTemperature);
                    requestMessage.Headers.Add("feeltemperature", jsonFeelTemperature);
                    requestMessage.Headers.Add("datestr", dateWeather);
                    HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                    foto = response.Content.ReadAsByteArrayAsync().Result;
                }
            }
            return foto;
        }


        /// <summary>
        /// Запрос на создание картинки в разрезе описание температуры
        /// </summary>
        /// <param name="jsonDescTemperature">Данные температуре</param>
        /// <param name="jsonWeatherInfo">Данные по описанию</param>
        /// <param name="dateWeather">На дату</param>
        /// <returns></returns>
        public byte[] GetPhotoDescTempeture(string jsonDescTemperature, string jsonWeatherInfo, string dateWeather)
        {
            byte[] foto = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                MediaTypeWithQualityHeaderValue mediaValue = new MediaTypeWithQualityHeaderValue("application/json");
                mediaValue.CharSet = "utf-8";
                client.DefaultRequestHeaders.Accept.Add(mediaValue);

                using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "chartdesc/"))
                {
                    requestMessage.Headers.Clear();
                    requestMessage.Headers.Add("desctemperature", jsonDescTemperature);
                    requestMessage.Headers.Add("weatherinfo", jsonWeatherInfo);
                    requestMessage.Headers.Add("datestr", dateWeather);
                    HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                    foto = response.Content.ReadAsByteArrayAsync().Result;
                }
            }
            return foto;
        }

    }
}
