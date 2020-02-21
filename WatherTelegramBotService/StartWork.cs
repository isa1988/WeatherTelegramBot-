using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace WatherTelegramBotService
{
    public class StartWork : IStartWork
    {
        public ITelegramBotClient botClient;

        public StartWork()
        {
            botClient = new TelegramBotClient("your bot token");
            /*var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName} username {me.Username}.");*/

            
        }
        
        public async void Run()
        {
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
        }

        async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text == null || e.Message.Text.ToLower() != "/go") return;
            while(true)
            {
                using(TelegramBotClientWork work = new TelegramBotClientWork(botClient, e.Message.Chat))
                {
                    int timeSleep = work.GetMainTimer();
                    Thread.Sleep(timeSleep);
                }
            }
        }
    }
}
