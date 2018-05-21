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

        /*private readonly List<string> _mainMenuTexts = new List<string>
        {
            "Procure new Asset",
            "Software Transfer",
            "On-boarding Request",
            "Report Defect",
            "Loaner Laptop"
        };*/

        private readonly List<string> _mainMenuTexts = new List<string>
        {
            "AGI Information",
            "Office Locations",
            "Office hours",
            "Please type in a question related to Above Guideline Increase (AGI)",
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
        private string tenantLandlord;
        private string name;

        #endregion


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            /* await context.PostWithButtonsAsync(MainMenuInitText, _mainMenuTexts); */
            await context.PostAsync(
                "Welcome to our chat application. Please note that this application is for testing purpose only");
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
                await context.PostWithButtonsAsync("Thank you! Please select an option from the menu below", _mainMenuTexts);
                context.Wait(AfterSelectMainMenuOption);
            }
            else
            {
                await AskLandLordOrTenant(context);
            }
        }

        private async Task AfterSelectMainMenuOption(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var res = await result;
            switch (res.Text)
            {
                //todo: try pattern matching
                case "AGI Information":
                    await context.PostAsync("<place for any random answer here :) >");
                    break;
                case "Office Locations":
                    var str =
                        "**Sud Office** <br>999 Farch Street, Suite 333\r\nSudbury, Ontario M1M 5B9\r\nFax: 777-333-4444 or 1-888-444-444\r\n<br>" +
                        "**Toronto East Office** \r\n2222 Nidland Venue, Unit 22222\r\nToronto, Ontario M1M 5B9 \r\nFax: 777-333-4444 or 1-888-444-444\r\n<br>" +
                        "**Toronto North Office** \r\n333 Cheppard Venue East, Suite 80000\r\nToronto, Ontario M1M 5B9 \r\nFax: 777-333-4444 or 1-888-444-444\r\n<br>" +
                        "**Toronto South Office** \r\n7777 St. Blair Venue Wast, Suite 11111\r\nToronto, Ontario M1M 5B9 \r\nFax: 777-333-4444 or 1-888-444-444\r\n";
                    await context.PostAsync(str);
                    break;
                case "Office hours":
                    await context.PostAsync("LTA offices are open from 08:30 to 04:00 PM from Monday to Friday");
                    break;
                case "Please type in a question related to Above Guideline Increase (AGI)":
                    await context.PostAsync("QNA will be here");
                   
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