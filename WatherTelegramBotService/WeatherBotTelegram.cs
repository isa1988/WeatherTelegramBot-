using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace WeatherTelegramBotService
{
    public class WeatherBotTelegram : IWeatherBotTelegram
    {
        private ITelegramBotClient botClient;

        public WeatherBotTelegram()
        {
            botClient = new TelegramBotClient("bot token");
        }
        
        public void Run()
        {
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
        }

        void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text == null || e.Message.Text.ToLower() != "/goW") return;
            using (TelegramBotClientWork work = new TelegramBotClientWork(botClient, e.Message.Chat))
            {
                MessageBot messageBot = new MessageBot(botClient);
                messageBot.SendMessage(e.Message.Chat, "я запустился");
                work.Start();
            }
        }
    }
}
