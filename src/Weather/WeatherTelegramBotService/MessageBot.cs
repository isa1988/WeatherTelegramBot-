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
        public Message[] SedFoto(ChatId chatId, string caption, List<byte[]> file)
        {
            if (file == null || file.Count == 0) return new Message[2];
            List<InputMediaBase> inputMediaBases = new List<InputMediaBase>();
            List<Message> messages = new List<Message>();
            for (int i = 0; i < file.Count; i++)
            {
                System.IO.Stream fileStream = new System.IO.MemoryStream(file[i]);
                //inputMediaBases.Add(new InputMediaPhoto(new InputMedia(fileStream, caption + i.ToString())));
                var inputMediaBases1 = new Telegram.Bot.Types.InputFiles.InputOnlineFile(fileStream, caption);
                Message message = botClient.SendPhotoAsync(chatId, inputMediaBases1, disableNotification: true).Result;
                messages.Add(message);
            }
            return messages.ToArray();
        }


        /// <summary>
        /// Отправить фото в чат 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="caption"></param>
        /// <param name="file"></param>
        public Message[] EditFoto(Message[] messages, List<byte[]> file)
        {
            if (messages == null || messages.Length == 0) return new Message[2];
            for (int i = 0; i < messages.Length; i++)
            {
                botClient.DeleteMessageAsync(messages[i].Chat, messages[i].MessageId);
            }
            return SedFoto(messages[0].Chat, "редактировать фото", file);
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
