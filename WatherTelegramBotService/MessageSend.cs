using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WatherTelegramBotService
{
    class MessageSend : IDisposable
    {
        private ITelegramBotClient botClient;

        public MessageSend(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        /// <summary>
        /// Посылка сообщения
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        public void SendMessage(ChatId chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
        }


        /// <summary>
        /// Логирование на дату
        /// </summary>
        /// <param name="chat">Чат</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="date">Дата</param>
        public void MessageWithTime(ChatId chat, string message, DateTime date)
        {
            string logMessage = message + " " + date.ToString("dd.MM.yyyy HH:mm:ss");
            Log(chat, logMessage);
        }


        /// <summary>
        /// Логирование на дату
        /// </summary>
        /// <param name="chat">Чат</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="date">Дата</param>
        public async Task<Message> GetMessageWithTime(ChatId chat, string message, DateTime date)
        {
            string logMessage = message + " " + date.ToString("dd.MM.yyyy HH:mm:ss");
            return await Log(chat, logMessage);
        }

        /// <summary>
        /// Посылка сообщения
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        public Task<Message> GetSendMessage(ChatId chatId, string message)
        {
            return botClient.SendTextMessageAsync(chatId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isToTeleTelegram"></param>
        public Task<Message> Log(ChatId currChat, string message, bool isToTeleTelegram = true)
        {
            if (isToTeleTelegram)
            {
                Task<Message> messageResponse = GetSendMessage(currChat, message);
                return messageResponse;
            }
            else
            {
                Console.WriteLine(message);
                return null;
            }
        }

        /// <summary>
        /// Редактирование сообщения 
        /// </summary>
        /// <param name="message">Сообщение которое надо изменить</param>
        /// <param name="messageText">Текст нового сообщения</param>
        /// <param name="isAccumulate">Накапливать по умолчанию да</param>
        public void EditMessage(Message message, string messageText, bool isAccumulate = true)
        {
            if (isAccumulate)
            {
                messageText = message.Text + Environment.NewLine + messageText;
                message.Text += Environment.NewLine + messageText;
            }
            botClient.EditMessageTextAsync(message.Chat, message.MessageId, messageText, Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="message">Сообщение которое надо изменить</param>
        /// <param name="messageText">Текст нового сообщения</param>
        /// <param name="isToTeleTelegram">Вевысти сообщение в бот</param>
        /// <param name="isAccumulate">Накапливать по умолчанию да</param>
        public void LogEdit(Message message, string messageText, bool isToTeleTelegram = true, bool isAccumulate = true)
        {
            if (isToTeleTelegram)
            {
                EditMessage(message, messageText, isAccumulate);
            }
            else
            {
                Console.WriteLine(messageText);
            }
        }

        public void Dispose()
        {
            botClient = null;
        }
    }
}
