# BsuBot


## VS Solution
Solution contain only libs that is publicly available on nuget, so just build the solution and nuget must get them all and build correctly with no issues.

All needed config strings are presented in Web.config file: BotId, MicrosoftAppId, MicrosoftAppPassword, they can be obtained in ms bot connector page. More about this variables - below in the text.

Solution consist of 1 project now and its a web api asp.net app.

App_start/WebApiConfig.cs - general configurations of webapi (json serialisation for example)
Global.asax - mainly place for configuring of IOC (Autofac used)
Controllers - folder for controllers (not much of them usually, now here only main controller to get all requests)
Dialogs - folder for dialogs - core abstractions for interactions. More about it https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-dialogs

## How to setup bots in Azure and connect them to channels
Good documentation on creating bot from Azure is here: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart 
but what i don't like about it that it also creates initial solution with lots of unneeded preferences. So after creating it from azure GUI i take publish profile, republish my own solution from VS and deleting all app settings in portal.
Just don't forget to set up BotId, MicrosoftAppId, MicrosoftAppPassword in web.config or in app settings on azure.

To connect new channel go to "Bot Services" -> select your bot -> Channels -> in right panel lots of channels are presented. If you choose one very descriptive step-by-step instructions will be shown on how to connect to one or another channel, so i will not write them here.

## Contact info
Developer is Andrey Stepanov. @Unders0n on github, vzzlom@gmail.com


