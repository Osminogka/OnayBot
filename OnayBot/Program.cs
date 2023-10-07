using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Collections.Generic;


namespace OnayBot
{
    internal class Program
    {
        public static Dictionary<string, long> UsersId = new Dictionary<string, long>()
        {
            {"number1", 12345678 },
            {"number2", 12345678 }
        };

        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("Telegram bot token");
            var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token);

            string accountSid = "Account SID";
            string authToken = "Account Token";

            TwilioClient.Init(accountSid, authToken);

            var me = await botClient.GetMeAsync();
           
            Console.ReadLine();
            cts.Cancel();
        }
        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            DateTime now = DateTime.Now;
            string OnayReply = $"ONAY!ALA\n BN {now.Day}/{now.Month} {now.Hour}:{now.Minute}\n{messageText},{GenerateRandomNumber()},80T\n https://qr.tha.kz/799DE ";
            Console.WriteLine("In proccess...");
            bool MessageIsSent = false;

            foreach(var TempUser in UsersId)
            {
                if (TempUser.Value != chatId)
                    continue;
                Console.WriteLine("Sending SMS to " + TempUser.Key);
                var scam_d = MessageResource.Create(
                    body: OnayReply,
                    from: new Twilio.Types.PhoneNumber("bot_number"),
                    to: new Twilio.Types.PhoneNumber(TempUser.Key));
                Thread.Sleep(3000);
                Message sentMessage_d = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Your bus number is: {messageText}\n SMS status: {scam_d.Status}, id: {message.Chat.Id}",
                    cancellationToken: cancellationToken);

                MessageIsSent = true;

                break;
            }

            if(MessageIsSent)
            {
                Console.WriteLine("Message sent");
                return;
            }

            Console.WriteLine("Error: User wasn't found");
        }
        static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            return Task.CompletedTask;
        }
        static string GenerateRandomNumber()
        {
            string numbers = "123456789";
            string letters = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string BusNumber = "";
            Random rnd = new Random();
            int currnum;

            for(int i = 0; i < 3; i++)
            {
                currnum = rnd.Next(numbers.Length);
                BusNumber += numbers[currnum];
            }
            for(int i = 0; i < 2; i++)
            {
                currnum = rnd.Next(letters.Length);
                BusNumber += letters[i];
            }
            for (int i = 0; i < 2; i++)
            {
                currnum = rnd.Next(numbers.Length);
                BusNumber += numbers[i];
            }

            return BusNumber;
        }
    }
}
