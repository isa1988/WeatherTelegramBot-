using System;
using WeatherTelegramBotService;

namespace WeatherTelegramBotConsolApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("сервис запущен, ожидается подтвержение от .......!");
            IWeatherBotTelegram work = new WeatherBotTelegram();
            work.Run();
            Console.ReadLine();
        }
    }
}
