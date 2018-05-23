using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BsuBot.Extensions
{
    public static class DialogExtensions
    {
        public static async Task PostWithButtonsAsync(this IDialogContext context, string text,
            List<CardAction> buttons)
        {


            var mes = context.MakeMessage();
            mes.Text = text;

            var cardForButton = new ThumbnailCard {Buttons = buttons};
            mes.Attachments.Add(cardForButton.ToAttachment());

            await context.PostAsync(mes);
        }

        public static async Task PostWithButtonsAsync(this IDialogContext context, string text,
            List<string> buttons)
        {
            List<CardAction> buttonsCards = new List<CardAction>();

            foreach (var buttonCapture in buttons)
            {
                var button = new CardAction
                {
                    DisplayText = buttonCapture,
                    Text = buttonCapture,
                    Type = "imBack",
                    Value = buttonCapture,
                    Title = buttonCapture
                  
                };
                buttonsCards.Add(button);
            }

            await PostWithButtonsAsync(context, text, buttonsCards);
        }
    }
}