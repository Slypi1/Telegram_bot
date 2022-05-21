
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Net;
using System.Net.Http;
using Telegram.Bot.Types.InputFiles;

namespace Telegram_bot
{
    class Program
    {
        static TelegramBotClient? botClient;

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Document != null)
            {
                await DownLoad(botClient,update.Message);
                
            }
                if (update!.Type == UpdateType.Message && update?.Message?.Text != null)
            {


                await HandleMessage(botClient, update.Message);
                return;
            }
        }
        static async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
          
            if (message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привествую, я готов перекинуть твои данные)", replyToMessageId: message.MessageId);
                return;
            }


            if (message.Text == "Привет")
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привет, привет давай кидай чего нибудь)", replyToMessageId: message.MessageId);
            else if ((message!.Text == "/file"))
                DowloadFile(message);
            else await botClient.SendTextMessageAsync(message.Chat.Id, $"Я играю в попугая:\n{message.Text}", replyToMessageId: message.MessageId);







        }
        static async Task DownLoad( ITelegramBotClient botClient, Message message)
        {
           var fileId = message?.Document?.FileId;
           var path = message?.Document?.FileName;
            var file = await botClient.GetFileAsync(fileId!);
            FileStream fs = new FileStream("_" + path, FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath!, fs);
            await botClient.SendTextMessageAsync(message!.Chat.Id, "Сохранил", replyToMessageId: message.MessageId);
            fs.Close();

            fs.Dispose();
        }
        static async void DowloadFile (Message message)
        {
           
            var path = message?.Document?.FileName;
            var info = new DirectoryInfo(@"C:\Users\anast\Downloads\Telegram Desktop");
            try
            {
                FileInfo[] fileinfo = info.GetFiles();
                foreach (var item in fileinfo)
                {
                    var stream = new FileStream(item.FullName, FileMode.Open);
                    var file = new InputOnlineFile(stream, stream.Name);
                    await botClient!.SendDocumentAsync(message!.Chat.Id, file);

                }

            }
            catch( Exception ex)
            {
                Console.WriteLine("Error");

            }
       
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка от телеги:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        static void Main(string[] args)
        {
          
            string token = System.IO.File.ReadAllText(@"C:\Dowload\token.txt");
            botClient = new TelegramBotClient(token);
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = {}
            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token
            );


            Console.WriteLine("Слушаю:");
            Console.ReadLine();


        }
        
        
    }
}

        