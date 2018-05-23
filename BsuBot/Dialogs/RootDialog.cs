using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BsuBot.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace BsuBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private QnaDialog _qnaDialog;

        private const string NotReadyYetText = "Not ready yet";

        private const string MainMenuInitText = "Hello\r\nPlease select your request from the menu below\r\n";

        /*private readonly List<string> _mainMenuTexts = new List<string>
        {
            "Procure new Asset",
            "Software Transfer",
            "On-boarding Request",
            "Report Defect",
            "Loaner Laptop"
        };*/

        public RootDialog(QnaDialog qnaDialog)
        {
            SetField.NotNull(out _qnaDialog, nameof(_qnaDialog), qnaDialog);
        }

        private readonly List<string> _mainMenuOptions = new List<string>
        {
           // "AGI Information",
            "Office Locations",
            "Office hours",
            "Ask question related to AGI Guideline"
        };

        private string name;
        private string tenantLandlord;


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            /* await context.PostWithButtonsAsync(MainMenuInitText, _mainMenuTexts); */
            await context.PostAsync(
                "Welcome to our chat application. Please note that this application is for testing purpose only. You can type **menu** anywhere to get to main menu.");
             await AskLandLordOrTenant(context);

            
        }

       

        private async Task AskLandLordOrTenant(IDialogContext context)
        {
            await context.PostWithButtonsAsync("Are you a Landlord or Tenant?", new List<string> { "Landlord", "Tenant" });
            context.Wait(AfterSelectLandlordTenantOption);
        }

        private async Task AfterSelectLandlordTenantOption(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var res = await result;
            switch (res.Text)
            {
               
                case "Tenant":
                    tenantLandlord = "Tenant";
                    /*await context.PostWithButtonsAsync(string.Format(OnboardingText, res.Text), _OnboardingTexts);
                    context.Wait(ResumeAfterOnboarding);*/

                    break;
                case "Landlord":
                    tenantLandlord = "Landlord";
                    break;
                default:
                    await context.PostAsync("Not recognized");
                    context.Done(1);
                    return;
                    break;

            }

            await context.PostAsync("ok! Thanks");
            PromptDialog.Text(context, AfterNameResume, "what is you name please ?");

        }

        private async Task AfterNameResume(IDialogContext context, IAwaitable<string> result)
        {
            name = await result;
            await context.PostAsync($"Thanks {name}");
            await context.PostAsync("please confirm the information you entered: this could be yes or no to confirm");
            PromptDialog.Confirm(context, AfterRightOrNo, $"You are: {tenantLandlord} and your name is {name}");
        }

        private async Task AfterRightOrNo(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                await ShowMainMenu(context);
            }
            else
            {
                await AskLandLordOrTenant(context);
            }
        }

        private async Task ShowMainMenu(IDialogContext context)
        {
            await context.PostWithButtonsAsync("Thank you! Please select an option from the menu below", _mainMenuOptions);
            context.Wait(AfterSelectMainMenuOption);
        }

        private async Task AfterSelectMainMenuOption(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var res = await result;
            switch (res.Text)
            {
                //todo: try pattern matching
                case "AGI Information":
                    await context.PostAsync("place for any random answer here :) ");
                    context.Wait(AfterSelectMainMenuOption);
                    break;
                case "Office Locations":
                    var str =
                        "**Sud Office** 999 Farch Street, Suite 333\r\nSudbury, Ontario M1M 5B9\r\nFax: 777-333-4444 or 1-888-444-444\r\n\r\n" +
                        "**Toronto East Office** \r\n2222 Nidland Venue, Unit 22222\r\nToronto, Ontario M1M 5B9 \r\nFax: 777-333-4444 or 1-888-444-444\r\n\r\n" +
                        "**Toronto North Office** \r\n333 Cheppard Venue East, Suite 80000\r\nToronto, Ontario M1M 5B9 \r\nFax: 777-333-4444 or 1-888-444-444\r\n\r\n" +
                        "**Toronto South Office** \r\n7777 St. Blair Venue Wast, Suite 11111\r\nToronto, Ontario M1M 5B9 \r\nFax: 777-333-4444 or 1-888-444-444\r\n";
                    await context.PostAsync(str);
                    context.Wait(AfterSelectMainMenuOption);
                    break;
                case "Office hours":
                    await context.PostAsync("LTA offices are open from 08:30 to 04:00 PM from Monday to Friday");
                    context.Wait(AfterSelectMainMenuOption);
                    break;
                case "Ask question related to AGI Guideline":
                    context.Call(new ExceptionHandlerDialog<object>(_qnaDialog, true),
                        ResumeAfterQna);
                    break;
                default:
                    await context.PostAsync("can't recognize");
                    await ShowMainMenu(context);
                    break;
            }
        }

        private async Task ResumeAfterQna(IDialogContext context, IAwaitable<object> result)
        {
            await ShowMainMenu(context);
           
        }

    }
}