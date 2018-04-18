using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsuBot.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BsuBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string NotReadyYetText = "Not ready yet";

        private const string MainMenuInitText = "Hello\r\nPlease select your request from the menu below\r\n";

        private readonly List<string> _mainMenuTexts = new List<string>
        {
            "Procure new Asset",
            "Software Transfer",
            "On-boarding Request",
            "Report Defect",
            "Loaner Laptop"
        };




        private const string OnboardingText =
            "Ok, your choice is {0}.\r\n\r\nWould you like to create a new account or transfer an existing account from different organization? Please select from the menu below\r\n";

        private readonly List<string> _OnboardingTexts = new List<string>
        {
            "New account",
            "Transfer account"
        };

        private readonly List<string> _SmartPhoneTexts = new List<string>
        {
            "Iphone",
            "Samsung",
            "Blackberry"
        };

        #region FieldsToPersist

        private string employeeFirstName;

        private string employeeLastName;

        private string employeePhoneNumber;
        private string smartPhone;

        #endregion


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostWithButtonsAsync(MainMenuInitText, _mainMenuTexts);
            context.Wait(AfterSelectMainMenuOption);
        }

        private async Task AfterSelectMainMenuOption(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var res = await result;
            switch (res.Text)
            {
                case "On-boarding Request":
                    await context.PostWithButtonsAsync(string.Format(OnboardingText, res.Text), _OnboardingTexts);
                    context.Wait(ResumeAfterOnboarding);

                    break;
                default:
                    await context.PostAsync(NotReadyYetText);
                    context.Done(1);
                    break;
            }

        }

        private async Task ResumeAfterOnboarding(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var res = await result;
            switch (res.Text)
            {
                case "New account":
                    PromptDialog.Text(context, ResumeAfterEmployeeFirstName,
                        "Please provide me with the employee first name");

                    break;
                default:
                    await context.PostAsync(NotReadyYetText);
                    context.Done(1);
                    break;
            }
        }

        private async Task ResumeAfterEmployeeFirstName(IDialogContext context, IAwaitable<string> result)
        {
            employeeFirstName = await result;
            PromptDialog.Text(context, ResumeAfterEmployeeLastName, "Please provide me with the employee last name");
        }

        private async Task ResumeAfterEmployeeLastName(IDialogContext context, IAwaitable<string> result)
        {
            employeeLastName = await result;
            PromptDialog.Text(context, ResumeAfterEmployeePhoneNumber,
                "Please provide me with the employee’s phone number");
        }

        private async Task ResumeAfterEmployeePhoneNumber(IDialogContext context, IAwaitable<string> result)
        {
            employeePhoneNumber = await result;
            var txt = $"First name: **{employeeFirstName}**" +
                      $"<br/><br/>Last name: **{employeeLastName}** " +
                      $"<br/><br/>Phone#: **{employeePhoneNumber}**";
            await context.PostAsync(txt);

            PromptDialog.Confirm(context, ResumeAfterConfirm, "Please confirm if the following information is correct");

            
        }
       

        private async Task ResumeAfterConfirm(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                PromptDialog.Confirm(context, ResumeAfterOrderSmartPhone,
                    "Would you like to order a smartphone for the new employee?");
            }
            else
            {
                await context.PostAsync(NotReadyYetText);
                context.Done(1);
            }
        }

        private async Task ResumeAfterOrderSmartPhone(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                PromptDialog.Choice<string>(context, ResumeAfterSmartphoneSelect, _SmartPhoneTexts, "Ok\r\nPlease select a model from the menu below\r\n");
               // await context.PostWithButtonsAsync("Ok\r\nPlease select a model from the menu below\r\n",_SmartPhoneTexts);
               // context.Wait(ResumeAfterSmartphoneSelect);
            }
            else
            {
                await context.PostAsync(NotReadyYetText);
                context.Done(1);
            }
        }

        private async Task ResumeAfterSmartphoneSelect(IDialogContext context, IAwaitable<string> result)
        {
            smartPhone = (await result);
            await context.PostAsync($"Ok, we will order {smartPhone} for you.");
            PromptDialog.Confirm(context, ResumeAfterVpnAnwer, "Does the new employee need a VPN?");
        }

        private async Task ResumeAfterVpnAnwer(IDialogContext context, IAwaitable<bool> result)
        {
            var res = await result;
            if (res)
            {
                var txt =
                    "Ok, we will order you a VPN.\r\n\r\nThank you very much. We will work on your request as soon as possible. \r\n\r\nHave a great day\r\nBye \r\n";
                await context.PostAsync(txt);
                context.Done(1);
            }
            else
            {
                await context.PostAsync(NotReadyYetText);
                context.Done(1);
            }
        }

        /* private async Task ResumeAfterSmartphoneSelect(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            smartPhone = (await result).Text;
            await context.PostAsync($"Ok, we will order {smartPhone} for you.");

        }*/
    }
}