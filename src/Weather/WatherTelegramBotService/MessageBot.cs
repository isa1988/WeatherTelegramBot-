using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherTelegramBotService
{
    class MessageBot
    {
        private ITelegramBotClient botClient;

        public MessageBot(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        /// <summary>
        /// Отправить фото в чат 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="caption"></param>
        /// <param name="file"></param>
        public void SedFoto(ChatId chatId, string caption, byte[] file)
        {
            if (file == null || file.Length == 0) return;
            System.IO.Stream fileStream = new System.IO.MemoryStream(file);
            List<InputMediaBase> inputMediaBases = new List<InputMediaBase>();
            inputMediaBases.Add(new InputMediaPhoto(new InputMedia(fileStream, caption)));
            
            botClient.SendMediaGroupAsync(chatId, inputMediaBases);
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
            SendMessage(chat, logMessage);
        }


        /// <summary>
        /// Посылка сообщения
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        public Task<Message> GetAfterSendMessage(ChatId chatId, string message)
        {
            return botClient.SendTextMessageAsync(chatId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
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
                message.Text += Environment.NewLine + messageText;
                messageText = message.Text;
            }
            botClient.EditMessageTextAsync(message.Chat, message.MessageId, messageText, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        
    }
}
