using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using NasaApp.Models;

namespace NasaApp.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly TelegramBotClient _botClient;
        private readonly NasaApiService _nasaService;

        public TelegramBotService(NasaApiService nasaService)
        {
            _botClient = new TelegramBotClient("TelegramBotToken");
            _nasaService = nasaService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = new Telegram.Bot.Polling.ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
            _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, options, stoppingToken);
            Console.WriteLine("Telegram Bot is running...");
            await Task.Delay(-1, stoppingToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                if (update.Message.Text == "/start")
                {
                    var kb = new InlineKeyboardMarkup(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("🌌 Фото сьогоднішнього дня", "apod_today") },
                        new[] { InlineKeyboardButton.WithCallbackData("🎲 Випадкове космічне фото", "apod_random") },
                        new[] { InlineKeyboardButton.WithCallbackData("☄️ Астероїди поблизу Землі", "asteroids_today") }
                    });

                    await bot.SendTextMessageAsync(update.Message.Chat.Id,
                        "Вітаю в NASAgram! Виберіть знизу Inlie-кнопку.",
                        replyMarkup: kb, cancellationToken: ct);
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                string data = update.CallbackQuery.Data;
                long chatId = update.CallbackQuery.Message.Chat.Id;

                try
                {
                    if (data == "apod_today" || data == "apod_random")
                    {
                        await bot.SendTextMessageAsync(chatId, "⏳ Звертаюсь NASA...", cancellationToken: ct);

                        var apod = (data == "apod_today")
                            ? await _nasaService.GetPhotoOfTheDayAsync()
                            : await _nasaService.GetRandomPhotoAsync();

                        if (apod != null)
                        {
                            string caption = $"<b>{apod.Title}</b>\n\n{apod.Explanation}";
                            if (caption.Length > 1000) caption = caption.Substring(0, 1000) + "...";

                            if (apod.MediaType == "image")
                                await bot.SendPhotoAsync(chatId, InputFile.FromUri(apod.Url), caption: caption, parseMode: ParseMode.Html, cancellationToken: ct);
                            else
                                await bot.SendTextMessageAsync(chatId, $"📹 Це відео: {apod.Title}\n{apod.Url}", cancellationToken: ct);
                        }
                    }
                    else if (data == "asteroids_today")
                    {
                        await bot.SendTextMessageAsync(chatId, "📡 Шукаємо астероїди...", cancellationToken: ct);
                        var asteroids = await _nasaService.GetAsteroidsTodayAsync();

                        if (asteroids != null)
                        {
                            await bot.SendTextMessageAsync(chatId,
                                $"☄️ <b>Астероїдний звіт</b>\n\nСьогодні повз Землю пролетить об'єктів: <b>{asteroids.ElementCount}</b>",
                                parseMode: ParseMode.Html, cancellationToken: ct);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await bot.SendTextMessageAsync(chatId, $"❌ Помилка: {ex.Message}", cancellationToken: ct);
                }

                await bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: ct);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
        {
            Console.WriteLine($"Помилка бота: {ex.Message}");
            return Task.CompletedTask;
        }
    }
}