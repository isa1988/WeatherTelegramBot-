using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace WatherTelegramBotService
{
    public class StartWork : IStartWork
    {
        public ITelegramBotClient botClient;

        public StartWork()
        {
            botClient = new TelegramBotClient("bot token");
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
            //для тестов 
            /*if (e.Message?.Text == "/test")
            {
                Message message1;
                message1 = await SendMessage(e.Message.Chat, "Тест");
                for(int i = 0; i<100; i++)
                {
                    EditMessage(e.Message.Chat, message1, "Test " + i);
                }
            }*/
            if (e.Message.Text == null || e.Message.Text.ToLower() != "/go") return;
            TelegramBotClientWork work = new TelegramBotClientWork(botClient, e.Message.Chat);
            using(MessageSend messageSend = new MessageSend(botClient))
            {
                await messageSend.Log(e.Message.Chat, "я запустился");
            }
            work.Start();
            /*object myLock = new object();
            Message message = null;
            Message messageLog = null;
            while (true)
            {
                using(TelegramBotClientWork work = new TelegramBotClientWork(botClient, e.Message.Chat, message, messageLog))
                {
                    int timeSleep = await work.GetMainTimer();
                    if (work.MessageAsync != null)
                    {
                        message = work.MessageAsync.Result;
                        messageLog = work.MessageLogAsnc.Result;
                    }
                    Thread.Sleep(timeSleep);
                }
            }*/

        }


        void EditMessage(Chat chat, Message message, string messageText)
        {
            botClient.EditMessageTextAsync(chat, message.MessageId, messageText, Telegram.Bot.Types.Enums.ParseMode.Html);
        }


        async Task<Message> SendMessage(ChatId chatId, string message)
        {
           return await botClient.SendTextMessageAsync(chatId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}
