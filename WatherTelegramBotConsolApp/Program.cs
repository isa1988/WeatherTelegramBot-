using System;
using WatherTelegramBotService;

namespace WatherTelegramBotConsolApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            StartWork work = new StartWork();
            work.Run();
            Console.ReadLine();
        }
    }
}
