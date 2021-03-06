﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using BsuBot.Dialogs;
using BsuBot.Logger;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using NLog;

namespace BsuBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                var logger = new LoggerService<ILogger>();
                logger.Info("Starting service...");

                var config = GlobalConfiguration.Configuration;

                Conversation.UpdateContainer(
                    builder =>
                    {
                        builder.RegisterGeneric(typeof(LoggerService<>)).As(typeof(ILoggerService<>))
                            .InstancePerDependency();

                        // Register your Web API controllers.
                        builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
                        builder.RegisterWebApiFilterProvider(config);

                        builder.RegisterType<RootDialog>().AsSelf().InstancePerDependency();
                        builder.RegisterType<QnaDialog>().AsSelf().InstancePerDependency();

                        //BOT Data storage
                        builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                        var store = new InMemoryDataStore();

                        builder.Register(c => store)
                            .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                            .AsSelf()
                            .SingleInstance();
                    });

                // Set the dependency resolver to be Autofac.
                config.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);


                GlobalConfiguration.Configure(cfg =>
                {
                    cfg.MapHttpAttributeRoutes();

                    cfg.Routes.MapHttpRoute(
                        "DefaultApi",
                        "api/{controller}/{id}",
                        new {id = RouteParameter.Optional}
                    );

                    // Register your Web API controllers.

                });

                logger.Info("Service successfully started.");

            }
            catch (Exception e)
            {
                var logger = new LoggerService<ILogger>();
                logger.Error(e);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
